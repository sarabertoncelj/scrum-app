import { ProjectModalComponent } from '../../directives/project-modal/project-modal.component';
import { Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import { Project } from 'src/app/models/Project';
import { ProjectService } from 'src/app/services/project.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AdminGuard } from 'src/app/guards/admin-guard.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { User } from 'src/app/models/User';
import { UserRole } from 'src/app/enums/UserRole';
import { AlertService } from 'src/app/services/alert.service';
import { ProjectRole } from 'src/app/enums/ProjectRole';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css']
})
export class ProjectsComponent implements OnInit {
  @Input() projects: Project[];

  currentUser: User;

  constructor(public authenticationService: AuthenticationService,
              private projectService: ProjectService,
              private modalService: NgbModal,
              public adminGuard: AdminGuard,
              private alertService: AlertService) { }

  ngOnInit(): void {
    this.authenticationService.currentUser.subscribe(x => this.currentUser = x);
    this.projectService.getProjects().subscribe(
      res => {
        this.projects = res
      },
      err => {
        if(err) {
          this.alertService.error(err.error.message);
        }
      }
    )
  }

  openAddProjectDialog() {
    let modalRef = this.modalService.open(ProjectModalComponent, {size: "dialog-centered"})
    //modalRef.componentInstance.project = this.project;
    modalRef.result.then((result) => {
      if(result) {
        this.projects.push(result);
        this.alertService.success("Added new project: " + result['name']);
      }
    }, (reason) => {
      if(reason) {
        this.alertService.error(reason);
      }
    });
  }
  updateProjects(project: Project){
    let updatedItem = this.projects.find(x => x.id === project.id);
    let index = this.projects.indexOf(updatedItem);
    this.projects[index] = project;
  }

  openEditProjectDialog(project: Project) {
    let modalRef = this.modalService.open(ProjectModalComponent, {size: "dialog-centered"})
    modalRef.componentInstance.project = project;
    modalRef.result.then((result) => {
      if(result) {
        this.updateProjects(result);
        this.alertService.success("Project updated!");
      }
    }, (reason) => {
      if(reason) {
        this.alertService.error(reason);
      }
    });
  }
  removeProject(project: Project){
    let removedItem = this.projects.find(x => x.id === project.id);
    let index = this.projects.indexOf(removedItem);
    this.projects.splice(index, 1);
  }
  deleteProject(project: Project) {
    this.projectService.deleteProject(project.id).subscribe(
      () => {
        this.removeProject(project);
        this.alertService.success("Removed project: "+ project.name);
      },
      (err) => {
        this.alertService.error(err.error.message);
      }
    )
  }

  canEditOrDelete(project: Project): boolean {
    if(this.authenticationService.isAdministrator()) {
      return true;
    }
    return project.projectRoles.includes(ProjectRole["Scrum Master"]);
  }

}
