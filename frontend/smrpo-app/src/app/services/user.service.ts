import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private usersUrl = environment.apiBaseUrl + '/users';

  constructor(private http: HttpClient) { }
  register (user: User){
    return this.http.post(this.usersUrl + '/register', user);
  }
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.usersUrl);
  }
}
