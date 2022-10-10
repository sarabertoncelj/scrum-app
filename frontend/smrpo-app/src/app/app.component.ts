import { AuthenticationService } from './services/authentication.service';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { User } from './models/User';
import { UserRole } from './enums/UserRole';
import { AlertService } from './services/alert.service';

@Component({
  selector: 'app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title: string = 'SMRPO app';
  currentUser: User;

  constructor(
    public authenticationService: AuthenticationService,
    private router: Router,
    private alertService: AlertService
  ) {
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
  }
  ngOnInit (): void {
    if (!this.currentUser) {
      this.router.navigate(['/login'])
    }
  }
  logout () {
    this.authenticationService.logout();
    this.alertService.warn("Successfully logged out", { keepAfterRouteChange: true });
    this.router.navigate(['/login']);
  }

  get lastLogin () {
    if (new Date(this.currentUser.lastLogin).toDateString() == new Date("0001-01-01T00:00:00").toDateString()) {
      return null;
    }
    return this.currentUser.lastLogin;
  }


  isLoggedIn (): boolean {
    return this.currentUser != null;
  }

  goToRegistration () {
    this.router.navigate(['/users']);
  }

  goToProjects () {
    this.router.navigate(['/projects']);
  }
}
