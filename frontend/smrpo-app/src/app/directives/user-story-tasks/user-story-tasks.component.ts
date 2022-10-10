import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { UserStory } from 'src/app/models/UserStory';
import { UserStoryTask } from 'src/app/models/UserStoryTask';
import { UserStoryTaskStatus } from 'src/app/enums/UserStoryTaskStatus';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { User } from 'src/app/models/User';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserStoryTaskModalComponent } from '../user-story-task-modal/user-story-task-modal.component';
import { AlertService } from 'src/app/services/alert.service';
import { Project } from 'src/app/models/Project';
import { ProjectService } from 'src/app/services/project.service';
import { ProjectUser } from 'src/app/models/ProjectUser';
import { UserStoryTaskService } from 'src/app/services/user-story-task.service';
import { UserStoryStatus } from 'src/app/enums/UserStoryStatus';
import { ProjectRole } from 'src/app/enums/ProjectRole';
import { WorkLogsModalComponent } from '../work-logs-modal/work-logs-modal.component';
import { UserStoryService } from 'src/app/services/user-story.service';

@Component({
  selector: 'user-story-tasks',
  templateUrl: './user-story-tasks.component.html',
  styleUrls: ['./user-story-tasks.component.css']
})
export class UserStoryTasksComponent implements OnInit {
  @Input() userStoryTasks: UserStoryTask[];
  @Output() userStoryTasksChange = new EventEmitter<UserStoryTask[]>();
  @Output() userStoryTasksRemove = new EventEmitter<UserStoryTask[]>();
  @Input() project: Project;
  @Input() userStory: UserStory;
  @Input() userStoryTask: UserStoryTask;

  projectUsers: ProjectUser[];

  constructor(public authenticationService: AuthenticationService,
              private projectService: ProjectService,
              private userStoryTaskService: UserStoryTaskService,
              private modalService: NgbModal,
              private userStoryService: UserStoryService,
              private alertService: AlertService) { }

  get UserStoryTaskStatus() { return UserStoryTaskStatus; }
  get UserStoryStatus() { return UserStoryStatus; }

  ngOnInit(): void {
    this.projectService.getProjectUsers(this.project.id).subscribe(
      res => {
        this.projectUsers = res.filter(x => x.projectRoles.includes(ProjectRole.Developer) && (this.authenticationService.isScrumMaster() || this.authenticationService.currentUserValue.id === x.id));
      },
      err => {
        this.alertService.error(err.error.message);
      }
    );
  }

  isThisUser(user: User):boolean {
    return this.authenticationService.currentUserValue.id === user.id;
  }

  openAddUserStoryTaskDialog() {
    let modalRef = this.modalService.open(UserStoryTaskModalComponent, {size: "dialog-centered"});
    modalRef.componentInstance.projectUsers = this.projectUsers;
    modalRef.componentInstance.userStory = this.userStory;
    modalRef.result.then((result) => {
        if(result) {
          this.userStoryTasks = result;
          this.userStoryTasksChange.emit(this.userStoryTasks);
          this.alertService.success("Added new task");
        }
      }, (reason) => {
        if(reason) {
          this.alertService.error(reason);
        }
    });
  }

  openWorkLogsDialog(task: UserStoryTask) {
    let modalRef = this.modalService.open(WorkLogsModalComponent, {size: "dialog-centered"});
    modalRef.componentInstance.task = task;
    modalRef.componentInstance.projectUsers = this.projectUsers;
    modalRef.result.then((result) => {
        if(result) {
          this.alertService.success(result);
        }
        this.refreshUserStoryTasks();
      }, (reason) => {
        if(reason) {
          this.alertService.error(reason);
        }
        this.refreshUserStoryTasks();
    });
  }

  refreshUserStoryTasks() {
    this.userStoryService.getUserStoryTasks(this.userStory.id).subscribe(
      res => {
        this.userStoryTasks = res;
      },
      err => {
        this.alertService.error(err.error.message);
      }
    )
  }

