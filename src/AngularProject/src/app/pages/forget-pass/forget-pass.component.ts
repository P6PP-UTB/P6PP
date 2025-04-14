import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-forget.pass',
  imports: [ReactiveFormsModule],
  templateUrl: './forget-pass.component.html',
  styleUrl: './forget-pass.component.scss'
})
export class ForgetPassComponent {
  resetForm: FormGroup;
  resetError: string | null = null;

    constructor(
      private fb: FormBuilder,
    ) {
      this.resetForm = this.fb.group({
        email: [],
      });
    }

  onSubmit() {
    this.resetError = null;

    if (this.resetForm.valid) {
      const payload = this.resetForm.value;
      console.log(payload);
    }
  }
}
