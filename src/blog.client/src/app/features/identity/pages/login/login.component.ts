import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { Client, ForgotPasswordRequest, LoginRequest } from 'src/app/core/services/api.generated';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    RouterModule,
    MatDividerModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnDestroy {
  public loginForm: FormGroup;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(private fb: FormBuilder, private _apiClient: Client) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public onSubmit(): void {
    if (!this.loginForm.valid) return;

    const request: LoginRequest = {
      email: this.loginForm.value.email,
      password: this.loginForm.value.password,
      twoFactorCode: undefined,
      twoFactorRecoveryCode: undefined
    };

    this._apiClient.postApiAccountLogin(true, true, request)
      .pipe(takeUntil(this._destroy$))
      .subscribe(() => {
        // eslint-disable-next-line no-console
        console.log('User logged in successfully');
      });
  }

  public onForgotPassword(): void {
    const request: ForgotPasswordRequest = {
      email: this.loginForm.value.email
    };

    this._apiClient.postApiAccountForgotPassword(request)
      .pipe(takeUntil(this._destroy$))
      .subscribe(() => {
        // eslint-disable-next-line no-console
        console.log('Link to reset password sent to email');
      });
  }
}
