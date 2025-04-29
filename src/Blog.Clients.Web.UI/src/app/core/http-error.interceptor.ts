import {
    HttpErrorResponse,
    HttpEvent,
    HttpHandler,
    HttpInterceptor,
    HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(
        private readonly router: Router,
        private readonly authService: AuthService) { }

    public intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        return next.handle(req).pipe(
            catchError((error: HttpErrorResponse) => {
                if (error.status === 401 && this.authService.isLoggedIn()) {
                    this.authService.reset();
                    const returnUrl = this.router.url;
                    this.router.navigate(['/identity/login'], { queryParams: { returnUrl } });
                }
                return throwError(() => error);
            })
        );
    }
}