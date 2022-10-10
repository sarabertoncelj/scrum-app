import { ProjectRole } from '../enums/ProjectRole';
import { ProjectUser } from './ProjectUser';

export class Project {
    id: string;
    name: string;
    projectRoles: ProjectRole[];
    users: ProjectUser[];
}