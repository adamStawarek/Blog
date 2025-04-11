import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { map, switchMap, tap } from 'rxjs/operators';
import { Client, CreateAccountRequest, LoginRequest, ResetPasswordRequest } from './api.generated';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    public isAuthenticated$: Observable<boolean>;
    public isAnonymous$: Observable<boolean>;
    public isAdmin$: Observable<boolean>;
    public user$: Observable<User>;

    private _isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
    private _userSubject = new BehaviorSubject<User>(undefined!);

    constructor(private _apiClient: Client) {
        this.isAuthenticated$ = this._isAuthenticatedSubject.asObservable();
        this.user$ = this._userSubject.asObservable();

        this.isAdmin$ = this.user$.pipe(map(user => user?.roles.includes('Admin')));
        this.isAnonymous$ = this.isAuthenticated$.pipe(map(isAuthenticated => !isAuthenticated));
    }

    public init(): void {
        this._apiClient.getAccountInfo()
            .subscribe({
                next: accountInfo => {
                    this._isAuthenticatedSubject.next(true);
                    this._userSubject.next({
                        id: accountInfo.id,
                        email: accountInfo.userName,
                        roles: accountInfo.roles
                    });
                },
                error: () => {
                    this._isAuthenticatedSubject.next(false);
                    this._userSubject.next(undefined!);
                }
            });
    }

    public register(userName: string, email: string, password: string): Observable<void> {
        const request: CreateAccountRequest = {
            userName: userName,
            email: email,
            password: password
        };

        return this._apiClient.createAccount(request);
    }

    public login(email: string, password: string): Observable<User> {
        const request: LoginRequest = {
            email: email,
            password: password,
            twoFactorCode: undefined,
            twoFactorRecoveryCode: undefined
        };

        return this._apiClient.postAccountLogin(true, true, request)
            .pipe(
                tap(() => {
                    this._isAuthenticatedSubject.next(true);
                }),
                switchMap(() => this._apiClient.getAccountInfo()),
                map(accountInfo => {
                    const user: User = {
                        id: accountInfo.id,
                        email: accountInfo.userName,
                        roles: accountInfo.roles
                    };
                    this._userSubject.next(user);
                    return user;
                }),
                tap({
                    error: () => {
                        this._isAuthenticatedSubject.next(false);
                        this._userSubject.next(undefined!);
                    }
                })
            );
    }

    public forgotPassword(email: string): Observable<void> {
        return this._apiClient.postAccountForgotPassword({
            email: email
        });
    }

    public changePassword(email: string, password: string, code: string): Observable<void> {
        const request: ResetPasswordRequest = {
            resetCode: code,
            email: email,
            newPassword: password
        }
        return this._apiClient.postAccountResetPassword(request);
    }
}

export interface User {
    id: string;
    email: string;
    roles: string[];
}
