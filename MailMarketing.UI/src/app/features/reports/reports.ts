import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MailLogDto, ReportService } from '../../core/services/report.service';
import { TemplateDto, TemplateService } from '../../core/services/template.service';

@Component({
  selector: 'app-reports',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './reports.html',
  styleUrl: './reports.scss'
})
export class Reports implements OnInit {
  private readonly templateService = inject(TemplateService);
  private readonly reportService = inject(ReportService);

  protected readonly templates = signal<TemplateDto[]>([]);
  protected readonly reports = signal<MailLogDto[]>([]);
  protected readonly isLoadingTemplates = signal(false);
  protected readonly isLoadingReports = signal(false);
  protected readonly errorMessage = signal('');
  protected selectedTemplateId: number | null = null;
  protected startDate = '';
  protected endDate = '';
  protected status = '';

  ngOnInit(): void {
    this.loadTemplates();
  }

  protected loadReports(): void {
    this.errorMessage.set('');

    if (!this.selectedTemplateId) {
      this.errorMessage.set('Lütfen şablon seçin.');
      return;
    }

    this.isLoadingReports.set(true);

    this.reportService.getFilteredLogs({
      templateId: this.selectedTemplateId,
      startDate: this.startDate,
      endDate: this.endDate,
      status: this.status
    }).subscribe({
      next: reports => {
        this.reports.set(reports);
        this.isLoadingReports.set(false);
      },
      error: () => {
        this.isLoadingReports.set(false);
        this.errorMessage.set('Rapor verileri yüklenemedi.');
      }
    });
  }

  private loadTemplates(): void {
    this.isLoadingTemplates.set(true);

    this.templateService.getAll().subscribe({
      next: templates => {
        this.templates.set(templates);
        this.isLoadingTemplates.set(false);
      },
      error: () => {
        this.isLoadingTemplates.set(false);
        this.errorMessage.set('Şablonlar yüklenemedi.');
      }
    });
  }
}
