import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { UserListDto, UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-users',
  imports: [CommonModule],
  templateUrl: './users.html',
  styleUrl: './users.scss'
})
export class Users implements OnInit {
  private readonly userService = inject(UserService);

  protected readonly users = signal<UserListDto[]>([]);
  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal('');
  protected readonly successMessage = signal('');

  ngOnInit(): void {
    this.loadUsers();
  }

  protected deactivate(user: UserListDto): void {
    const confirmed = window.confirm('Kullanıcıyı pasife almak istediğinize emin misiniz?');

    if (!confirmed) {
      return;
    }

    this.successMessage.set('');
    this.errorMessage.set('');

    this.userService.deactivateUser(user.id).subscribe({
      next: () => {
        this.users.set(this.users().map(item =>
          item.id === user.id ? { ...item, isActive: false } : item
        ));
        this.successMessage.set('Kullanıcı pasife alındı.');
      },
      error: error => {
        this.errorMessage.set(error?.error || 'Kullanıcı pasife alınamadı.');
      }
    });
  }

  private loadUsers(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.userService.getUsers().subscribe({
      next: users => {
        this.users.set(users);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Kullanıcılar yüklenemedi.');
      }
    });
  }
}
