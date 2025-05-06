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
  isBooked: boolean = false;
  bookingId: number | null = null;

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

    this.checkIfBooked(this.course.id);
  }

  checkIfBooked(courseId: number) {
    this.courseService.getUserBookings().subscribe({
      next: (res) => {
        const bookings = res.data || [];
        const existingBooking = bookings.find(
          (b: any) => b.serviceId === courseId
        );
        if (existingBooking) {
          this.isBooked = true;
          this.bookingId = existingBooking.id;
        } else {
          this.isBooked = false;
          this.bookingId = null;
        }
      }
    });
  }

  toggleReservation(): void {
    if (this.isBooked && this.bookingId) {
      this.courseService.cancelBooking(this.bookingId).subscribe({
        next: () => {
          this.toastr.info('Reservation cancelled.', 'Cancelled');
          this.isBooked = false;
          this.bookingId = null;
          this.courseService.notifyRefreshBookings();
        },
        error: () => {
          this.toastr.error('Failed to cancel reservation.', 'Error');
        }
      });
    } else {
      this.courseService.bookService(this.course.id).subscribe({
        next: () => {
          this.toastr.success('Course reserved successfully!', 'Success');
          this.checkIfBooked(this.course.id); // обновим состояние
          this.courseService.notifyRefreshBookings();
        },
        error: (err: any) => {
          const errorMessage = err.error?.message || 'An error occurred while booking the course.';
          this.toastr.error(errorMessage, 'Error');
        }
      });
    }
  }
}
