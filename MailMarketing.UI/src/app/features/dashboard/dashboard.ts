import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  private readonly dashboardService = inject(DashboardService);
  private readonly authService = inject(AuthService);

  protected readonly totalSubscribers = signal(0);
  protected readonly totalTemplates = signal(0);
  protected readonly totalSentMails = signal(0);
  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly currentUser = signal('Kullanıcı');

  ngOnInit(): void {
    this.currentUser.set(this.authService.getCurrentUserDisplayName());
    this.loadStats();
  }

  private loadStats(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.dashboardService.getStats().subscribe({
      next: stats => {
        this.totalSubscribers.set(stats.totalSubscribers);
        this.totalTemplates.set(stats.totalTemplates);
        this.totalSentMails.set(stats.totalSentMails);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Dashboard istatistikleri yüklenemedi.');
      }
    });
  }
}
