import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SubscriberDto, SubscriberService } from '../../core/services/subscriber.service';

@Component({
  selector: 'app-subscribers',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './subscribers.html',
  styleUrl: './subscribers.scss'
})
export class Subscribers implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly subscriberService = inject(SubscriberService);

  protected readonly subscribers = signal<SubscriberDto[]>([]);
  protected readonly filteredSubscribers = signal<SubscriberDto[]>([]);
  protected emailFilter = '';
  protected dateFilter = '';
  protected readonly isLoading = signal(false);
  protected readonly isSubmitting = signal(false);
  protected submitAttempted = false;
  protected readonly successMessage = signal('');
  protected readonly errorMessage = signal('');

  protected readonly subscriberForm = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]]
  });

  ngOnInit(): void {
    this.loadSubscribers();
  }

  protected filterSubscribers(): void {
    const normalizedEmail = this.emailFilter.trim().toLowerCase();

    this.filteredSubscribers.set(this.subscribers().filter(subscriber => {
      const matchesEmail = !normalizedEmail || subscriber.email.toLowerCase().includes(normalizedEmail);
      const matchesDate = !this.dateFilter || subscriber.createdAt.slice(0, 10) === this.dateFilter;

      return matchesEmail && matchesDate;
    }));
  }

  protected addSubscriber(): void {
    this.submitAttempted = true;
    this.successMessage.set('');
    this.errorMessage.set('');

    if (this.subscriberForm.invalid || this.isSubmitting()) {
      this.subscriberForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    this.subscriberService.create(this.subscriberForm.getRawValue()).subscribe({
      next: subscriber => {
        this.subscribers.set([subscriber, ...this.subscribers()]);
        this.filterSubscribers();
        this.subscriberForm.reset();
        this.submitAttempted = false;
        this.isSubmitting.set(false);
        this.successMessage.set('Başarıyla kaydedildi.');
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('Abone kaydedilemedi. E-posta adresi daha önce eklenmiş olabilir.');
      }
    });
  }

  protected deleteSubscriber(subscriber: SubscriberDto): void {
    const confirmed = window.confirm('Kaydı silmek istediğinize emin misiniz?');

    if (!confirmed) {
      return;
    }

    this.subscriberService.delete(subscriber.id).subscribe({
      next: () => {
        this.subscribers.set(this.subscribers().filter(item => item.id !== subscriber.id));
        this.filterSubscribers();
      },
      error: error => {
        const message = error?.error?.message ?? error?.error ?? 'Kayıt silinemedi. Bu aboneye daha önce mail atılmış olabilir.';
        window.alert(message);
      }
    });
  }

  protected get showEmailError(): boolean {
    const control = this.subscriberForm.controls.email;

    return control.invalid && (control.touched || this.submitAttempted);
  }

  private loadSubscribers(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.subscriberService.getAll().subscribe({
      next: subscribers => {
        this.subscribers.set(subscribers);
        this.filterSubscribers();
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Aboneler yüklenemedi.');
      }
    });
  }
}
