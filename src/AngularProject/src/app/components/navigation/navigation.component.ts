import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router'; 
import { ToastrService } from 'ngx-toastr'; 
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { NotificationsComponent } from '../notifications/notifications.component';

@Component({
  selector: 'app-navigation',
  imports: [RouterModule, MatIconModule, CommonModule, NotificationsComponent],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.scss'
})
export class NavigationComponent {
  isMenuOpen = false;
  isLoggedIn = false;
  MoneyBalance = 123;
  userId: number | null | undefined;

  constructor(
    private authService: AuthService,
    private toastr: ToastrService,
    private router: Router 
  ) {}

  ngOnInit() {
    this.authService.isLoggedIn$.subscribe(status => {
      this.isLoggedIn = status;

      this.userId = this.authService.getUserId()

      if(this.userId != null || this.userId != 0){
        
      }
    });
  }

  logout() {
    this.authService.logout();
    this.toastr.success('You have successfully logged out', 'Goodbye!');
    this.router.navigate(['/']); 
  }

  showNotifications = false;

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
  }
}
