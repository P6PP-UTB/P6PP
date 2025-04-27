import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Course } from '../../services/interfaces/course';
import { CourseService } from '../../services/course.service';

@Component({
  selector: 'app-course',
  imports: [CommonModule],
  templateUrl: './course.component.html',
  styleUrl: './course.component.scss'
})
export class CourseComponent {
  @Input() course!: Course;

  constructor(private courseService: CourseService) {}

  reserveCourse(serviceId: number): void {
    this.courseService.bookService(serviceId).subscribe({
      next: (response: any) => {
        console.log('Booking successful:', response);
        alert('Course reserved successfully!');
      },
      error: (err: any) => {
        console.error('Error booking course:', err);
        const errorMessage = err.error?.message || 'An error occurred while booking the course.';
        alert(errorMessage);
      }
    });
  }
}