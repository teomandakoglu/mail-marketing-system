import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

const passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

@Component({
  selector: 'app-forgot-password',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.scss'
})
export class ForgotPassword {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);

  protected readonly step = signal<1 | 2>(1);
  protected readonly isSubmitting = signal(false);
  protected readonly successMessage = signal('');
  protected readonly errorMessage = signal('');
  protected emailSubmitted = false;
  protected passwordSubmitted = false;

  protected readonly emailForm = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]]
  });

  protected readonly passwordForm = this.formBuilder.nonNullable.group({
    password: ['', [Validators.required, Validators.pattern(passwordPattern)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: this.passwordsMatchValidator });

  protected verifyEmail(): void {
    this.emailSubmitted = true;
    this.successMessage.set('');
    this.errorMessage.set('');

    if (this.emailForm.invalid || this.isSubmitting()) {
      this.emailForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    this.authService.checkEmail(this.emailForm.controls.email.value).subscribe({
      next: () => {
        this.step.set(2);
        this.isSubmitting.set(false);
      },
      error: error => {
        this.isSubmitting.set(false);
        this.errorMessage.set(error?.error || 'Kullanıcı bulunamadı');
      }
    });
  }

  protected resetPassword(): void {
    this.passwordSubmitted = true;
    this.successMessage.set('');
    this.errorMessage.set('');

    if (this.passwordForm.invalid || this.isSubmitting()) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    this.authService.resetPassword({
      email: this.emailForm.controls.email.value,
      newPassword: this.passwordForm.controls.password.value
    }).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.passwordForm.reset();
        this.successMessage.set('Parolanız başarıyla güncellendi.');
      },
      error: error => {
        this.isSubmitting.set(false);
        this.errorMessage.set(error?.error || 'Parola güncellenemedi.');
      }
    });
  }

  protected hasEmailError(): boolean {
    const control = this.emailForm.controls.email;

    return control.invalid && (control.touched || this.emailSubmitted);
  }

  protected hasPasswordError(): boolean {
    const control = this.passwordForm.controls.password;

    return control.invalid && (control.touched || this.passwordSubmitted);
  }

  protected hasConfirmPasswordError(): boolean {
    const control = this.passwordForm.controls.confirmPassword;

    return (control.invalid || this.passwordForm.hasError('passwordMismatch')) && (control.touched || this.passwordSubmitted);
  }

  private passwordsMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    return password && confirmPassword && password !== confirmPassword ? { passwordMismatch: true } : null;
  }
}
