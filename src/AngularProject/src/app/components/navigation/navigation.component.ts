import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr'; 
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
@Component({
  selector: 'app-navigation',
  imports: [RouterModule, 
  MatIconModule, CommonModule],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.scss'
})
export class NavigationComponent {
  isMenuOpen = false;
  isLoggedIn = false;

  constructor(private authService: AuthService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.authService.isLoggedIn$.subscribe(status => {
      this.isLoggedIn = status;
    });
  }

  // toggleMenu() {
  //   this.isMenuOpen = !this.isMenuOpen;
  // }

  // onMenuClosed() {
  //   this.isMenuOpen = false;
  // }
  logout() {
    this.authService.logout();
    this.toastr.success('You have successfully logged out', 'Goodbye!');
  }
}
