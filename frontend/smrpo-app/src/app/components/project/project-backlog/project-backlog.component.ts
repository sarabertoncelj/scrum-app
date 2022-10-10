import { Component, OnInit } from '@angular/core';
import { Project } from 'src/app/models/Project';
import { UserStory } from 'src/app/models/UserStory';
import { ProjectService } from 'src/app/services/project.service';
import { UserStoryStatus } from 'src/app/enums/UserStoryStatus';
import { UserStoryModalComponent } from 'src/app/directives/user-story-modal/user-story-modal.component';
import {NgbModal, ModalDismissReasons} from '@ng-bootstrap/ng-bootstrap';
import { Sprint } from 'src/app/models/Sprint';
import { AlertService } from 'src/app/services/alert.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserStoryPriority } from 'src/app/enums/UserStoryPriority';

@Component({
  selector: 'project-backlog',
  templateUrl: './project-backlog.component.html',
  styleUrls: ['./project-backlog.component.css']
})
export class ProjectBacklogComponent implements OnInit {
  project: Project;
  sprints: Sprint[];
  finishedUserStories: UserStory[] = [];
  unfinishedUserStories: UserStory[] = [];

  constructor(public authenticationService: AuthenticationService,
              private projectService:ProjectService,
              private modalService: NgbModal,
              private alertService: AlertService) {
  }
  
  ngOnInit(): void {
  }

  setProject(event) {
    this.project = event;
    this.projectService.getUserStories(this.project.id).subscribe(
      res => {
        let userStories: UserStory[] = res.filter(userStory => userStory.priority !== UserStoryPriority["Won't have this time"]);;
        for (let userStory of userStories) {
          if(userStory.status == UserStoryStatus.Unfinished) {
            this.unfinishedUserStories.push(userStory);
          }
          if(userStory.status == UserStoryStatus.Finished) {
            this.finishedUserStories.push(userStory);
          }
        }
      },
      err => {
        this.alertService.error(err.error.message);
      }
    )
  }

  setSprints(event) {
    this.sprints = event;
  }

  openAddUserStoryDialog() {
    let modalRef = this.modalService.open(UserStoryModalComponent, {size: "dialog-centered"})
    modalRef.componentInstance.project = this.project;
    modalRef.result.then((result) => {
      if(result) {
        this.unfinishedUserStories.push(result);
        this.alertService.success("Added new user story: "+ result['name']);
      }
    }, (reason) => {
      if(reason) {
        this.alertService.error(reason);
      }
    });
  }

  moveUserStory(userStory: UserStory) {
    if(userStory.status === UserStoryStatus.Finished) {
      this.unfinishedUserStories = this.unfinishedUserStories.filter(unfinishedUserStory => unfinishedUserStory.id !== userStory.id);
      this.finishedUserStories.push(userStory);
    }
  }
}
