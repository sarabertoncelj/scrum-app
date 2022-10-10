import { ProjectRole } from './../enums/ProjectRole';
import { UserRole } from '../enums/UserRole';

export class User {
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    role: UserRole;
    password: string;
    token: string;
    id: string;
    lastLogin: Date;
    projectRoles: ProjectRole[];
}
