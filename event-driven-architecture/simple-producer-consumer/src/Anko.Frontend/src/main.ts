import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

// Entry point for the application
// AppComponent is the root component of the application
// appConfig is the configuration for the application

// Different to usual components, for AppComponent, providers are not passed directly
// in @Component decorator, but through appConfig
bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
