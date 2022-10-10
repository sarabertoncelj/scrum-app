import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserStoryTaskModalComponent } from './user-story-task-modal.component';

describe('UserStoryTaskModalComponent', () => {
  let component: UserStoryTaskModalComponent;
  let fixture: ComponentFixture<UserStoryTaskModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserStoryTaskModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserStoryTaskModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
