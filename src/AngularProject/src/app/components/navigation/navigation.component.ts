import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router'; // Добавили Router
import { ToastrService } from 'ngx-toastr'; 
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navigation',
  imports: [RouterModule, MatIconModule, CommonModule],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.scss'
})
export class NavigationComponent {
  isMenuOpen = false;
  isLoggedIn = false;
  MoneyBalance = 123;
  constructor(
    private authService: AuthService,
    private toastr: ToastrService,
    private router: Router // инжектим роутер
  ) {}

  ngOnInit() {
    this.authService.isLoggedIn$.subscribe(status => {
      this.isLoggedIn = status;
    });
  }

  logout() {
    this.authService.logout();
    this.toastr.success('You have successfully logged out', 'Goodbye!');
    this.router.navigate(['/']); // тут редирект после логаута
  }
}
