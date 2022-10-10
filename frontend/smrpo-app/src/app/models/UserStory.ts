import { UserStoryPriority } from '../enums/UserStoryPriority';
import { UserStoryStatus } from '../enums/UserStoryStatus';
import { Sprint } from './Sprint';

export class UserStory {
    id: string;
    name: string;
    description: string;
    priority: UserStoryPriority;
    businessValue: number;
    comment: string;
    acceptanceTests: string[];
    status: UserStoryStatus;
    sprint: Sprint;
}