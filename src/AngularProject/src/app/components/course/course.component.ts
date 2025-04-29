import { ToastrService } from 'ngx-toastr';
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Course } from '../../services/interfaces/course';
import { CourseService } from '../../services/course.service';

@Component({
  selector: 'app-course',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './course.component.html',
  styleUrl: './course.component.scss'
})
export class CourseComponent {
  @Input() course!: Course;

  constructor(
    private courseService: CourseService,
    private toastr: ToastrService
  ) {}

  reserveCourse(serviceId: number): void {
    this.courseService.bookService(serviceId).subscribe({
      next: (response: any) => {
        // console.log('Booking successful:', response);
        this.toastr.success('Course reserved successfully!', 'Success');
      },
      error: (err: any) => {
        // console.error('Error booking course:', err);
        const errorMessage = err.error?.message || 'An error occurred while booking the course.';
        this.toastr.error(errorMessage, 'Error');
      }
    });
  }
}