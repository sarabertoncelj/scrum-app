import { AuthenticationService } from '../../services/authentication.service';
import { ProjectRole } from '../../enums/ProjectRole';
import { ProjectUser } from '../../models/ProjectUser';
import { Project } from '../../models/Project';
import { User } from '../../models/User';
import { ProjectService } from '../../services/project.service';
import { first } from 'rxjs/operators';
import { UserService } from '../../services/user.service';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'project-modal',
  templateUrl: './project-modal.component.html',
  styleUrls: ['./project-modal.component.css']
})
export class ProjectModalComponent implements OnInit {
  public projectForm: FormGroup;
  project: Project;
  currentProjectUsers: ProjectUser[];
  currentDevelopers: ProjectUser[] = [];
  loading = false;
  submitted = false;
  error: string;
  
  users: User[];
  usersCanAdd: User[];

  constructor(
    private authenticationService: AuthenticationService,
    public activeModal: NgbActiveModal,
    private service: ProjectService,
    private userService: UserService,
    private formBuilder: FormBuilder,
  ) {}

  ngOnInit(): void {

    let name = '';
    let scrumMaster = '';
    let productOwner = '';
    let developers = [];

    if(this.project) {
      name = this.project.name;
      this.currentProjectUsers = this.project.users;
      scrumMaster = this.project.users.find(u => u.projectRoles.includes(ProjectRole['Scrum Master'])).id;
      productOwner = this.project.users.find(u => u.projectRoles.includes(ProjectRole['Product Owner'])).id;
      developers = this.project.users.filter(u => u.projectRoles.includes(ProjectRole['Developer'])).map(x => x.id);
    }

    this.projectForm = this.formBuilder.group({
      name: [name, Validators.required],
      scrumMaster: [scrumMaster, Validators.required],
      productOwner: [productOwner, Validators.required],
      developers: this.formBuilder.array(developers)
    });

    this.userService.getUsers().subscribe(
      res => {
        this.users = res;
        this.usersCanAdd = [];
        this.users.forEach(val => {
          if(!developers.includes(val.id)) {
            this.usersCanAdd.push(Object.assign({}, val));
          }
        });
      },
      err => {
        this.error = err.error.message
      }
    )
  }
  get f() { return this.projectForm.controls; }
  get ProjectRole() { return ProjectRole; }

  ProjectUsersValidator(form: FormArray): { [key: string]: boolean } {
    let foundSM: number = 0;
    let foundPO: number = 0;
    form.controls.forEach(formGroup => {
      let projectRole = formGroup['controls'].projectRole.value;
      if(projectRole == ProjectRole['Scrum Master']) {
        foundSM++;
      }
      if(projectRole == ProjectRole['Product Owner']) {
        foundPO++;
      }
    });
    
    let returnObj = {};
    if(foundSM !== 1) {
      returnObj['noScrumMaster'] = true;
    }
    if(foundPO !== 1) {
      returnObj['noProductOwner'] = true;
    }
    if(Object.keys(returnObj).length > 0) {
      return returnObj;
    } else {
      return null;
    }
  }
  
  addDeveloper(userId: string) {
    if(userId) {
      let projectUsers: FormArray = this.projectForm.get('developers') as FormArray;
      projectUsers.push(new FormControl(userId));
      this.usersCanAdd = this.usersCanAdd.filter((user) => user.id !== userId);
    }
  }

  removeDeveloper(index: number) {
    let projectUsers: FormArray = this.projectForm.get('developers') as FormArray;
    let projectUserId = projectUsers.get(index.toString()).value;
    projectUsers.removeAt(index);
    this.usersCanAdd.push(this.users.find(user => user.id === projectUserId));
  }

  mapToUsername(userId: string):string {
    return this.users.find(user => user.id === userId).username;
  }

  onSubmit() {
    this.submitted = true;

    if (this.projectForm.invalid) {
      return;
    }
    
    let project: Project = new Project;
    project["projectUsers"] = [];
    project.name = this.f.name.value;
    project.users = [];
    this.f.developers.value.forEach(developer => {
      let projectUser: ProjectUser = new ProjectUser;
      projectUser.id = developer;
      projectUser.projectRole = ProjectRole['Developer'];
      project["projectUsers"].push(projectUser);
    });
    let productOwner: ProjectUser = new ProjectUser;
    productOwner.id = this.f.productOwner.value;
    productOwner.projectRole = ProjectRole['Product Owner'];
    project["projectUsers"].push(productOwner);
    let scrumMaster: ProjectUser = new ProjectUser;
    scrumMaster.id = this.f.scrumMaster.value;
    scrumMaster.projectRole = ProjectRole['Scrum Master'];
    project["projectUsers"].push(scrumMaster);

    this.loading = true;
    if(!this.project){
      this.service.addProject(project).subscribe(
        res => {
          this.activeModal.close(res);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
    } else {
      this.service.editProject(this.project.id, project).subscribe(
        res => { 
          this.activeModal.close(res);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
    }
    
  }
}
