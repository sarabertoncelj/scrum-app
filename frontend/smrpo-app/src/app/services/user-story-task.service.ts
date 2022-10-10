import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { UserStoryTask } from '../models/UserStoryTask';
import { Observable } from 'rxjs';
import { WorkLog } from '../models/WorkLog';

@Injectable({
  providedIn: 'root'
})
export class UserStoryTaskService {
  private userStoryTasksUrl = environment.apiBaseUrl + '/tasks';

  constructor(private http: HttpClient) { }
  
  addUserStoryTask (userStoryTaskCreate: any): Observable<UserStoryTask[]> {
    return this.http.post<UserStoryTask[]>(this.userStoryTasksUrl, userStoryTaskCreate);
  }
  updateUserStoryTask (userStoryTaskId: string, userStoryTask: UserStoryTask): Observable<UserStoryTask> {
    return this.http.put<UserStoryTask>(this.userStoryTasksUrl + "/" + userStoryTaskId, userStoryTask);
  }
  deleteUserStoryTask (userStoryTaskId: string): Observable<UserStoryTask> {
    return this.http.delete<UserStoryTask>(this.userStoryTasksUrl + "/" + userStoryTaskId);
  }
  assignUserStoryTask (userStoryTaskId: string, userId: string): Observable<UserStoryTask> {
    return this.http.post<UserStoryTask>(this.userStoryTasksUrl + "/" + userStoryTaskId + "/assign/" + userId, {});
  }
  acceptUserStoryTask (userStoryTaskId: string): Observable<UserStoryTask> {
    return this.http.post<UserStoryTask>(this.userStoryTasksUrl + "/" + userStoryTaskId + "/accept", {});
  }
  declineUserStoryTask (userStoryTaskId: string): Observable<UserStoryTask> {
    return this.http.post<UserStoryTask>(this.userStoryTasksUrl + "/" + userStoryTaskId + "/decline", {});
  }
  finishUserStoryTask (userStoryTaskId: string): Observable<UserStoryTask> {
    return this.http.post<UserStoryTask>(this.userStoryTasksUrl + "/" + userStoryTaskId + "/finish", {});
  }
  startUserStoryTask (userStoryTaskId: string): Observable<UserStoryTask> {
    return this.http.post<UserStoryTask>(this.userStoryTasksUrl + "/" + userStoryTaskId + "/start", {});
  }
  stopUserStoryTask (userStoryTaskId: string): Observable<UserStoryTask> {
    return this.http.post<UserStoryTask>(this.userStoryTasksUrl + "/" + userStoryTaskId + "/end", {});
  }

  getWorkLogs (userStoryTaskId: string): Observable<WorkLog[]> {
    return this.http.get<WorkLog[]>(this.userStoryTasksUrl + "/" + userStoryTaskId + "/work_logs");
  }
  createWorkLog (userStoryTaskId: string, workLog: WorkLog): Observable<WorkLog> {
    return this.http.post<WorkLog>(this.userStoryTasksUrl + "/" + userStoryTaskId + "/work_logs", workLog);
  }
  updateWorkLog (userStoryTaskId: string, workLogId: string, workLog: WorkLog): Observable<WorkLog[]> {
    return this.http.post<WorkLog[]>(this.userStoryTasksUrl + "/" + userStoryTaskId + "/work_logs/" + workLogId, workLog);
  }
}
