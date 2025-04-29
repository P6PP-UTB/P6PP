import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-verification',
  imports: [CommonModule],
  templateUrl: './verification.component.html',
  styleUrl: './verification.component.scss',
})
export class VerificationComponent {
  verified: boolean = false;

  constructor(private route: ActivatedRoute, private http: HttpClient) {}

  submit() {
    console.log('submit');
    this.route.queryParams.subscribe((params) => {
      const userId = params['userId'];
      const token = params['token'];
      console.log(params);

      const baseUrl =
        'https://authservice.thankfulflower-27b66160.polandcentral.azurecontainerapps.io/api/auth/verify-email';
      const url = `${baseUrl}/${encodeURIComponent(
        userId
      )}/${encodeURIComponent(token)}`;
      console.log('link: ', url);

      this.http.post(url, null).subscribe({
        next: () => {
          alert('Email is verified'), (this.verified = true);
        },
        error: (err) => {
          console.error(err);
          alert('Error: ' + (err.error?.message || 'unexpected error'));
        },
      });
    });
  }
}
