import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserStoryModalComponent } from './user-story-modal.component';

describe('UserStoryModalComponent', () => {
  let component: UserStoryModalComponent;
  let fixture: ComponentFixture<UserStoryModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserStoryModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserStoryModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
