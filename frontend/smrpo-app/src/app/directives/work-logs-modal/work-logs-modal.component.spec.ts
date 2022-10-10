import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkLogsModalComponent } from './work-logs-modal.component';

describe('WorkLogsModalComponent', () => {
  let component: WorkLogsModalComponent;
  let fixture: ComponentFixture<WorkLogsModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkLogsModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkLogsModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
