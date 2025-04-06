import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { NavigationComponent } from '../../components/navigation/navigation.component';

@Component({
  selector: 'app-profile.page',
  standalone: true,
  imports: [CommonModule, NavigationComponent],
  templateUrl: './profile.page.html',
  styleUrl: './profile.page.scss'
})
export class ProfilePage implements OnInit {
  user: any = null;

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.userService.getCurrentUser().subscribe({
      next: (res) => {
        this.user = res?.data?.user;
      },
      error: (err) => {
      }
    });
  }
} 