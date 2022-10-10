import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectWallComponent } from './project-wall.component';

describe('ProjectWallComponent', () => {
  let component: ProjectWallComponent;
  let fixture: ComponentFixture<ProjectWallComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProjectWallComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectWallComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
