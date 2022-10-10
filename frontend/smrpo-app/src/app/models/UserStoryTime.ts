import { UserStoryPriority } from '../enums/UserStoryPriority';
import { UserStoryStatus } from '../enums/UserStoryStatus';
import { Sprint } from './Sprint';

export class UserStoryTime {
    estimation: number;
    sprint: Sprint;
}