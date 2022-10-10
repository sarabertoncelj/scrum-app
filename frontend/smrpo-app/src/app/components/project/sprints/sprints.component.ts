import { Component, OnInit } from '@angular/core';
import { Project } from 'src/app/models/Project';
import { Sprint } from 'src/app/models/Sprint';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SprintModalComponent } from 'src/app/directives/sprint-modal/sprint-modal.component';
import { ProjectRole } from 'src/app/enums/ProjectRole';
import { AlertService } from 'src/app/services/alert.service';
import { DatePipe } from '@angular/common';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'app-sprints',
  templateUrl: './sprints.component.html',
  styleUrls: ['./sprints.component.css']
})
export class SprintsComponent implements OnInit {
  project: Project;
  sprints: Sprint[] = [];
  error: String;

  constructor(public authenticationService: AuthenticationService,
              private modalService: NgbModal,
              private alertService: AlertService,
              private datePipe: DatePipe) { }

  ngOnInit(): void {
  }

  setProject(event) {
    this.project = event;
  }

  setSprints(event) {
    this.sprints = event;
  }
  
  sprintRemove(event) {
    this.sprints = this.sprints.filter(sprint => sprint.id !== event.id);
  }

  sprintUpdated(event) {
    let index = this.sprints.findIndex(sprint => sprint.id === event.id);
    this.sprints[index] = event;
  }

  openAddSprintDialog() {
    let modalRef = this.modalService.open(SprintModalComponent, {size: "dialog-centered"})
    modalRef.componentInstance.project = this.project;
    modalRef.result.then((result) => {
      if(result) {
        this.sprints.push(result);
        this.alertService.success("Added new sprint from "+ this.datePipe.transform(result['start']) + " to "+ this.datePipe.transform(result['end']));
      }
    }, (reason) => {
      if(reason) {
        this.alertService.error(reason);
      }
    });
  }

}
