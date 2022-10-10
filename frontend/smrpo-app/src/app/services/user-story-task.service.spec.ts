import { TestBed } from '@angular/core/testing';

import { UserStoryTaskService } from './user-story-task.service';

describe('UserStoryTaskService', () => {
  let service: UserStoryTaskService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserStoryTaskService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
