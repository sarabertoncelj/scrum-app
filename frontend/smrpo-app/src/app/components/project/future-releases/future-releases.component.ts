import { Component, OnInit } from '@angular/core';
import { Project } from 'src/app/models/Project';
import { Sprint } from 'src/app/models/Sprint';
import { UserStory } from 'src/app/models/UserStory';
import { SprintService } from 'src/app/services/sprint.service';
import { AlertService } from 'src/app/services/alert.service';
import { ProjectService } from 'src/app/services/project.service';
import { UserStoryStatus } from 'src/app/enums/UserStoryStatus';
import { UserStoryPriority } from 'src/app/enums/UserStoryPriority';

@Component({
  selector: 'app-future-releases',
  templateUrl: './future-releases.component.html',
  styleUrls: ['./future-releases.component.css']
})
export class FutureReleasesComponent implements OnInit {
  project: Project;
  sprints: Sprint[];

  wontHaveThisTimeUserStories: UserStory[];

  constructor(private projectService: ProjectService,
              private alertService: AlertService
  ) {}

  ngOnInit(): void {

  }

  setSprints(event) {
    this.sprints = event;
  }

  setProject(event) {
    this.project = event;
    this.projectService.getUserStories(this.project.id).subscribe(
      res => {
        this.wontHaveThisTimeUserStories = res.filter(userStory => userStory.status == UserStoryStatus.Unfinished && userStory.priority === UserStoryPriority["Won't have this time"]);
      },
      err => {
        this.alertService.error(err.error.message);
      }
    )
  }
}
