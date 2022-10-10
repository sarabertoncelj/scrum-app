import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FutureReleasesComponent } from './future-releases.component';

describe('FutureReleasesComponent', () => {
  let component: FutureReleasesComponent;
  let fixture: ComponentFixture<FutureReleasesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FutureReleasesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FutureReleasesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
