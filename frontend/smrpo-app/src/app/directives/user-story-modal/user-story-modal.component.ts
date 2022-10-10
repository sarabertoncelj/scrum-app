import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { UserStoryPriority } from 'src/app/enums/UserStoryPriority';
import { UserStoryService } from 'src/app/services/user-story.service';
import { Project } from 'src/app/models/Project';
declare var jQuery:any;
import 'bootstrap-input-spinner/src/bootstrap-input-spinner.js';
import { UserStory } from 'src/app/models/UserStory';

@Component({
  selector: 'user-story-modal',
  templateUrl: './user-story-modal.component.html',
  styleUrls: ['./user-story-modal.component.css']
})
export class UserStoryModalComponent implements OnInit {
  public userStoryForm: FormGroup;
  newAcceptanceTest: string;
  loading: boolean = false;
  submitted: boolean = false;

  userStory: UserStory;

  @Input() project: Project;

  constructor(public activeModal: NgbActiveModal,
              private formBuilder: FormBuilder,
              private userStoryService: UserStoryService) {}

  get f() { return this.userStoryForm.controls; }
  get UserStoryPriority() { return UserStoryPriority; }

  ngOnInit(): void {
    let config = {
      buttonsClass: "btn-outline-dark",
      textAlign: "center",
    }
    jQuery("input[type='number']").inputSpinner(config);

    let name = '';
    let description = '';
    let acceptanceTest = [];
    let priority = UserStoryPriority["Must have"];
    let businessValue = 1;

    if(this.userStory) {
      name = this.userStory.name;
      description = this.userStory.description;
      acceptanceTest = this.userStory.acceptanceTests;
      priority = this.userStory.priority;
      businessValue = this.userStory.businessValue;
      jQuery("input[type='number']").val(businessValue);
    }

    this.userStoryForm = this.formBuilder.group({
      name: [name, Validators.required],
      description: [description],
      acceptanceTests: this.formBuilder.array(acceptanceTest, Validators.required),
      priority: [priority, Validators.required],
      businessValue: [businessValue, Validators.required]
    });
  }

  onSubmit() {
    this.submitted = true;
    // stop here if form is invalid
    if (this.userStoryForm.invalid) {
      return;
    }

    this.loading = true;
    if(!this.userStory) {
      this.userStoryForm.value['projectId'] = this.project.id;
      this.userStoryService.addUserStory(this.userStoryForm.value).subscribe(
        res => {
          this.activeModal.close(res);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
    } else {
      this.userStoryService.updateUserStory(this.userStory.id, this.userStoryForm.value).subscribe(
        res => {
          this.activeModal.close(res);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
    }

  }

  addAcceptanceTest() {
    let acceptanceTests: FormArray = this.userStoryForm.get('acceptanceTests') as FormArray;
    let formControl: FormControl = new FormControl('acceptanceTest', Validators.required);
    formControl.setValue(this.newAcceptanceTest);
    acceptanceTests.push(formControl);
    this.newAcceptanceTest = "";
  }

  removeAcceptanceTest(index: number) {
    let acceptanceTests = this.userStoryForm.get('acceptanceTests') as FormArray;
    acceptanceTests.removeAt(index);
  }
}
