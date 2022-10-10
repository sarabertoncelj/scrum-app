import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';
import { UserRole } from '../enums/UserRole';

@Injectable()
export class AdminGuard implements CanActivate {

    constructor(
        public auth: AuthenticationService,
        public router: Router
    ) { }

    canActivate (): boolean {
        const user = this.auth.currentUserValue;
        if (user && user.role === UserRole.Administrator) {
            return true;
        }
        this.router.navigate(['/']);
        return false;
    }
}
