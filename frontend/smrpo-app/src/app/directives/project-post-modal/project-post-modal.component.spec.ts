import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectPostModalComponent } from './project-post-modal.component';

describe('ProjectPostModalComponent', () => {
  let component: ProjectPostModalComponent;
  let fixture: ComponentFixture<ProjectPostModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProjectPostModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectPostModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
