import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { EmailConfigService } from '../../core/services/email-config.service';

@Component({
  selector: 'app-email-config',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './email-config.html',
  styleUrl: './email-config.scss'
})
export class EmailConfig implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly emailConfigService = inject(EmailConfigService);

  protected readonly isLoading = signal(false);
  protected readonly isSubmitting = signal(false);
  protected readonly successMessage = signal('');
  protected readonly errorMessage = signal('');
  protected submitAttempted = false;

  protected readonly configForm = this.formBuilder.nonNullable.group({
    mailServer: ['', [Validators.required]],
    smtpPort: [1025, [Validators.required]],
    useSsl: [false, [Validators.required]],
    emailAddress: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  ngOnInit(): void {
    this.loadConfig();
  }

  protected onSubmit(): void {
    this.submitAttempted = true;
    this.successMessage.set('');
    this.errorMessage.set('');

    if (this.configForm.invalid || this.isSubmitting()) {
      this.configForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    this.emailConfigService.createOrUpdateConfig(this.configForm.getRawValue()).subscribe({
      next: config => {
        this.configForm.patchValue({
          mailServer: config.mailServer,
          smtpPort: config.smtpPort,
          useSsl: config.useSsl,
          emailAddress: config.emailAddress
        });
        this.isSubmitting.set(false);
        this.successMessage.set('Ayarlar başarıyla kaydedildi.');
        window.alert('Ayarlar başarıyla kaydedildi');
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('Ayarlar kaydedilemedi.');
      }
    });
  }

  protected hasError(controlName: keyof typeof this.configForm.controls): boolean {
    const control = this.configForm.controls[controlName];

    return control.invalid && (control.touched || this.submitAttempted);
  }

  private loadConfig(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.emailConfigService.getConfig().subscribe({
      next: config => {
        this.configForm.patchValue({
          mailServer: config.mailServer,
          smtpPort: config.smtpPort,
          useSsl: config.useSsl,
          emailAddress: config.emailAddress
        });
        this.isLoading.set(false);
      },
      error: error => {
        this.isLoading.set(false);

        if (error?.status !== 404) {
          this.errorMessage.set('SMTP ayarları yüklenemedi.');
        }
      }
    });
  }
}
