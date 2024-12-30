import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from 'src/app/core/auth.service';

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
    MatButtonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnDestroy {
  public loginForm: FormGroup;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private _snakBar: MatSnackBar,
    private _router: Router,
    private _authService: AuthService) {

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

    this._authService.login(this.loginForm.value.email, this.loginForm.value.password)
      .pipe(takeUntil(this._destroy$))
      .subscribe({
        next: (user) => {
          if (!user) return;

          this._router.navigate(['/']);
        },
        error: () => {
          this._snakBar.open('Login failed', 'Close', {
            duration: 5000,
            panelClass: ['notification-error']
          })
        }
      });
  }

  public onForgotPassword(): void {
    this._router.navigate(['/identity/forgot-password']);
  }
}
