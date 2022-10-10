import { Component, OnInit } from '@angular/core';
import { Sprint } from 'src/app/models/Sprint';
import { Project } from 'src/app/models/Project';
import { UserStory } from 'src/app/models/UserStory';
import { SprintService } from 'src/app/services/sprint.service';
import { AlertService } from 'src/app/services/alert.service';
import { UserStoryService } from 'src/app/services/user-story.service';

@Component({
  selector: 'app-sprint-backlog',
  templateUrl: './sprint-backlog.component.html',
  styleUrls: ['./sprint-backlog.component.css']
})
export class SprintBacklogComponent implements OnInit {

  project: Project;
  sprints: Sprint[];

  activeUserStories: UserStory[];

  constructor(private sprintService: SprintService,
              private alertService: AlertService
  ) {}

  ngOnInit(): void {

  }

  setProject(event) {
    this.project = event;
  }

  setSprints(event) {
    this.sprints = event;
    let activeSprint = this.sprints.find(x => x.active === true);
    if(activeSprint) {
      this.sprintService.getSprintUserStories(activeSprint.id).subscribe(
        res => {
          this.activeUserStories = res;
        },
        err => {
          this.alertService.error(err.error.message);
        }
      )
    }
  }

  removeUserStory(userStory: UserStory) {
    this.activeUserStories = this.activeUserStories.filter(activeUserStory => activeUserStory.id !== userStory.id);
  }
}
