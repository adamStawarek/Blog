import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from 'src/app/core/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    RouterModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnDestroy {
  public registerForm: FormGroup;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private _snakBar: MatSnackBar,
    private _router: Router,
    private _authService: AuthService) {

    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public onSubmit(): void {
    if (!this.registerForm.valid) return;

    this._authService.register(this.registerForm.value.email, this.registerForm.value.password)
      .pipe(takeUntil(this._destroy$))
      .subscribe({
        next: () => {
          this._snakBar.open('Confirmation link was sent to provided email', 'Close', {
            duration: 5000,
            panelClass: ['notification-success']
          });

          this._router.navigate(['/identity/login']);
        },
        error: (error) => {
          let msg = 'Failed to register account';
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
