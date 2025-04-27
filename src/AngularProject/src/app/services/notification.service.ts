import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = 'http://localhost:5181/api/notification'; // URL для получения уведомлений

  constructor(private http: HttpClient) { }

  // Метод для получения уведомлений
  getNotifications(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl); // Выполняем GET-запрос
  }
}