  assignTo(userStoryTask: UserStoryTask, projectUser: ProjectUser) {
    this.userStoryTaskService.assignUserStoryTask(userStoryTask.id, projectUser.id).subscribe(
      res => {
        this.showUpdatedTask(res);
        this.alertService.success("Assigned task to user " + projectUser.username);
      },
      err => {
        this.alertService.error(err.error.message);
      }
    );
  }

  acceptTask(userStoryTask: UserStoryTask) {
    this.userStoryTaskService.acceptUserStoryTask(userStoryTask.id).subscribe(
      res => {
        this.showUpdatedTask(res);
        this.alertService.success("You accepted task" + userStoryTask.description);
      },
      err => {
        this.alertService.error(err.error.message);
      }
    );
  }

  declineTask(userStoryTask: UserStoryTask) {
    this.userStoryTaskService.declineUserStoryTask(userStoryTask.id).subscribe(
      res => {
        this.showUpdatedTask(res);
        this.alertService.success("You declined task" + userStoryTask.description);
      },
      err => {
        this.alertService.error(err.error.message);
      }
    );
  }

  startTask(userStoryTask: UserStoryTask) {
    this.userStoryTaskService.startUserStoryTask(userStoryTask.id).subscribe(
      res => {
        this.showUpdatedTask(res);
        this.alertService.success("You started working on task" + userStoryTask.description);
      },
      err => {
        this.alertService.error(err.error.message);
      }
    );
  }

  stopTask(userStoryTask: UserStoryTask) {
    this.userStoryTaskService.stopUserStoryTask(userStoryTask.id).subscribe(
      res => {
        this.showUpdatedTask(res);
        this.alertService.success("You stopped working on task" + userStoryTask.description);
      },
      err => {
        this.alertService.error(err.error.message);
      }
    );
  }

  endTask(userStoryTask: UserStoryTask) {
    this.userStoryTaskService.finishUserStoryTask(userStoryTask.id).subscribe(
      res => {
        this.showUpdatedTask(res);
        this.alertService.success("You ended task" + userStoryTask.description);
      },
      err => {
        this.alertService.error(err.error.message);
      }
    );
  }

  showUpdatedTask(userStoryTask: UserStoryTask){
    let updateItem = this.userStoryTasks.find(x => x.id === userStoryTask.id);
    let index = this.userStoryTasks.indexOf(updateItem);
    this.userStoryTasks[index] = userStoryTask;
  }

  editTask(userStoryTask: UserStoryTask) {
    let modalRef = this.modalService.open(UserStoryTaskModalComponent, {size: "dialog-centered"})
    modalRef.componentInstance.projectUsers = this.projectUsers;
    modalRef.componentInstance.userStory = this.userStory;
    modalRef.componentInstance.userStoryTask = userStoryTask;
    modalRef.result.then((result) => {
      if(result) {        
        this.userStoryTasks = result;
        this.userStoryTasksChange.emit(this.userStoryTasks);
        this.alertService.success("Task "+ userStoryTask.description + " updated successfully!");
      }
    }, (reason) => {
      if(reason) {
        this.alertService.error(reason);
      }
    });
  }

  removeTask(userStoryTask: UserStoryTask){
    let removedItem = this.userStoryTasks.find(x => x.id === userStoryTask.id);
    let index = this.userStoryTasks.indexOf(removedItem);
    this.userStoryTasks.splice(index, 1);
  }
  deleteTask(userStoryTask: UserStoryTask) {
    this.userStoryTaskService.deleteUserStoryTask(userStoryTask.id).subscribe(
      () => {
        this.removeTask(userStoryTask);
        this.userStoryTasksRemove.emit(this.userStoryTasks);
        this.alertService.success("Task "+ userStoryTask.description + " removed successfully!");
      },
      (err) => {
        this.alertService.error(err.error.message);
      }
    )
  }

  handleTaskStop(task: UserStoryTask) {
    this.stopTask(task);
  }
}
