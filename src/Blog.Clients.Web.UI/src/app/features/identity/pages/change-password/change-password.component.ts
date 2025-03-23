import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from 'src/app/core/auth.service';

@Component({
  selector: 'app-change-password',
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
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.scss'
})
export class ChangePasswordComponent implements OnDestroy {
  public resetForm: FormGroup;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private _snakBar: MatSnackBar,
    private _authService: AuthService) {

    this.resetForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      code: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public onSubmit(): void {
    if (!this.resetForm.valid) return;

    this._authService.changePassword(this.resetForm.value.email, this.resetForm.value.password, this.resetForm.value.code)
      .pipe(takeUntil(this._destroy$))
      .subscribe({
        next: () => {
          this._snakBar.open('Password was changed!', 'Close', {
            duration: 5000,
            panelClass: ['notification-success']
          });
        },
        error: (error) => {
          let msg = 'Failed to set new password';
          if (error.errors) {
            const errors = Object.values(error.errors).join('\n');
            msg = `${errors}`;
          }

          this._snakBar.open(msg, 'Close', {
            duration: 5000,
            panelClass: ['notification-error']
          });
        }
      });
  }
}