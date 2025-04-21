import { Component, HostListener, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common'
import { AppComponent } from '../../app.component';
import { NavigationComponent } from "../../components/navigation/navigation.component";
import { CalendarComponent } from "../../components/calendar/calendar.component";
import { FooterComponent } from '../../components/footer/footer.component';
import { CourseComponent } from '../../components/course/course.component';

import { Course } from '../../services/interfaces/course';

import { CourseService } from '../../services/course.service';


@Component({
  selector: 'app-main-page',
  imports: [FooterComponent,NavigationComponent, CommonModule, CalendarComponent, CourseComponent],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.scss'
})
export class MainPageComponent {
  @ViewChild('bgVideo') bgVideoRef!: ElementRef<HTMLVideoElement>;
  isMuted = false;
  isHidden = false;

  constructor(
    private courseService: CourseService,
  ){

  }
  
  // courses: Course[] = [];

  // TESTING DATA !!!COMMENT 31:0!!!
  courses: Course[] = [
    {
      id: 1,
      start: new Date(),
      end: new Date(),
      price: 100,
      serviceName: "Swimming",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Swimming Pool 1",
      isCancelled: false
    },
    {
      id: 2,
      start: new Date(),
      end: new Date(),
      price: 200,
      serviceName: "Yoga",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Room 1",
      isCancelled: false
    },
    {
      id: 3,
      start: new Date(),
      end: new Date(),
      price: 250,
      serviceName: "Boxing",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Room 3",
      isCancelled: false
    },
    {
      id: 4,
      start: new Date(),
      end: new Date(),
      price: 50,
      serviceName: "Group training with Ronnie Coleman",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Room 6",
      isCancelled: false
    },
    {
      id: 5,
      start: new Date(),
      end: new Date(),
      price: 150,
      serviceName: "Powerlifting",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Room 2",
      isCancelled: false
    },
  ];

  // ngOnInit(){
  //   this.courseService.getAllCourses().subscribe(courses => {
  //     console.log(courses)
  //     this.courses = courses;
  //   });
  // }

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
