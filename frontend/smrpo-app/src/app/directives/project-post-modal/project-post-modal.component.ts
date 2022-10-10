import { Component, OnInit } from '@angular/core';
import { Project } from 'src/app/models/Project';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ProjectService } from 'src/app/services/project.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ProjectPost } from 'src/app/models/ProjectPost';

@Component({
  selector: 'app-project-post-modal',
  templateUrl: './project-post-modal.component.html',
  styleUrls: ['./project-post-modal.component.css']
})
export class ProjectPostModalComponent implements OnInit {
  project: Project;
  projectPostForm: FormGroup;
  loading = false;
  submitted = false;

  constructor(
    private formBuilder: FormBuilder,
    private projectService: ProjectService,
    public activeModal: NgbActiveModal
  ) {}

  ngOnInit (): void {
    this.projectPostForm = this.formBuilder.group({
      title: ['', Validators.required],
      description: ['', Validators.required]
    });
  }

  get f () { return this.projectPostForm.controls; }

  onSubmit () {
    this.submitted = true;
    // stop here if form is invalid
    if (this.projectPostForm.invalid) {
      return;
    }

    //prepare
    let projectPost: ProjectPost = new ProjectPost();
    projectPost.title = this.f.title.value;
    projectPost.description = this.f.description.value;

    this.loading = true;
    this.projectService.addProjectPost(this.project.id, projectPost).subscribe(
        projectPosts => {
          this.activeModal.close(projectPosts);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
  }
}
