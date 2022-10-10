import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserService } from 'src/app/services/user.service';
import { User } from 'src/app/models/User';
import { AlertService } from 'src/app/services/alert.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AdminGuard } from 'src/app/guards/admin-guard.service';
import { UserModalComponent } from 'src/app/directives/user-modal/user-modal.component';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
  users: User[];

  constructor(public authenticationService: AuthenticationService,
              private userService: UserService,
              private modalService: NgbModal,
              public adminGuard: AdminGuard,
              private alertService: AlertService) { }

  ngOnInit(): void {
    this.userService.getUsers().subscribe(
      res => {
        this.users = res
      },
      err => {
        if(err) {
          this.alertService.error(err);
        }
      }
    )
  }

  openAddUserDialog() {
    let modalRef = this.modalService.open(UserModalComponent, {size: "dialog-centered"})
    //modalRef.componentInstance.project = this.project;
    modalRef.result.then((result) => {
      if(result) {
        this.users.push(result);
        this.alertService.success("Added new user: " + result['username']);
      }
    }, (reason) => {
      if(reason) {
        this.alertService.error(reason);
      }
    });
  }

}
