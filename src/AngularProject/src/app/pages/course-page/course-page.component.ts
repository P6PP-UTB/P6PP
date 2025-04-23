import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationComponent } from '../../components/navigation/navigation.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { CourseService } from '../../services/course.service';
import { UserService } from '../../services/user.service';
import { Course } from '../../services/interfaces/course';


@Component({
  selector: 'app-course-page',
  imports: [CommonModule, NavigationComponent, FooterComponent],
  templateUrl: './course-page.component.html',
  styleUrl: './course-page.component.scss'
})
export class CoursePageComponent {

  constructor(
    private courseService: CourseService,
    private userService: UserService
  ){

  }

  // course: Course = {
  //     id: 1,
  //     trainerId: 1,
  //     start: new Date(),
  //     end: new Date(),
  //     price: 100,
  //     serviceName: "Swimming",
  //     currentCapacity: 35,
  //     totalCapacity: 40,
  //     roomName: "Swimming Pool 1",
  //     isCancelled: false
  //   }

  // course: any | null = null;
  course: Course = {
    id: 99999999,
    trainerId: 99999999,
    start: new Date(),
    end: new Date(),
    price: 99999999,
    serviceName: "Testing",
    currentCapacity: 99999999,
    totalCapacity: 4999999990,
    roomName: "Testing",
    isCancelled: false
  };

  trainer: any;

  async ngOnInit() {
    const currentUrl: string = window.location.href;
    const id = this.getLastSegment(currentUrl);
  
    await this.courseService.getOneCourse(id).subscribe(response => {
      console.log('Fetched response:', response);
      this.course = response.data;
      console.log('Fetched course:', this.course);
      this.userService.getUserById(this.course.trainerId).subscribe( trainerResponse => {
        console.log("Trainer: ", trainerResponse);
        this.trainer = trainerResponse.data.user.firstName + " " + trainerResponse.data.user.lastName;
      });
    });


  }
  

  private getLastSegment(url: string): string {
    const parts = url.split("/");
    const result = parts.pop();
    return result || "1";
  }
  
}
