import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Client } from './api.generated';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    public isAuthenticated$: Observable<boolean>;

    private _isAuthenticatedSubject = new BehaviorSubject<boolean>(false);

    constructor(private _apiClient: Client) {
        this.isAuthenticated$ = this._isAuthenticatedSubject.asObservable();
    }

    // public login(): void {
    //     const request: LoginRequest = {
    //         email: this.loginForm.value.email,
    //         password: this.loginForm.value.password,
    //         twoFactorCode: undefined,
    //         twoFactorRecoveryCode: undefined
    //     };


    //     this._apiClient.postAccountLogin(true, true, {
    //     });
    // }

    // public hideLoader(): void {
    //     this._activeRequestsCount--;
    //     if (this._activeRequestsCount === 0) {
    //         this._isAuthenticatedSubject.next(false);
    //     }
    // }
}