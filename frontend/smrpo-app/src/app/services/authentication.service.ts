import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { map } from 'rxjs/operators';
import { User } from '../models/User';
import { ProjectRole } from '../enums/ProjectRole';
import { UserRole } from '../enums/UserRole';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private usersUrl = environment.apiBaseUrl + '/users';
  private currentUserSubject: BehaviorSubject<User>;
  public currentUser: Observable<User>;
  constructor(private http: HttpClient) { 
    this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();

  }
  public get currentUserValue(): User {
      return this.currentUserSubject.value;
  }

  login(username, password) {
      return this.http.post<any>(this.usersUrl + '/authenticate', { username, password })
          .pipe(map(user => {
              // store user details and jwt token in local storage to keep user logged in between page refreshes
              localStorage.setItem('currentUser', JSON.stringify(user));
              this.currentUserSubject.next(user);
              return user;
          }));
  }
  logout() {
    // remove user from local storage and set current user to null
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  setProjectRoles(projectRoles: ProjectRole[]) {
    let user: User = this.currentUserValue;
    user.projectRoles = projectRoles;
    localStorage.setItem('currentUser', JSON.stringify(user));
  }

  isScrumMaster():boolean {
    if(this.currentUserValue.role === UserRole.Administrator) {
      return true;
    }
    if(this.currentUserValue.projectRoles) {
      return this.currentUserValue.projectRoles.includes(ProjectRole["Scrum Master"]);
    } else {
      return false;
    }
  }

  isProductOwner():boolean {
    if(this.currentUserValue.role === UserRole.Administrator) {
      return true;
    }
    if(this.currentUserValue.projectRoles) {
      return this.currentUserValue.projectRoles.includes(ProjectRole["Product Owner"]);
    } else {
      return false;
    }
  }

  isDeveloper():boolean {
    if(this.currentUserValue.role === UserRole.Administrator) {
      return true;
    }
    if(this.currentUserValue.projectRoles) {
      return this.currentUserValue.projectRoles.includes(ProjectRole['Developer']);
    } else {
      return false;
    }
  }

  isAdministrator():boolean {
    return this.currentUserValue.role === UserRole.Administrator;
  }
}
