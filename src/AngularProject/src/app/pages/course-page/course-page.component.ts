import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationComponent } from '../../components/navigation/navigation.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { CourseService } from '../../services/course.service';
import { UserService } from '../../services/user.service';
import { Course } from '../../services/interfaces/course';
import { ToastrService } from 'ngx-toastr'; 

@Component({
  selector: 'app-course-page',
  standalone: true,
  imports: [CommonModule, NavigationComponent, FooterComponent],
  templateUrl: './course-page.component.html',
  styleUrls: ['./course-page.component.scss']
})
export class CoursePageComponent {
  constructor(
    private courseService: CourseService,
    private userService: UserService,
    private toastr: ToastrService 
  ) {}

  course!: Course;
  trainer: string = '';
  bookingId: number | null = null; // ЕСЛИ null — значит записи нет

  async ngOnInit() {
    const currentUrl: string = window.location.href;
    const id = this.getLastSegment(currentUrl);
  
    this.courseService.getOneCourse(id).subscribe(response => {
      this.course = response.data;
      this.userService.getUserById(this.course.trainerId).subscribe(trainerResponse => {
        console.log("Trainer response: ", trainerResponse);
        this.trainer = trainerResponse.firstName + ' ' + trainerResponse.lastName;
      });
  
      // СЮДА ПЕРЕНОСИМ ПОСЛЕ ТОГО КАК КУРС ПОЛУЧИЛИ!!!
      this.checkIfBooked(this.course.id);
    });
  }

  // Проверяем, забронирован ли курс
  checkIfBooked(courseId: number) {
    this.courseService.getUserBookings().subscribe(response => {
      const bookings = response.data || [];
      const existingBooking = bookings.find((b: any) => b.serviceId === courseId);

      if (existingBooking) {
        this.bookingId = existingBooking.id;
      } else {
        this.bookingId = null;
      }
    });
  }

  enrollOrCancel() {
    if (this.bookingId) {
      // Отмена записи
      this.courseService.cancelBooking(this.bookingId).subscribe({
        next: () => {
          this.toastr.success('Booking cancelled successfully!', 'Success');
          this.bookingId = null;
        },
        error: (err) => {
          this.toastr.error(err.error?.message || 'Failed to cancel booking.', 'Error');
        }
      });
    } else {
      // Запись на курс
      this.courseService.bookService(this.course.id).subscribe({
        next: (response) => {
          this.toastr.success('Enrolled successfully!', 'Success');
          this.checkIfBooked(this.course.id); // Чекнем бронь снова
        },
        error: (err) => {
          this.toastr.error(err.error?.message || 'Failed to enroll.', 'Error');
        }
      });
    }
  }


  // course: Course = {
  //   id: 99999999,
  //   trainerId: 99999999,
  //   start: new Date(),
  //   end: new Date(),
  //   price: 99999999,
  //   serviceName: "ServiceName",
  //   currentCapacity: 99999999,
  //   totalCapacity: 4999999990,
  //   roomName: "RoomName",
  //   isCancelled: false
  // };

  // trainer: string = '';

  // Массив изображений
  imageUrls: string[] = [
    'https://goo.su/kSY4URo?',
    'https://goo.su/YyOz7YE',
    'https://tse3.mm.bing.net/th?id=OIG2.5bowY.5mQdQKTIv5qY1g&pid=ImgGn'
  ];
  currentIndex = 0;

  // Геттер для текущего изображения
  get currentImage(): string {
    return this.imageUrls[this.currentIndex];
  }

  // Метод для перехода к предыдущему изображению
  prevImage(): void {
    this.currentIndex = (this.currentIndex - 1 + this.imageUrls.length) % this.imageUrls.length;
  }

  // Метод для перехода к следующему изображению
  nextImage(): void {
    this.currentIndex = (this.currentIndex + 1) % this.imageUrls.length;
  }

  // Функция для получения последнего сегмента URL
  private getLastSegment(url: string): string {
    const parts = url.split('/');
    return parts.pop() || '1';
  }
}