import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { UserStoryTask } from 'src/app/models/UserStoryTask';

@Component({
  selector: 'active-task',
  templateUrl: './active-task.component.html',
  styleUrls: ['./active-task.component.css']
})
export class ActiveTaskComponent implements OnInit, OnDestroy {
  @Input() task: UserStoryTask;
  time: string;
  interval;

  @Output() stopTaskEvent: EventEmitter<UserStoryTask> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
    this.interval = setInterval(() => {
      this.calculateTimer(new Date().getTime() - (new Date(this.task.activeFrom)).getTime());
    },1000)
  }

  ngOnDestroy(): void {
    clearInterval(this.interval);
  }

  stopTask() {
    this.stopTaskEvent.emit(this.task);
  }

  calculateTimer(timeMs) {
    var ms = timeMs % 1000;
    timeMs = (timeMs - ms) / 1000;
    var secs = timeMs % 60;
    timeMs = (timeMs - secs) / 60;
    var mins = timeMs % 60;
    var hrs = (timeMs - mins) / 60;
  
    this.time = hrs.toString().padStart(2, "0") + ' : ' + mins.toString().padStart(2, "0") + ' : ' + secs.toString().padStart(2, "0");
  }

}
