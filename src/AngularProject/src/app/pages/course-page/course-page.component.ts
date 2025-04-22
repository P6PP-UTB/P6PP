import { Component } from '@angular/core';
import { NavigationComponent } from '../../components/navigation/navigation.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { CourseService } from '../../services/course.service';
import { Course } from '../../services/interfaces/course';


@Component({
  selector: 'app-course-page',
  imports: [NavigationComponent, FooterComponent],
  templateUrl: './course-page.component.html',
  styleUrl: './course-page.component.scss'
})
export class CoursePageComponent {

  constructor(
    private courseService: CourseService,
  ){

  }

  cource: Course = {
      id: 1,
      trainerId: 1,
      start: new Date(),
      end: new Date(),
      price: 100,
      serviceName: "Swimming",
      currentCapacity: 35,
      totalCapacity: 40,
      roomName: "Swimming Pool 1",
      isCancelled: false
    }

  ngOnInit(){
    // const currentUrl: string = window.location.href;
    // const id = this.getLastSegment(currentUrl);

    // this.courseService.getOneCourse(id).subscribe(course => {
    //   console.log('Fetched course:', course);
    // });
    console.log('Fetched course:', this.cource);
  }

  private getLastSegment(url: string): string {
    const parts = url.split("/");
    const result = parts.pop();
    return result || "1";
  }
  
}
