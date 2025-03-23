import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from 'src/app/core/auth.service';

@Component({
  selector: 'app-forgot-password',
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
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss'
})
export class ForgotPasswordComponent implements OnDestroy {
  public resetForm: FormGroup;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private _router: Router,
    private _snakBar: MatSnackBar,
    private _authService: AuthService) {

    this.resetForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public onSubmit(): void {
    if (!this.resetForm.valid) return;

    this._authService.forgotPassword(this.resetForm.value.email)
      .pipe(takeUntil(this._destroy$))
      .subscribe(() => {
        this._snakBar.open('Password reset email was sent', 'Close', {
          duration: 5000,
          panelClass: ['notification-success']
        });

        this._router.navigate(['/identity/change-password']);
      });
  }
}