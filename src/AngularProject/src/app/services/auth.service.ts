import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { RegisterPayload } from './interfaces/registerPayload';
import { LoginPayload } from './interfaces/loginPayload';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl =
    'https://authservice.internal.thankfulflower-27b66160.polandcentral.azurecontainerapps.io/api/auth';
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
