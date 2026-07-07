import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { CampaignService } from '../../core/services/campaign.service';
import { SubscriberDto, SubscriberService } from '../../core/services/subscriber.service';
import { TemplateDto, TemplateService } from '../../core/services/template.service';

@Component({
  selector: 'app-campaigns',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './campaigns.html',
  styleUrl: './campaigns.scss'
})
export class Campaigns implements OnInit {
  private readonly subscriberService = inject(SubscriberService);
  private readonly templateService = inject(TemplateService);
  private readonly campaignService = inject(CampaignService);

  protected readonly subscribers = signal<SubscriberDto[]>([]);
  protected readonly activeTemplates = signal<TemplateDto[]>([]);
  protected readonly isLoading = signal(false);
  protected readonly isSending = signal(false);
  protected readonly successMessage = signal('');
  protected readonly errorMessage = signal('');
  protected selectedTemplateId: number | null = null;
  protected selectedSubscriberIds = new Set<number>();

  ngOnInit(): void {
    this.loadPageData();
  }

  protected toggleSubscriberSelection(id: number, checked: boolean): void {
    if (checked) {
      this.selectedSubscriberIds.add(id);
    } else {
      this.selectedSubscriberIds.delete(id);
    }
  }

  protected toggleAllSubscribers(): void {
    if (this.areAllSubscribersSelected()) {
      this.selectedSubscriberIds.clear();
      return;
    }

    this.selectedSubscriberIds = new Set(this.subscribers().map(subscriber => subscriber.id));
  }

  protected areAllSubscribersSelected(): boolean {
    return this.subscribers().length > 0 && this.selectedSubscriberIds.size === this.subscribers().length;
  }

  protected isSubscriberSelected(id: number): boolean {
    return this.selectedSubscriberIds.has(id);
  }

  protected sendCampaign(): void {
    this.successMessage.set('');
    this.errorMessage.set('');

    if (!this.selectedTemplateId || this.selectedSubscriberIds.size === 0) {
      this.errorMessage.set('Lütfen şablon ve abone seçin.');
      return;
    }

    const confirmed = window.confirm('Gönderim yapmak istediğinize emin misiniz?');

    if (!confirmed) {
      return;
    }

    this.isSending.set(true);

    this.campaignService.sendCampaign({
      templateId: this.selectedTemplateId,
      subscriberIds: Array.from(this.selectedSubscriberIds)
    }).subscribe({
      next: () => {
        this.selectedTemplateId = null;
        this.selectedSubscriberIds.clear();
        this.isSending.set(false);
        this.successMessage.set('Gönderim başarıyla kuyruğa alındı.');
      },
      error: () => {
        this.isSending.set(false);
        this.errorMessage.set('Gönderim başlatılamadı.');
      }
    });
  }

  private loadPageData(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    forkJoin({
      subscribers: this.subscriberService.getAll(),
      templates: this.templateService.getAll()
    }).subscribe({
      next: ({ subscribers, templates }) => {
        this.subscribers.set(subscribers);
        this.activeTemplates.set(templates.filter(template => template.isActive));
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Kampanya verileri yüklenemedi.');
      }
    });
  }
}
