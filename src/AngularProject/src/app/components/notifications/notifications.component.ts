import { Component, HostListener, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  
  constructor(private notificationService: NotificationService) {
  }

  public ngOnInit(): void {
    this.notificationService.notifications$.subscribe(data => {
      console.log(data);
      this.notifications = data;
    })

    console.log(this.notifications);
  }

  @Output() closed = new EventEmitter<void>();

  public notifications: any;

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
