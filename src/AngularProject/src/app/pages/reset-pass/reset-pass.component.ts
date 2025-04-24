import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-reset-pass',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './reset-pass.component.html',
  styleUrl: './reset-pass.component.scss'
})
export class ResetPassComponent {
  resetForm: FormGroup;
  hidePassword = true;
  hideRepeatPassword = true;
  
  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private http: HttpClient
  ) {
    this.resetForm = this.fb.group({ 
      newPassword: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern('^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$')]],

      repeatPassword: ['', [
        Validators.required]]
    });
  }

  onSubmit() {  
    if (this.resetForm.valid) {
      const form = this.resetForm.value;

      this.route.queryParams.subscribe(params => {
        const userId = params['userId'];
        const token = params['token'];
  
        const payload = {
          newPassword: form.newPassword,
          token: token,
          userId: userId
        };
        console.log(payload)

        this.http.post('http://localhost:8005/api/auth/change-password', payload)
        .subscribe({
          next: () => alert('Your password has been changed'),
          error: (err) => {
            console.error(err);
            alert('Error: ' + (err.error?.message || 'unexpected error'));
          }
        });
      });
    }
  }

}
