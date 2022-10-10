import { Component, OnInit } from '@angular/core';
import { UserStoryTask } from 'src/app/models/UserStoryTask';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UserStoryTaskService } from 'src/app/services/user-story-task.service';
import { WorkLog } from 'src/app/models/WorkLog';
import { AlertService } from 'src/app/services/alert.service';
import { User } from 'src/app/models/User';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ProjectUser } from 'src/app/models/ProjectUser';

@Component({
  selector: 'app-work-logs-modal',
  templateUrl: './work-logs-modal.component.html',
  styleUrls: ['./work-logs-modal.component.css']
})
export class WorkLogsModalComponent implements OnInit {  
  task: UserStoryTask;
  projectUsers: ProjectUser[];
  workLogs: WorkLog[] = [];

  currentWorkLogUser: ProjectUser;
  workLogsUsers: ProjectUser[] = [];
  workLogsPerUser: WorkLog[] = [];

  constructor(
    public activeModal: NgbActiveModal,
    private userStoryTaskService: UserStoryTaskService,
    private alertService: AlertService,
    public authenticationService: AuthenticationService
  ) {}

  ngOnInit(): void {
    this.userStoryTaskService.getWorkLogs(this.task.id).subscribe(
      workLogs => {
        this.workLogs = workLogs;
        this.extractUsers();
        this.currentWorkLogUser = this.workLogsUsers.filter(x => x.id === this.authenticationService.currentUserValue.id)[0];
        if(this.currentWorkLogUser) {
          this.workLogsPerUser = this.workLogs.filter(x => x.userId == this.currentWorkLogUser.id)
        }
      },
      err => {
        this.alertService.error(err.error.message);
      }
    )
  }

  extractUsers() {
    this.workLogs.forEach(workLog => {
      if(this.workLogsUsers.filter(x => x.id == workLog.userId).length == 0) {
        this.workLogsUsers.push(this.projectUsers.filter(x => x.id === workLog.userId)[0]);
      }
    })
  }

  selectWorkLogsUsers(workLogsUser: ProjectUser) {
    this.currentWorkLogUser = this.workLogsUsers.filter(x => x.id === workLogsUser.id)[0];
    this.workLogsPerUser = this.workLogs.filter(x => x.userId == this.currentWorkLogUser.id)
  }

  canEdit(): boolean {
    return this.currentWorkLogUser.id === this.authenticationService.currentUserValue.id;
  }
}
