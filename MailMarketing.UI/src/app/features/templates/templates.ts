import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { QuillModule } from 'ngx-quill';
import { finalize } from 'rxjs';
import { TemplateDto, TemplateService } from '../../core/services/template.service';

@Component({
  selector: 'app-templates',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, QuillModule],
  templateUrl: './templates.html',
  styleUrl: './templates.scss'
})
export class Templates implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly templateService = inject(TemplateService);

  protected readonly templates = signal<TemplateDto[]>([]);
  protected readonly isLoading = signal(false);
  protected readonly isSubmitting = signal(false);
  protected submitAttempted = false;
  protected readonly successMessage = signal('');
  protected readonly errorMessage = signal('');

  protected readonly templateForm = this.formBuilder.nonNullable.group({
    title: ['', [Validators.required]],
    content: ['', [Validators.required]]
  });

  protected readonly editorModules = {
    toolbar: [
      ['bold', 'italic', 'underline'],
      [{ header: [1, 2, 3, false] }],
      [{ list: 'ordered' }, { list: 'bullet' }],
      ['link'],
      ['clean']
    ]
  };

  ngOnInit(): void {
    this.loadTemplates();
  }

  protected addTemplate(): void {
    this.submitAttempted = true;
    this.successMessage.set('');
    this.errorMessage.set('');

    if (this.templateForm.invalid || this.isSubmitting()) {
      this.templateForm.markAllAsTouched();
      this.errorMessage.set('Başlık ve içerik alanları zorunludur.');
      return;
    }

    this.isSubmitting.set(true);

    this.templateService.create(this.templateForm.getRawValue()).subscribe({
      next: template => {
        this.templates.set([template, ...this.templates()]);
        this.templateForm.reset();
        this.submitAttempted = false;
        this.isSubmitting.set(false);
        this.successMessage.set('Şablon başarıyla kaydedildi.');
      },
      error: () => {
        this.isSubmitting.set(false);
        this.errorMessage.set('Şablon kaydedilemedi.');
      }
    });
  }

  protected deleteTemplate(template: TemplateDto): void {
    const confirmed = window.confirm('Kaydı silmek istediğinize emin misiniz?');

    if (!confirmed) {
      return;
    }

    this.templateService.delete(template.id).subscribe({
      next: () => {
        this.templates.set(this.templates().filter(item => item.id !== template.id));
      },
      error: error => {
        const message = error?.error?.message ?? error?.error ?? 'Şablon silinemedi. Daha önce mail gönderiminde kullanılmış olabilir.';
        window.alert(message);
      }
    });
  }

  protected toggleStatus(template: TemplateDto): void {
    const nextStatus = !template.isActive;

    this.templateService.update(template.id, {
      title: template.title,
      content: template.content,
      isActive: nextStatus
    }).subscribe({
      next: () => {
        this.templates.set(this.templates().map(item =>
          item.id === template.id ? { ...item, isActive: nextStatus } : item
        ));
      },
      error: () => {
        window.alert('Şablon durumu güncellenemedi.');
      }
    });
  }

  protected get showTitleError(): boolean {
    const control = this.templateForm.controls.title;

    return control.invalid && (control.touched || this.submitAttempted);
  }

  protected get showContentError(): boolean {
    const control = this.templateForm.controls.content;

    return control.invalid && (control.touched || this.submitAttempted);
  }

  private loadTemplates(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.templateService.getAll().pipe(
      finalize(() => {
        this.isLoading.set(false);
      })
    ).subscribe({
      next: templates => {
        this.templates.set(templates);
      },
      error: () => {
        this.errorMessage.set('Şablonlar yüklenemedi.');
      }
    });
  }
}
