import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { SubscriberService } from '../../core/services/subscriber.service';

@Component({
  selector: 'app-landing',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './landing.html',
  styleUrl: './landing.scss'
})
export class Landing {
  private readonly formBuilder = inject(FormBuilder);
  private readonly subscriberService = inject(SubscriberService);

  protected readonly isSubmitting = signal(false);
  protected readonly successMessage = signal('');
  protected readonly errorMessage = signal('');
  protected submitAttempted = false;

  protected readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]]
  });

  protected subscribe(): void {
    this.submitAttempted = true;
    this.successMessage.set('');
    this.errorMessage.set('');

    if (this.form.invalid || this.isSubmitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    this.subscriberService.subscribePublic(this.form.controls.email.value).subscribe({
      next: () => {
        this.form.reset();
        this.submitAttempted = false;
        this.isSubmitting.set(false);
        this.successMessage.set('Kaydedildi. Bülten listemize başarıyla katıldınız.');
      },
      error: error => {
        this.isSubmitting.set(false);
        this.errorMessage.set(error?.error || 'Kayıt tamamlanamadı.');
      }
    });
  }

  protected get showEmailError(): boolean {
    const control = this.form.controls.email;

    return control.invalid && (control.touched || this.submitAttempted);
  }
}
