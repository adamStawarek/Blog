import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { Subject, takeUntil } from 'rxjs';
import { Client, RegisterRequest } from 'src/app/core/services/api.generated';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnDestroy {
  public registerForm: FormGroup;

  private _destroy$: Subject<void> = new Subject<void>();

  constructor(private fb: FormBuilder, private _apiClient: Client) {
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

    const request: RegisterRequest = {
      email: this.registerForm.value.email,
      password: this.registerForm.value.password
    };

    this._apiClient.postApiAccountRegister(request)
      .pipe(takeUntil(this._destroy$))
      .subscribe(() => {
        // eslint-disable-next-line no-console
        console.log('User registered successfully');
      });
  }
}
