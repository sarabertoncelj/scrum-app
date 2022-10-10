import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserStory } from '../models/UserStory';
import { UserStoryTime } from '../models/UserStoryTime';
import { UserStoryTask } from '../models/UserStoryTask';

@Injectable({
  providedIn: 'root'
})
export class UserStoryService {
  private userStoriesUrl = environment.apiBaseUrl + '/user_stories';

  constructor(private http: HttpClient) { }
  
  addUserStory (userStory: UserStory): Observable<UserStory> {
    return this.http.post<UserStory>(this.userStoriesUrl, userStory);
  }

  getUserStoryTimes (userStoryId: string): Observable<UserStoryTime[]> {
    return this.http.get<UserStoryTime[]>(this.userStoriesUrl + "/" + userStoryId + "/user_story_times");
  }

  updateUserStoryTime (userStoryId: string, userStoryTime: any): Observable<UserStoryTime> {
    return this.http.post<UserStoryTime>(this.userStoriesUrl + "/" + userStoryId + "/user_story_times", userStoryTime);
  }

  getUserStoryTasks (userStoryId: string): Observable<UserStoryTask[]> {
    return this.http.get<UserStoryTask[]>(this.userStoriesUrl + "/" + userStoryId + "/tasks");
  }

  acceptUserStory (userStoryId: string): Observable<UserStory> {
    return this.http.post<UserStory>(this.userStoriesUrl + "/" + userStoryId + "/accept", {});
  }

  rejectUserStory (userStoryId: string, comment: string): Observable<UserStory> {
    return this.http.post<UserStory>(this.userStoriesUrl + "/" + userStoryId + "/reject", {"comment": comment});
  }

  deleteUserStory (userStoryId: string): Observable<void> {
    return this.http.delete<void>(this.userStoriesUrl + "/" + userStoryId)
  }

  updateUserStory (userStoryId: string, userStory: UserStory): Observable<UserStory> {
    return this.http.put<UserStory>(this.userStoriesUrl + "/" + userStoryId, userStory);
  }
}
