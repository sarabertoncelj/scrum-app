import { User } from './User';
import { UserStoryTaskStatus } from '../enums/UserStoryTaskStatus';

export class UserStoryTask {
    id: string;
    description: string;
    remainingTime: number;
    activeFrom: Date;
    accepted: boolean;
    status: UserStoryTaskStatus;
    user: User;
}