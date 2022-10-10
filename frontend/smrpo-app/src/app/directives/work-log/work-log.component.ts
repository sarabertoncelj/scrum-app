import { Component, OnInit, Input, AfterViewInit } from '@angular/core';
import { WorkLog } from 'src/app/models/WorkLog';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AlertService } from 'src/app/services/alert.service';
declare var jQuery:any;
import 'bootstrap-input-spinner/src/bootstrap-input-spinner.js';
import { UserStoryTaskService } from 'src/app/services/user-story-task.service';
import { UserStoryTask } from 'src/app/models/UserStoryTask';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'work-log',
  templateUrl: './work-log.component.html',
  styleUrls: ['./work-log.component.css']
})
export class WorkLogComponent implements OnInit, AfterViewInit {
  @Input() task: UserStoryTask;
  @Input() workLog: WorkLog;
  @Input() indexValue: number;
  @Input() lengthValue: number;
  @Input() readOnly: boolean;

  loading: boolean = false;
  public workLogForm: FormGroup;

  constructor(private formBuilder: FormBuilder,
              private userStoryTaskService: UserStoryTaskService,
              private alertService: AlertService,
              private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.workLogForm = this.formBuilder.group({
      hoursWorked: [{value: this.workLog.hoursWorked, disabled: this.readOnly}, Validators.required],
      hoursRemaining: [{value: this.workLog.hoursRemaining, disabled: this.readOnly}, Validators.required],
    });
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      let config = {
        buttonsClass: "btn-outline-dark",
        textAlign: "center",
      }
      
      jQuery("#hoursRemaining_"+ this.indexValue ).inputSpinner(config);
      jQuery("#hoursWorked_"+ this.indexValue).inputSpinner(config);
      }, 50);

  }

  get f() { return this.workLogForm.controls; }

  onSubmit() {
    if (this.workLogForm.invalid) {
      if(this.f.hoursWorked.errors && this.f.hoursWorked.errors.required) {
        this.alertService.error("Hours worked is required");
      }

      if(this.f.hoursRemaining.errors && this.f.hoursRemaining.errors.required) {
        this.alertService.error("Hours remaining is required");
      }
      this.workLogForm.reset();
      this.f.hoursWorked.setValue(this.workLog.hoursWorked);
      jQuery("#hoursWorked_"+ this.indexValue).val(this.workLog.hoursWorked);
      this.f.hoursRemaining.setValue(this.workLog.hoursRemaining);
      jQuery("#hoursRemaining_"+ this.indexValue).val(this.workLog.hoursRemaining);
      return;
    }

    this.loading = true;
    let updateWorkLog = new WorkLog();
    updateWorkLog.hoursRemaining = this.f.hoursRemaining.value;
    updateWorkLog.hoursWorked = this.f.hoursWorked.value;
    this.userStoryTaskService.updateWorkLog(this.task.id, this.workLog.id, updateWorkLog).subscribe(
      res => {
        let updatedWorkLog = res.filter(x => x.id == this.workLog.id)[0]
        this.alertService.success("Saved work log " + this.datePipe.transform(updatedWorkLog.day));
        this.workLog = updatedWorkLog;
        this.loading = false
      },
      err => {
        this.alertService.error(err.error.message);
        this.f.hoursWorked.setValue(this.workLog.hoursWorked);
        jQuery("#hoursWorked_"+ this.indexValue).val(this.workLog.hoursWorked);
        this.f.hoursRemaining.setValue(this.workLog.hoursRemaining);
        jQuery("#hoursRemaining_"+ this.indexValue).val(this.workLog.hoursRemaining);
        this.loading = false
      }
    );
  }
}
