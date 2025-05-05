import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export interface Payment {
  userId: number;
  roleId: number;
  transactionType: string;
  amount: number;
}

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private getBalanceUrl = 'http://localhost:5185/api/getbalance/';
  private createPaymentUrl = 'http://localhost:5185/api/createpayment';

  constructor(private http: HttpClient) {}

  getUserBalance(): Observable<number> {
    const id = localStorage.getItem('userId');

    if (!id) {
      return throwError(() => new Error('User ID not found in localStorage'));
    }

    const reqUrl = this.getBalanceUrl + id;

    return this.http.get<any>(reqUrl).pipe(
      map(response => response.data.creditBalance),
      catchError(error => {
        console.error('Ошибка при получении баланса:', error);
        return throwError(() => error);
      })
    );
  }

  createPayment(payment: Payment): Observable<any> {
    return this.http.post<any>(this.createPaymentUrl, payment).pipe(
      catchError(error => {
        console.error('Ошибка при создании платежа:', error);
        return throwError(() => error);
      })
    );
  }
}
