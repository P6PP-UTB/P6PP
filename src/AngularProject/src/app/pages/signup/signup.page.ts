import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { NavigationComponent } from '../../components/navigation/navigation.component';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [RouterModule, ReactiveFormsModule, CommonModule, NavigationComponent],
  templateUrl: './signup.page.html',
  styleUrl: './signup.page.scss'
})
export class SignupPage {
  signupForm: FormGroup;
  hidePassword = true;
  hideRepeatPassword = true;
  registrationError: string | null = null;
  showVerifyMessage: boolean = false;


  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.signupForm = this.fb.group({
      name: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.pattern('^[a-zA-Z]{2,}$')]],

      surname: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.pattern('^[a-zA-Z ]{2,}$')]],

      username: ['', [
        Validators.required, 
        Validators.minLength(2),
        Validators.pattern('^(?![.-])[a-zA-Z0-9_-]{2,}(?<![.-])$')]],

      email: ['', [
        Validators.required, 
        Validators.email]],

      password: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern('^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$')]],

      repeatPassword: ['', [
        Validators.required]]
    });
  }

  onSubmit() {
    this.registrationError = null;
  
    if (this.signupForm.valid) {
      const form = this.signupForm.value;
  
      const payload = {
        userName: form.username,
        email: form.email,
        password: form.password,
        firstName: form.name,
        lastName: form.surname
      };
  
      this.authService.registerUser(payload).subscribe({
        next: () => {
          this.showVerifyMessage = true;
          //this.signupForm.reset();
          //this.router.navigate(['/login']); // или куда надо
        },
        error: (err) => {
          if (err.status === 400 && err.error?.message) {
            this.registrationError = err.error.message;
          } else {
            this.registrationError = 'Something went wrong, try again later.';
          }
        }
      });
  
    } else {
      console.warn('Invalid:', this.signupForm.value);
    }
  }
}