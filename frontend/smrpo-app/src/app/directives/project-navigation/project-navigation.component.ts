import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Project } from 'src/app/models/Project';
import { ProjectService } from 'src/app/services/project.service';
import { first } from 'rxjs/operators';
import { User } from 'src/app/models/User';
import { ProjectRole } from 'src/app/enums/ProjectRole';
import { Sprint } from 'src/app/models/Sprint';
import { AlertService } from 'src/app/services/alert.service';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'project-navigation',
  templateUrl: './project-navigation.component.html',
  styleUrls: ['./project-navigation.component.css']
})
export class ProjectNavigationComponent implements OnInit {
  project: Project;
  sprints: Sprint[];
  page: String;
  user: User;

  @Output() projectEvent = new EventEmitter();
  @Output() sprintsEvent = new EventEmitter();

  constructor(
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private router: Router,
    private authenticationService: AuthenticationService,
    private alertService: AlertService
  ) {
    const url = route.snapshot.url.slice(-1)[0].toString();
    switch (url) {
      case "project_wall": {
        this.page = "project_wall";
        break;
      }
      case "product_backlog": {
        this.page = "product_backlog";
        break;
      }
      case "sprint_backlog": {
        this.page = "sprint_backlog";
        break;
      }
      case "sprints": {
        this.page = "sprints";
        break;
      }
      case "future_releases": {
        this.page = "future_releases";
        break;
      }
      default: {
        this.page = "product_backlog";
      }
    }
  }

  ngOnInit (): void {
    this.route.params.subscribe(params => {
      let id: string = params['id'];
      this.projectService.getProject(id).pipe(first()).subscribe(
        res => {
          this.project = res          
          this.projectEvent.emit(this.project);
          this.authenticationService.setProjectRoles(this.project.projectRoles);
          this.projectService.getSprints(this.project.id).subscribe(
            sprints => {
              this.sprints = sprints;
              this.sprintsEvent.emit(this.sprints);
            },
            err => {
              this.alertService.error(err.error.message);
            }
          )
        },
        err => {
          this.alertService.error(err.error.message);
        }
      );
    });
  }

}
