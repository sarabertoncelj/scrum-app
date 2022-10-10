import { JwtInterceptor } from './jwt.interceptor';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { OwlDateTimeModule, OwlNativeDateTimeModule, OWL_DATE_TIME_FORMATS } from 'ng-pick-datetime';


import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { RouterModule, Routes, OutletContext } from '@angular/router';
import { ProjectNavigationComponent } from './directives/project-navigation/project-navigation.component';
import { ErrorComponent } from './components/error/error.component';
import { ProjectBacklogComponent } from './components/project/project-backlog/project-backlog.component';
import { ProjectsComponent } from './components/projects/projects.component';
import { ProjectModalComponent } from './directives/project-modal/project-modal.component';
import { UserStoryComponent } from './directives/user-story/user-story.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { UserStoryModalComponent } from './directives/user-story-modal/user-story-modal.component';
import { AdminGuard } from './guards/admin-guard.service';
import { AuthGuard } from './guards/auth-guard.service';
import { SprintsComponent } from './components/project/sprints/sprints.component';
import { SprintComponent } from './directives/sprint/sprint.component';
import { NoAuthGuard } from './guards/no-auth-guard.service';
import { AlertComponent } from './directives/alert/alert.component';
import { UsersComponent } from './components/users/users.component';
import { UserModalComponent } from './directives/user-modal/user-modal.component';
import { SprintModalComponent } from './directives/sprint-modal/sprint-modal.component';
import { DatePipe } from '@angular/common';
import { SprintBacklogComponent } from './components/project/sprint-backlog/sprint-backlog.component';
import { UserStoryTasksComponent } from './directives/user-story-tasks/user-story-tasks.component';
import { UserStoryTaskModalComponent } from './directives/user-story-task-modal/user-story-task-modal.component';
import { RejectUserStoryModalComponent } from './directives/reject-user-story-modal/reject-user-story-modal.component';
import { FutureReleasesComponent } from './components/project/future-releases/future-releases.component';
import { ProjectWallComponent } from './components/project/project-wall/project-wall.component';
import { ProjectPostComponent } from './directives/project-post/project-post.component';
import { ProjectPostModalComponent } from './directives/project-post-modal/project-post-modal.component';
import { WorkLogsModalComponent } from './directives/work-logs-modal/work-logs-modal.component';
import { WorkLogComponent } from './directives/work-log/work-log.component';
import { ActiveTaskComponent } from './directives/active-task/active-task.component';

const appRoutes: Routes = [
  {
    path: '',
    redirectTo: 'projects',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [NoAuthGuard]
  },
  {
    path: 'projects',
    component: ProjectsComponent
  },
  {
    path: 'users',
    component: UsersComponent,
    canActivate: [AuthGuard, AdminGuard]
  },
  {
    path: 'project/:id/project_wall',
    component: ProjectWallComponent,
  },
  {
    path: 'project/:id/product_backlog',
    component: ProjectBacklogComponent,
  },
  {
    path: 'project/:id/sprint_backlog',
    component: SprintBacklogComponent,
  },
  {
    path: 'project/:id/sprints',
    component: SprintsComponent,
    data: { title: 'Sprints' },
  },
  {
    path: 'project/:id/future_releases',
    component: FutureReleasesComponent,
  },
  {
    path: '**',
    component: ErrorComponent
  },
];

export const MY_NATIVE_FORMATS = {
  fullPickerInput: { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: 'numeric' },
  datePickerInput: { year: 'numeric', month: 'short', day: 'numeric' },
  timePickerInput: { hour: 'numeric', minute: 'numeric' },
  monthYearLabel: { year: 'numeric', month: 'short' },
  dateA11yLabel: { year: 'numeric', month: 'long', day: 'numeric' },
  monthYearA11yLabel: { year: 'numeric', month: 'long' },
};

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    ProjectNavigationComponent,
    ErrorComponent,
    ProjectBacklogComponent,
    ProjectsComponent,
    UserStoryComponent,
    UserStoryModalComponent,
    ProjectModalComponent,
    UserStoryComponent,
    SprintsComponent,
    SprintComponent,
    AlertComponent,
    UsersComponent,
    UserModalComponent,
    SprintModalComponent,
    SprintBacklogComponent,
    UserStoryTasksComponent,
    UserStoryTaskModalComponent,
    RejectUserStoryModalComponent,
    FutureReleasesComponent,
    ProjectWallComponent,
    ProjectPostComponent,
    ProjectPostModalComponent,
    WorkLogsModalComponent,
    WorkLogComponent,
    ActiveTaskComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    NgbModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    RouterModule.forRoot(
      appRoutes,
      { enableTracing: false } // <-- debugging purposes only
    ),
    ReactiveFormsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: OWL_DATE_TIME_FORMATS, useValue: MY_NATIVE_FORMATS },
    AuthGuard,
    AdminGuard,
    NoAuthGuard,
    DatePipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
