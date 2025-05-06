import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CourseService } from '../../services/course.service';
import { Course } from '../../services/interfaces/course';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit {
  today = new Date();
  currentDate = new Date();
  month: number = this.today.getMonth();
  year: number = this.today.getFullYear();

  weekdays = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
  daysWithPadding: (number | null)[] = [];

  eventsToday: { title: string, time: string, color: string }[] = [];
  eventsTomorrow: { title: string, time: string, color: string }[] = [];
  eventsByDate: { [date: string]: { title: string, time: string, color: string }[] } = {};

  showMonthMenu: boolean = false;
  months: string[] = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];

  constructor(private courseService: CourseService) {
    this.generateCalendar();
  }

  ngOnInit(): void {
    this.courseService.getUserBookings().subscribe((response) => {
      const bookings = response.data || [];
  
      const loadedCourses: Course[] = [];
      let completed = 0;
  
      if (bookings.length === 0) return;
  
      bookings.forEach((booking: any) => {
        this.courseService.getOneCourse(booking.serviceId.toString()).subscribe((res) => {
          loadedCourses.push(res.data);
          completed++;
  
          if (completed === bookings.length) {
            // все курсы загружены
            this.populateCalendarEvents(loadedCourses);
          }
        });
      });
    });
  }
  toggleMonthMenu() {
    this.showMonthMenu = !this.showMonthMenu;
  }

  selectMonth(selectedMonth: number) {
    this.month = selectedMonth;
    this.showMonthMenu = false;
    this.generateCalendar();
  }

  generateCalendar() {
    const firstDay = new Date(this.year, this.month, 1);
    const lastDay = new Date(this.year, this.month + 1, 0);
    const daysInMonth = lastDay.getDate();

    const startDay = (firstDay.getDay() + 6) % 7; // Monday = 0
    const days: (number | null)[] = Array(startDay).fill(null);

    for (let i = 1; i <= daysInMonth; i++) {
      days.push(i);
    }

    while (days.length % 7 !== 0) {
      days.push(null);
    }

    this.daysWithPadding = days;
  }

  changeMonth(offset: number) {
    this.month += offset;
    if (this.month > 11) {
      this.month = 0;
      this.year++;
    } else if (this.month < 0) {
      this.month = 11;
      this.year--;
    }
    this.generateCalendar();
  }

  getMonthName(): string {
    return new Date(this.year, this.month).toLocaleString('default', { month: 'long' });
  }

  isToday(day: number | null): boolean {
    if (day == null) return false;
    return (
      day === this.today.getDate() &&
      this.month === this.today.getMonth() &&
      this.year === this.today.getFullYear()
    );
  }

  getEventsForDate(year: number, month: number, day: number) {
    const pad = (n: number) => n.toString().padStart(2, '0');
    const dateStr = `${year}-${pad(month + 1)}-${pad(day)}`;
    return this.eventsByDate[dateStr] || [];
  }

  populateCalendarEvents(bookings: Course[]) {
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(today.getDate() + 1);
  
    const formatLocalDate = (d: Date) => {
      const year = d.getFullYear();
      const month = (d.getMonth() + 1).toString().padStart(2, '0');
      const day = d.getDate().toString().padStart(2, '0');
      return `${year}-${month}-${day}`;
    };
  
    const todayStr = formatLocalDate(today);
    const tomorrowStr = formatLocalDate(tomorrow);
  
    this.eventsByDate = {};
  
    bookings.forEach(course => {
      const courseDateObj = new Date(course.start);
      const dateStr = formatLocalDate(courseDateObj);
      const timeStr = courseDateObj.toTimeString().slice(0, 5); // HH:mm
  
      const event = {
        title: course.serviceName,
        time: timeStr,
        color: this.getDeterministicColor(course.serviceName)
      };
  
      if (!this.eventsByDate[dateStr]) {
        this.eventsByDate[dateStr] = [];
      }
  
      this.eventsByDate[dateStr].push(event);
    });
  
    this.eventsToday = this.eventsByDate[todayStr] || [];
    this.eventsTomorrow = this.eventsByDate[tomorrowStr] || [];
  }


  getDeterministicColor(courseName: string): string {
    const colors = ['#EA2839', '#EC741B', '#6EBEA0', '#46505A','#F3D196','#6C9753','#DBA3C1','#9C8CDE','#9B4648'];
  
    // Простейший хеш-функция (как сид)
    let hash = 0;
    for (let i = 0; i < courseName.length; i++) {
      hash = courseName.charCodeAt(i) + ((hash << 5) - hash);
    }
  
    const index = Math.abs(hash) % colors.length;
    return colors[index];
  }
  
  
}
