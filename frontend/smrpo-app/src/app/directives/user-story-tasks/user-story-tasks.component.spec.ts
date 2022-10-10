import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserStoryTasksComponent } from './user-story-tasks.component';

describe('UserStoryTasksComponent', () => {
  let component: UserStoryTasksComponent;
  let fixture: ComponentFixture<UserStoryTasksComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserStoryTasksComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserStoryTasksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
