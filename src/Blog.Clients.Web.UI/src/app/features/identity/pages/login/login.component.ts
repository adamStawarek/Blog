import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
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
    private _fb: FormBuilder,
    private _snakBar: MatSnackBar,
    private _router: Router,
    private _route: ActivatedRoute,
    private _authService: AuthService) {

    this.loginForm = this._fb.group({
      userName: ['', [Validators.required]],
      password: ['', Validators.required]
    });
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public onSubmit(): void {
    if (!this.loginForm.valid) return;

    this._authService.login(this.loginForm.value.userName, this.loginForm.value.password)
      .pipe(takeUntil(this._destroy$))
      .subscribe({
        next: (user) => {
          if (!user) return;

          const returnUrl = this._route.snapshot.queryParamMap.get('returnUrl') ?? '/';
          this._router.navigate([returnUrl]);
        },
        error: () => {
          this._snakBar.open('Login failed', 'Close', {
            duration: 5000,
            panelClass: ['notification-error']
          })
        }
      });
  }

  public onCancel(): void {
    this._authService.reset();
    this._router.navigate(['/']);
  }

  public onForgotPassword(): void {
    this._router.navigate(['/identity/forgot-password']);
  }
}
