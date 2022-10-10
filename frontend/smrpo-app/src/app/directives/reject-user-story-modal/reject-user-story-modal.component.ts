import { Component, OnInit } from '@angular/core';
import { UserStory } from 'src/app/models/UserStory';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserStoryService } from 'src/app/services/user-story.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-reject-user-story-modal',
  templateUrl: './reject-user-story-modal.component.html',
  styleUrls: ['./reject-user-story-modal.component.css']
})
export class RejectUserStoryModalComponent implements OnInit {
  userStory: UserStory;
  rejectForm: FormGroup;
  loading = false;
  submitted = false;

  constructor(
    private formBuilder: FormBuilder,
    private userStoryService: UserStoryService,
    public activeModal: NgbActiveModal
  ) {}

  ngOnInit (): void {
    this.rejectForm = this.formBuilder.group({
      comment: ['', Validators.required],
    });
  }

  get f () { return this.rejectForm.controls; }

  onSubmit () {
    this.submitted = true;
    // stop here if form is invalid
    if (this.rejectForm.invalid) {
      return;
    }

    this.loading = true;
    this.userStoryService.rejectUserStory(this.userStory.id, this.f.comment.value).subscribe(
        userStory => {
          this.activeModal.close(userStory);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
  }
}
