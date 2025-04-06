import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  nameid: string;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private userBaseUrl = 'http://localhost:5189/api/user';

  constructor(private http: HttpClient) {}

  // 💉 Декодим userId из JWT токена
  getUserIdFromToken(): number | null {
    const token = localStorage.getItem('token');
    if (!token) {
      console.warn('🚫 Токен не найден');
      return null;
    }

    try {
      const decoded: any = jwtDecode(token);


      // Используем ключ по полной ссылке, как в токене
      const userId = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
      return parseInt(userId);
    } catch (err) {
      console.error('❌ Ошибка при декодировании токена:', err);
      return null;
    }
  }

  // 🔥 Получаем текущего пользователя через токен
  getCurrentUser(): Observable<any> {
    const id = this.getUserIdFromToken();
    if (!id) return of(null);

    return this.getUserById(id);
  }

  // 🚚 Получаем пользователя по ID
  getUserById(id: number): Observable<any> {
    return this.http.get(`${this.userBaseUrl}/${id}`);
  }
}
