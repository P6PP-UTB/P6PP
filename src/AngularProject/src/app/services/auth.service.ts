import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { RegisterPayload } from './interfaces/registerPayload';
import { LoginPayload } from './interfaces/loginPayload';
import { environment } from '../../enviroments/enviroment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = environment.api.auth;
  private _isLoggedIn$ = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) {}

  registerUser(payload: RegisterPayload): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, payload);
  }

  loginUser(payload: LoginPayload): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, payload);
  }

  logout(): void {
    this.http.post(`${this.baseUrl}/logout`, {}).subscribe({
      next: () => {
        this.clearSession();
      },
      error: () => {
        this.clearSession();
      }
    });
  }

  setSession(token: string, userId: number): void {
    localStorage.setItem('token', token);
    localStorage.setItem('userId', userId.toString());
    this._isLoggedIn$.next(true);
  }

  clearSession(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    this._isLoggedIn$.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUserId(): number | null {
    const id = localStorage.getItem('userId');
    return id ? parseInt(id, 10) : null;
  }

  get isLoggedIn$() {
    return this._isLoggedIn$.asObservable();
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }
}