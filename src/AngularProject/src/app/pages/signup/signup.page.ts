import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
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
    private authService: AuthService
  ) {
    this.signupForm = this.fb.group({
      name: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.pattern(/^[\p{L}\p{M}]{2,}$/u)
      ]],     

      surname: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.pattern(/^[\p{L}\p{M}]{2,}$/u)]],

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
    console.log("Signing up...")
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

      console.log("Payload: ", payload);
  
      this.authService.registerUser(payload).subscribe({
        next: () => {
          console.log("Success. Wait couple of minutes and check email")
          this.showVerifyMessage = true;
        },
        error: (err) => {
          console.log("Error occured")
          if (err.status === 400 && err.error?.message) {
            console.log("ERROR: ", err.error.message);
            this.registrationError = err.error.message;
          } else {
            console.log("UNEXPECTED ERROR: ", err.error.message);
            this.registrationError = 'Something went wrong, try again later.';
          }
        }
      });
  
    } else {
      console.warn('Invalid form:', this.signupForm.value);
      this.registrationError = "Check all fields for errors"
    }
  }
}