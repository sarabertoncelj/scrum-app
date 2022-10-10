import { Component, OnInit, Input } from '@angular/core';
import { ProjectPost } from 'src/app/models/ProjectPost';

@Component({
  selector: 'project-post',
  templateUrl: './project-post.component.html',
  styleUrls: ['./project-post.component.css']
})
export class ProjectPostComponent implements OnInit {
  @Input() projectPost: ProjectPost;

  constructor() { }

  ngOnInit(): void {
  }

}
