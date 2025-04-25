import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent {
  today = new Date();
  currentDate = new Date();
  month: number = this.today.getMonth();
  year: number = this.today.getFullYear();

  weekdays = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
  daysWithPadding: (number | null)[] = [];

  eventsToday: { title: string, time: string, color: string }[] = [];
  eventsTomorrow: { title: string, time: string, color: string }[] = [];

  constructor() {
    this.generateCalendar();
    const [todayEvents, tomorrowEvents] = this.getEvents();
    this.eventsToday = todayEvents;
    this.eventsTomorrow = tomorrowEvents;
  }

  getEvents() {
    const eventsToday = [
      { title: 'Ronnie Coleman', time: '08:00', color: 'green' },
      { title: 'KickBoxing', time: '09:00', color: 'red' },
      { title: 'Yoga', time: '10:00', color: 'orange' },
    ];
    const eventsTomorrow = [
      { title: 'Hobby Horsing', time: '13:00', color: 'seagreen' },
      { title: 'Chess', time: '14:00', color: 'purple' },
    ];
    return [eventsToday, eventsTomorrow];
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
}
