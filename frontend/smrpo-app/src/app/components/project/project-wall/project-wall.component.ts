import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertService } from 'src/app/services/alert.service';
import { Project } from 'src/app/models/Project';
import { ProjectService } from 'src/app/services/project.service';
import { ProjectPost } from 'src/app/models/ProjectPost';
import { ProjectPostModalComponent } from 'src/app/directives/project-post-modal/project-post-modal.component';

@Component({
  selector: 'project-wall',
  templateUrl: './project-wall.component.html',
  styleUrls: ['./project-wall.component.css']
})
export class ProjectWallComponent implements OnInit {
  project: Project;
  projectPosts: ProjectPost[];

  constructor(public authenticationService: AuthenticationService,
              private projectService: ProjectService,
              private modalService: NgbModal,
              private alertService: AlertService) {}

  ngOnInit (): void {

  }

  setProject (event) {
    this.project = event;
    this.projectService.getProjectPosts(this.project.id).subscribe(
      (projectPosts) => {
        this.projectPosts = projectPosts;
      },
      (err) => {
        this.alertService.error(err.error.message);
      }
    )
  }

  openAddPostDialog() {
    let modalRef = this.modalService.open(ProjectPostModalComponent, {size: "dialog-centered"})
    modalRef.componentInstance.project = this.project;
    modalRef.result.then((result) => {
      if(result) {
        this.projectPosts = result;
        this.alertService.success("Added new post");
      }
    }, (reason) => {
      if(reason) {
        this.alertService.error(reason);
      }
    });
  }

}
