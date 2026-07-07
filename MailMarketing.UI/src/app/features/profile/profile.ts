import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../core/services/user.service';

const passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

@Component({
  selector: 'app-profile',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class Profile implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly userService = inject(UserService);

  protected readonly isLoading = signal(false);
  protected readonly isSubmitting = signal(false);
  protected readonly successMessage = signal('');
  protected readonly errorMessage = signal('');
  protected submitAttempted = false;

  protected readonly profileForm = this.formBuilder.nonNullable.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.pattern(passwordPattern)]]
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  protected saveProfile(): void {
    this.submitAttempted = true;
    this.successMessage.set('');
    this.errorMessage.set('');

    if (this.profileForm.invalid || this.isSubmitting()) {
      this.profileForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    this.userService.updateProfile(this.profileForm.getRawValue()).subscribe({
      next: () => {
        this.profileForm.controls.password.reset();
        this.submitAttempted = false;
        this.isSubmitting.set(false);
        this.successMessage.set('Profil başarıyla güncellendi.');
      },
      error: error => {
        this.isSubmitting.set(false);
        this.errorMessage.set(error?.error || 'Profil güncellenemedi.');
      }
    });
  }

  protected hasError(controlName: keyof typeof this.profileForm.controls): boolean {
    const control = this.profileForm.controls[controlName];

    return control.invalid && (control.touched || this.submitAttempted);
  }

  private loadProfile(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.userService.getProfile().subscribe({
      next: profile => {
        this.profileForm.patchValue({
          firstName: profile.firstName,
          lastName: profile.lastName,
          email: profile.email,
          password: ''
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Profil bilgileri yüklenemedi.');
      }
    });
  }
}
