import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

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
  ) {
    this.resetForm = this.fb.group({ 
      password: ['', [
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
  
      const payload = {
        password: form.password
      };
    }
  }

}
