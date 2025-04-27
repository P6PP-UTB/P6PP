import { Component, HostListener, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent {

  @Output() closed = new EventEmitter<void>();

  notifications = [
    { message: 'Your booking has been cancelled' },
    { message: 'New course available!' },
    { message: 'Payment was successful' }
  ];

  closeNotifications() {
    this.closed.emit();
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: MouseEvent) {
    const notificationsElement = document.getElementById('notifications-container');
    const notificationButton = document.getElementById('notification-button');

    if (
      notificationsElement &&
      !notificationsElement.contains(event.target as Node) &&
      notificationButton &&
      !notificationButton.contains(event.target as Node)
    ) {
      this.closeNotifications(); 
    }
  }
}
