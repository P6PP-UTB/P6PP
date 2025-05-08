import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, tap, throwError } from 'rxjs';
import { RegisterPayload } from './interfaces/registerPayload';
import { LoginPayload } from './interfaces/loginPayload';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl =
    'https://authservice.thankfulflower-27b66160.polandcentral.azurecontainerapps.io/api/auth';
  private _isLoggedIn$ = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) {}

  registerUser(payload: RegisterPayload): Observable<any> {
    console.log('Registration payload:', payload);
    return this.http.post(`${this.baseUrl}/register`, payload).pipe(
      tap(response => console.log('Registration successful:', response)),
      catchError(this.handleError)
    );
  }

  loginUser(payload: LoginPayload): Observable<any> {
    //console.log('Login payload before mapping:', payload);
    
    // Match the C# model property exactly - "UsernameOrEmail"
    const apiPayload = {
      usernameOrEmail: payload.usernameOrEmail,
      password: payload.password
    };
    
    //console.log('Login payload after mapping:', apiPayload);
    
    return this.http.post(`${this.baseUrl}/login`, apiPayload).pipe(
      tap(response => console.log('Login successful:', response)),
      catchError(this.handleError)
    );
  }

  logout() {
    localStorage.removeItem('token');
    this._isLoggedIn$.next(false);
  }

  setToken(token: string) {
    localStorage.setItem('token', token);
    this._isLoggedIn$.next(true);
  }

  get isLoggedIn$() {
    return this._isLoggedIn$.asObservable();
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }
  
  private handleError(error: HttpErrorResponse) {
    console.error('API Error:', error);
    
    let errorMessage = 'An unknown error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.status === 400) {
        // Try to extract validation errors
        try {
          if (typeof error.error === 'string') {
            errorMessage = error.error;
          } else if (error.error.message) {
            errorMessage = error.error.message;
          } else if (error.error.errors) {
            // Format validation errors
            const validationErrors = error.error.errors;
            const errorMessages = [];
            
            for (const key in validationErrors) {
              if (Object.prototype.hasOwnProperty.call(validationErrors, key)) {
                errorMessages.push(`${key}: ${validationErrors[key].join(', ')}`);
              }
            }
            
            errorMessage = errorMessages.join('; ');
          } else {
            errorMessage = JSON.stringify(error.error);
          }
        } catch (e) {
          console.error('Error parsing API response:', e);
          errorMessage = 'Invalid response format';
        }
      } else {
        errorMessage = `Server error ${error.status}: ${error.message}`;
      }
    }
    
    return throwError(() => new Error(errorMessage));
  }
}