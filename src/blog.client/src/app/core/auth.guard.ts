import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { map, Observable, tap } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root',
})
export class AuthGuard implements CanActivate {
    constructor(private _authService: AuthService, private _router: Router) { }

    public canActivate(route: ActivatedRouteSnapshot): Observable<boolean> | boolean {
        const expectedRoles: string[] = route.data ? route.data['roles'] ?? [] : [];
        if (!expectedRoles || expectedRoles.length === 0) {
            return true;
        }

        return this._authService.user$
            .pipe(map(user => {
                if (!user) return false

                return expectedRoles.some(role => user.roles.includes(role));
            }))
            .pipe(tap((result) => {
                if (!result) {
                    this._router.navigate(['/articles']);
                }
            }));
    }
}
