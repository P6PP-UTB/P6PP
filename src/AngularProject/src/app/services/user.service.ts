import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  nameid: string;
  name: string;
}

interface UpdateUserRequest {
  username?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  phoneNumber?: string;
  weight?: number;
  height?: number;
  sex?: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private userBaseUrl =
    'https://userservice.thankfulflower-27b66160.polandcentral.azurecontainerapps.io/api/user';
  private authBaseUrl =
    'https://authservice.internal.thankfulflower-27b66160.polandcentral.azurecontainerapps.io/api/auth';

  constructor(private http: HttpClient) {}

  getUserIdFromToken(): number | null {
    const token = localStorage.getItem('token');
    if (!token) {
      console.warn('🚫 Токен не найден');
      return null;
    }

    try {
      const decoded: any = jwtDecode(token);
      const userId =
        decoded[
          'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
        ];
      return parseInt(userId);
    } catch (err) {
      console.error('Decoding error: ', err);
      return null;
    }
  }

  getCurrentUser(): Observable<any> {
    const id = this.getUserIdFromToken();
    if (!id) return of(null);
    return this.getUserById(id);
  }

  getUserById(id: number): Observable<any> {
    return this.http.get(`${this.userBaseUrl}/${id}`);
  }

  updateUser(id: number, payload: UpdateUserRequest): Observable<any> {
    return this.http.put(`${this.userBaseUrl}/${id}`, payload);
  }

  resetPassword(userId: number, newPassword: string): Observable<any> {
    return this.http.post(`${this.authBaseUrl}/reset-password`, {
      userId,
      newPassword,
    });
  }

  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.userBaseUrl}/${id}`);
  }
}
