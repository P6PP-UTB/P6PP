import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom, Observable, of } from 'rxjs';
import { Course } from './interfaces/course';

@Injectable({ providedIn: 'root' })
export class CourseService {
  private requestURL = 'http://localhost:8080/api/services';

  constructor(private http: HttpClient) {}

  getAllCourses(): Observable<Course[]> {
    return this.http.get<Course[]>(this.requestURL);
  }
  
  

  // filterActualCources(courses){

  // }
}