import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
// bootstrap import
import "bootstrap-icons/font/bootstrap-icons.css";


bootstrapApplication(AppComponent, { 
  providers:[
    appConfig.providers,
    provideAnimations(),
    provideToastr()
  ]  
})
  .catch((err) => console.error(err));
