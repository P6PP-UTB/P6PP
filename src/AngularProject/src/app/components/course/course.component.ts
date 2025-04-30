import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Course } from '../../services/interfaces/course';
import { CourseService } from '../../services/course.service';
import { UserService } from '../../services/user.service';
import { ToastrService } from 'ngx-toastr';
import { RouterModule } from '@angular/router';


@Component({
  selector: 'app-course',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './course.component.html',
  styleUrls: ['./course.component.scss']
})
export class CourseComponent implements OnInit {
  @Input() course!: Course;

  trainerName: string = '';

  constructor(
    private courseService: CourseService,
    private userService: UserService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    if (this.course?.trainerId) {
      this.userService.getUserById(this.course.trainerId).subscribe({
        next: (res) => {
          this.trainerName = `${res.firstName} ${res.lastName}`;
        },
        error: () => {
          this.toastr.error('Failed to load trainer info', 'Error');
        }
      });
    }
  }

  reserveCourse(serviceId: number): void {
    this.courseService.bookService(serviceId).subscribe({
      next: () => {
        this.toastr.success('Course reserved successfully!', 'Success');
      },
      error: (err: any) => {
        const errorMessage = err.error?.message || 'An error occurred while booking the course.';
        this.toastr.error(errorMessage, 'Error');
      }
    });
  }
}