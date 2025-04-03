import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

interface RegisterPayload {
  userName: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

interface LoginPayload {
  usernameOrEmail: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = 'http://localhost:8005/api/auth';
  private _isLoggedIn$ = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) {}

  registerUser(payload: RegisterPayload): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, payload);
  }

  loginUser(payload: LoginPayload): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, payload);
  }

  logout() {
    localStorage.removeItem('token');
    this._isLoggedIn$.next(false); // ⚡ Оповещаем всех
  }

  setToken(token: string) {
    localStorage.setItem('token', token);
    this._isLoggedIn$.next(true); // ⚡ Оповещаем всех
  }

  get isLoggedIn$() {
    return this._isLoggedIn$.asObservable();
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }


}