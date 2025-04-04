import { Component, HostListener, ViewChild, ElementRef } from '@angular/core';
import { AppComponent } from '../../app.component';
import { NavigationComponent } from "../../components/navigation/navigation.component";
import {CommonModule} from '@angular/common'
import { CalendarComponent } from "../../components/calendar/calendar.component";
import { FooterComponent } from '../../components/footer/footer.component';
@Component({
  selector: 'app-main-page',
  imports: [FooterComponent,NavigationComponent, CommonModule, CalendarComponent],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.scss'
})
export class MainPageComponent {
  @ViewChild('bgVideo') bgVideoRef!: ElementRef<HTMLVideoElement>;
  isMuted = false;
  isHidden = false;

  scrollDown() {
    const cont = document.querySelector('.scroll-container');
    if(cont){
      (cont as HTMLElement).scrollBy({
        top: window.innerHeight,
        behavior: 'smooth'
      })
    }
    window.scrollTo({ top: window.innerHeight, behavior: 'smooth' });
  }

  toggleMute() {
    const video = this.bgVideoRef.nativeElement;
    this.isMuted = !this.isMuted;
    video.muted = this.isMuted;
  }

  toggleVideo() {
    const video = this.bgVideoRef?.nativeElement;
    this.isHidden = !this.isHidden;
  
    if (this.isHidden && video) {
      video.pause();
    } else if (!this.isHidden && video) {
      video.play();
      video.muted = this.isMuted; // сохранить текущий статус звука
    }
  }
  

  onVideoEnded() {
    this.isHidden = true;
  }
}
