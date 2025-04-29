import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom, Observable, of } from 'rxjs';
import { Course } from './interfaces/course';

@Injectable({ providedIn: 'root' })
export class CourseService {
  private requestAllURL =
    'https://bookingservice.thankfulflower-27b66160.polandcentral.azurecontainerapps.io/api/services';
  private requestSingleURL =
    'https://bookingservice.thankfulflower-27b66160.polandcentral.azurecontainerapps.io/api/services/';

  constructor(private http: HttpClient) {}

  getAllCourses(): Observable<any> {
    return this.http.get<Course[]>(this.requestAllURL);
  }

  getOneCourse(id: any): Observable<any> {
    const reqUrl = this.requestSingleURL + id.toString();
    return this.http.get<Course>(reqUrl);
  }

  // async getAllCources(): Promise<Course[]>{{
  //   const data = await fetch(this.requestURL);
  //   return await data.json() ?? [];
  // }
  // filterActualCources(courses){
  // }
}
