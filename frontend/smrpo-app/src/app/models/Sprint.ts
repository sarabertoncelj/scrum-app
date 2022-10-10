import { UserStory } from './UserStory';

export class Sprint {
    id: string;
    start: Date;
    end: Date;
    velocity: number;
    active: boolean;

    projectId: string;

    userStories: UserStory[];
}
