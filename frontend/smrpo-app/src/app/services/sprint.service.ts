import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Sprint } from '../models/Sprint';
import { Observable } from 'rxjs';
import { UserStory } from '../models/UserStory';

@Injectable({
  providedIn: 'root'
})
export class SprintService {
  private sprintsUrl = environment.apiBaseUrl + '/sprints';

  constructor(private http: HttpClient) { }

  addSprint (sprint: Sprint): Observable<Sprint> {
    return this.http.post<Sprint>(this.sprintsUrl, sprint);
  }

  updateSprint (sprintId: string, sprint: Sprint): Observable<Sprint> {
    return this.http.put<Sprint>(this.sprintsUrl + "/" + sprintId, sprint);
  }

  deleteSprint (sprintId: string): Observable<void> {
    return this.http.delete<void>(this.sprintsUrl + "/" + sprintId);
  }

  addUserStory(sprintId: string, userStoryId: string): Observable<UserStory[]> {
    return this.http.post<UserStory[]>(this.sprintsUrl + "/" + sprintId + "/user_stories/" + userStoryId, {});
  }

  getSprintUserStories (sprintId: string): Observable<UserStory[]> {
    return this.http.get<UserStory[]>(this.sprintsUrl + "/" + sprintId + "/user_stories");
  }
}
