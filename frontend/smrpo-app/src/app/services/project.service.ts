import { ProjectRole } from 'src/app/enums/ProjectRole';
import { ProjectUser } from './../models/ProjectUser';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Project } from '../models/Project';
import { Observable } from 'rxjs';
import { UserStory } from '../models/UserStory';
import { Sprint } from '../models/Sprint';
import { ProjectPost } from '../models/ProjectPost';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private projectsUrl = environment.apiBaseUrl + '/projects';

  constructor(private http: HttpClient) { }

  getProject (projectId: string): Observable<Project> {
    return this.http.get<Project>(this.projectsUrl + "/" + projectId);
  }

  getProjects (): Observable<Project[]> {
    return this.http.get<Project[]>(this.projectsUrl);
  }

  addProject(project: Project): Observable<Project>{
    return this.http.post<Project>(this.projectsUrl, project);
  }

  editProject(projectId: string, project: Project): Observable<Project>{
    return this.http.put<Project>(this.projectsUrl + "/" + projectId, project);
  }

  deleteProject(projectId: string): Observable<void>{
    return this.http.delete<void>(this.projectsUrl + "/" + projectId);
  }

  addUserToProject(projectId: string, projectUser: ProjectUser): Observable<ProjectUser>{
    return this.http.post<ProjectUser>(this.projectsUrl + "/" + projectId + "/users", projectUser);
  }

  removeUserFromProject(projectId: string, userId: string): Observable<void>{
    return this.http.delete<void>(this.projectsUrl + "/" + projectId + "/users/" + userId);
  }

  removeUserProjectRole(projectId: string, userId: string, role: ProjectRole): Observable<void>{
    return this.http.delete<void>(this.projectsUrl + "/" + projectId + "/users/" + userId + "/" + role);
  }

  getUserStories (projectId: string): Observable<UserStory[]> {
    return this.http.get<UserStory[]>(this.projectsUrl + "/" + projectId + "/user_stories");
  }

  getSprints (projectId: string): Observable<Sprint[]> {
    return this.http.get<Sprint[]>(this.projectsUrl + "/" + projectId + "/sprints");
  }

  getProjectUsers (projectId: string): Observable<ProjectUser[]> {
    return this.http.get<ProjectUser[]>(this.projectsUrl + "/" + projectId + "/users");
  }

  getProjectPosts (projectId: string): Observable<ProjectPost[]> {
    return this.http.get<ProjectPost[]>(this.projectsUrl + "/" + projectId + "/posts");
  }

  addProjectPost(projectId: string, projectPost: ProjectPost): Observable<ProjectPost[]>{
    return this.http.post<ProjectPost[]>(this.projectsUrl + "/" + projectId + "/posts", projectPost);
  }

  
}
