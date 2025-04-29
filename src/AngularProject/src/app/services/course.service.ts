import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom, Observable, of } from 'rxjs';
import { Course } from './interfaces/course';

@Injectable({ providedIn: 'root' })
export class CourseService {
  private requestAllURL = 'http://localhost:8080/api/services';
  private requestSingleURL = 'http://localhost:8080/api/services/';
  private bookingURL = 'http://localhost:8080/api/Bookings';

  constructor(private http: HttpClient) {}

  getAllCourses(): Observable<any> {
    return this.http.get<Course[]>(this.requestAllURL);
  }

  getOneCourse(id: any): Observable<any>{
    const reqUrl = this.requestSingleURL + id.toString();
    return this.http.get<Course>(reqUrl);
  }

  // async getAllCources(): Promise<Course[]>{
  //   const data = await fetch(this.requestURL);
  //   return await data.json() ?? [];
  // }
  
  

  // filterActualCources(courses){

  // }
  bookService(serviceId: number): Observable<any> {
    const body = { serviceId: serviceId };
    return this.http.post(this.bookingURL, body);
  }

  getUserBookings(): Observable<any> {
    return this.http.get(this.bookingURL);
  }
  
  cancelBooking(bookingId: number): Observable<any> {
    return this.http.delete(`${this.bookingURL}/${bookingId}`);
  }
}