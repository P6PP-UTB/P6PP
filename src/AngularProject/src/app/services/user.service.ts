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
    'https://authservice.thankfulflower-27b66160.polandcentral.azurecontainerapps.io/api/auth';

  constructor(private http: HttpClient) {}
  
  getUserIdFromToken(): number | null {
    const token = localStorage.getItem('token');
    if (!token) {
      console.warn('No token found');
      return null;
    }
  
    try {
      const decoded: any = jwtDecode(token);
      console.log('Decoded token:', decoded); // Log the whole token to see its structure
      
      // Check different possible claim names
      let userId = decoded['userid'] || // from AuthConstants.UserId
                 decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ||
                 decoded['nameid'] ||
                 decoded['sub'];
                 
      if (!userId) {
        console.error('Could not find user ID in token. Token claims:', Object.keys(decoded));
        return null;
      }
      
      return parseInt(userId, 10);
    } catch (err) {
      console.error('Error decoding token:', err);
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
