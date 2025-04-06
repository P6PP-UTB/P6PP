import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './login.page.html',
  styleUrl: './login.page.scss'
})
export class LoginPage {
  loginForm: FormGroup;
  hidePassword = true;
  loginError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userService: UserService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      usernameOrEmail: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit() {
    this.loginError = null;

    if (this.loginForm.valid) {
      const payload = this.loginForm.value;
      this.authService.loginUser(payload).subscribe({
        next: (res) => {
          const token = res.data; 
          this.authService.setToken(token);
          const userId = this.userService.getUserIdFromToken();
          this.router.navigate(['']);
        },
        error: (err) => {
          if (err.status === 401 && err.error?.message) {
            this.loginError = err.error.message;
          } else {
            this.loginError = 'Something went wrong, try again later.';
          }
        }
      });
    } else {
      this.loginError = 'Please fill all fields correctly.';
    }
  }
}