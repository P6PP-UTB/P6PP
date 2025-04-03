import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-header',
  imports: [FormsModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  currentUrl: string = window.location.href;

  @Input() pageName = this.getLastSegment(this.currentUrl);

  private getLastSegment(url: string): string {
    const parts = url.split("/");
    const result = parts.pop() || "Main page";
    if (result == "login" || result == "signup") return "REZERVACE+"
    return result.charAt(0).toUpperCase() + result.slice(1);;
}
}
