import { User } from './User';
import { ProjectRole } from '../enums/ProjectRole';

export class ProjectUser {
    username: string;
    email: string;

    firstName: string;
    lastName: string;

    id: string;
    projectRoles: ProjectRole[];
    projectRole: ProjectRole;
}