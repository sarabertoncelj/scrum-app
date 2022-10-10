import { Component, OnInit } from '@angular/core';
import { Project } from 'src/app/models/Project';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { SprintService } from 'src/app/services/sprint.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
declare var jQuery: any;
import 'bootstrap-input-spinner/src/bootstrap-input-spinner.js';
import { Sprint } from 'src/app/models/Sprint';
@Component({
  selector: 'app-sprint-modal',
  templateUrl: './sprint-modal.component.html',
  styleUrls: ['./sprint-modal.component.css']
})
export class SprintModalComponent implements OnInit {
  project: Project;
  sprint: Sprint;
  sprintForm: FormGroup;
  loading = false;
  submitted = false;

  constructor(
    private formBuilder: FormBuilder,
    private service: SprintService,
    public activeModal: NgbActiveModal
  ) {

  }

  ngOnInit (): void {
    let config = {
      buttonsClass: "btn-outline-dark",
      textAlign: "center",
    }
    jQuery("input[type='number']").inputSpinner(config);

    let now: Date = new Date();
    now.setHours(0);
    now.setMinutes(0);
    now.setSeconds(0);
    now.setMilliseconds(0);

    let inAWeek: Date = new Date();
    inAWeek.setDate(inAWeek.getDate() + 7);
    let velocity = null;

    if(this.sprint) {
      now = new Date(this.sprint.start);
      inAWeek = new Date(this.sprint.end);
      velocity = this.sprint.velocity;
      jQuery("input[type='number']").val(velocity);
    }

    this.sprintForm = this.formBuilder.group({
      projectId: [this.project.id, Validators.required],
      start: [{value: now, disabled: this.sprint.active}, [Validators.required, this.InPastValidator]],
      end: [{value: inAWeek, disabled: this.sprint.active}, Validators.required],
      velocity: [velocity, Validators.required],
    }, {validator: this.EndBeforeStartValidator }); 
  }

  EndBeforeStartValidator (group: FormGroup) {
    let start = group.get('start').value;
    let end = group.get('end').value;

    let returnObj = {};
    if (start >= end) {
      returnObj['endBeforeStart'] = true;
    }

    if (Object.keys(returnObj).length > 0) {
      return returnObj;
    } else {
      return null;
    }
  }

  InPastValidator (control: FormControl) {
    let start = control.value;

    let now: Date = new Date();
    now.setHours(0);
    now.setMinutes(0);
    now.setSeconds(0);
    now.setMilliseconds(0);
    return start < now ? { 'inPast': true } : null;
  }

  get f () { return this.sprintForm.controls; }

  onSubmit () {
    this.submitted = true;
    // stop here if form is invalid
    if (this.sprintForm.invalid) {
      return;
    }

    let startDate = this.f.start.value;
    startDate.setHours(2);
    startDate.setMinutes(0);
    startDate.setSeconds(1);
    startDate.setMilliseconds(1);
    this.f.start.setValue(startDate);

    let endDate = this.f.end.value;
    endDate.setHours(23);
    endDate.setMinutes(59);
    endDate.setSeconds(59);
    endDate.setMilliseconds(999);
    this.f.end.setValue(endDate);

    this.loading = true;
    if(!this.sprint) {
      this.service.addSprint(this.sprintForm.value).subscribe(
        data => {
          this.activeModal.close(data);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
    } else {
      this.service.updateSprint(this.sprint.id, this.sprintForm.value).subscribe(
        data => {
          this.activeModal.close(data);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
    }
  }
}
