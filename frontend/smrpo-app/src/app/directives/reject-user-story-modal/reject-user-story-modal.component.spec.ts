import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RejectUserStoryModalComponent } from './reject-user-story-modal.component';

describe('RejectUserStoryModalComponent', () => {
  let component: RejectUserStoryModalComponent;
  let fixture: ComponentFixture<RejectUserStoryModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RejectUserStoryModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RejectUserStoryModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
