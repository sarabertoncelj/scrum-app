import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Sprint } from 'src/app/models/Sprint';
import { Project } from 'src/app/models/Project';
import { UserStory } from 'src/app/models/UserStory';
import { SprintService } from 'src/app/services/sprint.service';
import { AlertService } from 'src/app/services/alert.service';
import { DatePipe } from '@angular/common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SprintModalComponent } from '../sprint-modal/sprint-modal.component';

@Component({
  selector: 'sprint',
  templateUrl: './sprint.component.html',
  styleUrls: ['./sprint.component.css']
})
export class SprintComponent implements OnInit {
  @Input() sprint: Sprint;
  @Input() sprints: Sprint;
  @Input() project: Project;

  @Output() sprintRemove = new EventEmitter<Sprint>();
  @Output() sprintUpdated = new EventEmitter<Sprint>();

  constructor(private sprintService: SprintService,
              private alertService: AlertService,
              private modalService: NgbModal,
              private datePipe: DatePipe) { }

  ngOnInit (): void {
    //set reference
    this.sprint.userStories.forEach(userStory => userStory.sprint = this.sprint);
  }

  removeUserStory(userStory: UserStory) {
    this.sprint.userStories = this.sprint.userStories.filter(activeUserStory => activeUserStory.id !== userStory.id);
  }

  edit() {
    let modalRef = this.modalService.open(SprintModalComponent, {size: "dialog-centered"})
    modalRef.componentInstance.project = this.project;
    modalRef.componentInstance.sprint = this.sprint;
    modalRef.result.then((result) => {
      if(result) {
        this.sprintUpdated.emit(result);
        this.alertService.success("Updated sprint from "+ this.datePipe.transform(result.start) + " to "+ this.datePipe.transform(result.end));
      }
    }, (reason) => {
      if(reason) {
        this.alertService.error(reason);
      }
    });
  }

  delete() {
    this.sprintService.deleteSprint(this.sprint.id).subscribe(
      () => {
        this.sprintRemove.emit(this.sprint);
        this.alertService.success("Removed sprint: "+ this.datePipe.transform(this.sprint.start) + " to "+ this.datePipe.transform(this.sprint.end));
      },
      (err) => {
        this.alertService.error(err.error.message);
      }
    )
  }
}
