import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-verification',
  imports: [CommonModule],
  templateUrl: './verification.component.html',
  styleUrl: './verification.component.scss'
})
export class VerificationComponent {
  verified: boolean = false;
  btn: boolean = true;
  bad: boolean = false;
  error: string = "";
  
  constructor(
    private route: ActivatedRoute,
    private http: HttpClient
  ){}

  submit() {
    console.log("submit");

    //default conditions
    this.verified = false;
    this.btn = true;
    this.bad = false;
    this.error = "";

      this.route.queryParams.subscribe(params => {
        const userId = params['userId'];
        const token = params['token'];
        console.log(params);

        const baseUrl = 'http://localhost:8005/api/auth/verify-email';
        const url = `${baseUrl}/${encodeURIComponent(userId)}/${encodeURIComponent(token)}`;
        console.log("link: ", url);

        this.http.post(url, null).subscribe({
          next: () => {
            this.verified = true;
            this.btn = false;
          },
          error: (err) => {
            console.error(err);
            this.bad = true;
            this.error = 'Error: ' + (err.error?.message || 'unexpected error');
          }
        });
      });
    }
}
