import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ProjectUser } from 'src/app/models/ProjectUser';
import { UserStoryTaskService } from 'src/app/services/user-story-task.service';
import { UserStory } from 'src/app/models/UserStory';
declare var jQuery:any;
import 'bootstrap-input-spinner/src/bootstrap-input-spinner.js';
import { UserStoryTask } from '../../models/UserStoryTask';

@Component({
  selector: 'app-user-story-task-modal',
  templateUrl: './user-story-task-modal.component.html',
  styleUrls: ['./user-story-task-modal.component.css']
})
export class UserStoryTaskModalComponent implements OnInit {
  public userStoryTaskForm: FormGroup;
  loading = false;
  submitted = false;
  
  projectUsers: ProjectUser[];
  userStory: UserStory;
  userStoryTask: UserStoryTask;

  constructor(
    public activeModal: NgbActiveModal,
    private userStoryTaskService: UserStoryTaskService,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit(): void {
    let config = {
        buttonsClass: "btn-outline-dark",
        textAlign: "center",
    }
    jQuery("input[type='number']").inputSpinner(config);

    let description = '';
    let remainingTime = null;
    let userId = ''
    let blockAssignUser = false;

    if(this.userStoryTask) {
      description = this.userStoryTask.description;
      remainingTime = this.userStoryTask.remainingTime;
      if(this.userStoryTask.user) {
        userId = this.userStoryTask.user.id;
      }

      blockAssignUser = this.userStoryTask.accepted;

      jQuery("input[type='number']").val(remainingTime);
    }

    this.userStoryTaskForm = this.formBuilder.group({
      description: [description, Validators.required],
      remainingTime: [remainingTime, Validators.required],
      userId: [{value: userId, disabled: blockAssignUser}],
      userStoryId: [this.userStory.id]
    });
  }
  get f() { return this.userStoryTaskForm.controls; }

  onSubmit() {
    this.submitted = true;

    if (this.userStoryTaskForm.invalid) {
      return;
    }
        
    this.loading = true;
    if (!this.userStoryTask){
      this.userStoryTaskService.addUserStoryTask(this.userStoryTaskForm.value).subscribe(
        res => {
          this.activeModal.close(res);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
    } else {
      this.userStoryTaskService.updateUserStoryTask(this.userStoryTask.id, this.userStoryTaskForm.value).subscribe(
        res => {
          this.activeModal.close(res);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
    }
    
  }
}
