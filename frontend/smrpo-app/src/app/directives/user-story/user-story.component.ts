import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { UserStory } from 'src/app/models/UserStory';
import { UserStoryPriority } from 'src/app/enums/UserStoryPriority';
import { UserStoryStatus } from 'src/app/enums/UserStoryStatus';
import { SprintService } from 'src/app/services/sprint.service';
import { Sprint } from 'src/app/models/Sprint';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { User } from 'src/app/models/User';
import { ProjectRole } from 'src/app/enums/ProjectRole';
import { Project } from 'src/app/models/Project';
import { AlertService } from 'src/app/services/alert.service';
import { UserStoryTime } from 'src/app/models/UserStoryTime';
import { UserStoryService } from 'src/app/services/user-story.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserStoryTask } from 'src/app/models/UserStoryTask';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { RejectUserStoryModalComponent } from '../reject-user-story-modal/reject-user-story-modal.component';
import { UserStoryModalComponent } from '../user-story-modal/user-story-modal.component';

@Component({
  selector: 'user-story',
  templateUrl: './user-story.component.html',
  styleUrls: ['./user-story.component.css']
})
export class UserStoryComponent implements OnInit {
  public userStoryTimeForm: FormGroup;

  @Input() userStory: UserStory;
  @Output() userStoryChange = new EventEmitter<UserStory>();
  @Output() userStoryRejected:EventEmitter<UserStory> = new EventEmitter<UserStory>();
  @Output() userStoryAccepted:EventEmitter<UserStory> = new EventEmitter<UserStory>();
  @Input() project: Project;
  @Input() sprints: Sprint[];

  userStoryTimes: UserStoryTime[];
  activeUserStoryTime: UserStoryTime;

  userStoryTasks: UserStoryTask[];
  @Input() userStoryTasksExpanded: boolean = false;
  @Input() userStoryTasksShow: boolean = false;

  currentUser: User;
  activeSprint: Sprint;

  constructor(public authenticationService: AuthenticationService,
              private sprintService: SprintService,
              private userStoryService: UserStoryService,
              private formBuilder: FormBuilder,
              private modalService: NgbModal,
              private alertService: AlertService) { }

  ngOnInit(): void {
    this.activeSprint = this.sprints.find(x => x.active === true);
    this.userStoryService.getUserStoryTimes(this.userStory.id).subscribe(
      res => {
        this.userStoryTimes = res;
        if(this.activeSprint) {
          this.activeUserStoryTime = this.userStoryTimes.find(x => x.sprint.id === this.activeSprint.id);
          this.userStoryTimeForm = this.formBuilder.group({
            estimation: [this.activeUserStoryTime.estimation, [Validators.required, Validators.min(0), Validators.pattern("^[0-9]*$")]],
          });
        }
      },
      err => {
        this.alertService.error(err.error.message);
      }
    )
    this.userStoryService.getUserStoryTasks(this.userStory.id).subscribe(
      res => {
        this.userStoryTasks = res;
      },
      err => {
        this.alertService.error(err.error.message);
      }
    )
  }

  get f() { return this.userStoryTimeForm.controls; }
  get UserStoryPriority() { return UserStoryPriority; }
  get UserStoryStatus() { return UserStoryStatus; }

  addToActiveSprint() {
    this.sprintService.addUserStory(this.activeSprint.id, this.userStory.id).subscribe(
      res => {
        this.userStory.sprint = this.activeSprint;
        this.alertService.success("User story added to sprint");
      },
      err => {
        this.alertService.error(err.error.message);
      }
    )
  }

  delete() {
    this.userStoryService.deleteUserStory(this.userStory.id).subscribe(
      res => {
        this.alertService.success("User story " + this.userStory.name + " deleted");
        this.userStory = null;
      },
      err => {
        this.alertService.error(err.error.message);
      }
    )
  }

  edit() {
    let modalRef = this.modalService.open(UserStoryModalComponent, {size: "dialog-centered"})
    modalRef.componentInstance.userStory = this.userStory;
    modalRef.result.then((result) => {
      if(result) {
        this.userStory = result;
        this.alertService.success("Updated user "+ this.userStory.name +" updated");
      }
    }, (reason) => {
      if(reason) {
        this.alertService.error(reason);
      }
    });
  }

  accept() {
    this.userStoryService.acceptUserStory(this.userStory.id).subscribe(
      res => {
        this.userStory = res;
        this.userStoryChange.emit(this.userStory);
        this.userStoryAccepted.emit(this.userStory);
        this.alertService.success("User story " + this.userStory.name + " approved");
      },
      err => {
        this.alertService.error(err.error.message);
      }
    )
  }

  reject() {
    let modalRef = this.modalService.open(RejectUserStoryModalComponent, {size: "dialog-centered"});
    modalRef.componentInstance.userStory = this.userStory;
    modalRef.result.then((result) => {
        if(result) {
          this.userStory = result;
          this.userStoryChange.emit(this.userStory);
          this.userStoryRejected.emit(this.userStory);
          this.alertService.success("User story " + this.userStory.name + " rejected");
        }
      }, (reason) => {
        if(reason) {
          this.alertService.error(reason);
        }
    });
  }

  allTasksDone(): boolean {
    return this.userStoryTasks.every(userStoryTask => userStoryTask.remainingTime === 0);
  }

  onBlur() {
    if (this.userStoryTimeForm.invalid) {
      if(this.f.estimation.errors.required) {
        this.alertService.error("Estimation is required");
      } else if(this.f.estimation.errors.min) {
        this.alertService.error("Estimation must be a positive number");
      } else {
        this.alertService.error("Estimation must be a number");
      }
      this.userStoryTimeForm.reset();
      this.f.estimation.setValue(this.activeUserStoryTime.estimation);
      return;
    }

    if(this.f.estimation.value !== this.activeUserStoryTime.estimation) {
      this.userStoryService.updateUserStoryTime(this.userStory.id, {estimation: this.f.estimation.value, sprintId: this.activeUserStoryTime.sprint.id}).subscribe(
        res => {
          this.alertService.success("Estimation updated to " + res.estimation);
          this.activeUserStoryTime.estimation = res.estimation;
        },
        err => {
          this.alertService.error(err.error.message);
          this.f.estimation.setValue(this.activeUserStoryTime.estimation);
        }
      )
    }
  }
}
