import { Component, OnInit } from '@angular/core';
import { AlertService } from 'src/app/services/alert.service';

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.css']
})
export class ErrorComponent implements OnInit {

  constructor(private alertService: AlertService) { }

  ngOnInit(): void {
    this.alertService.error("Page not found!")
  }

}
