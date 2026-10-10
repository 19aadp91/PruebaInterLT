import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div style="max-width: 480px; margin: 2rem auto; padding: 2rem; background: white; border-radius: 12px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); border: 1px solid #e2e8f0;">
      <h2 style="font-size: 1.5rem; font-weight: 700; color: #0f172a; margin-bottom: 0.5rem; text-align: center;">Registro de Estudiante</h2>
      <p style="color: #64748b; font-size: 0.9rem; text-align: center; margin-bottom: 1.5rem;">Adhiérete al programa de créditos académicos en línea</p>

      @if (errorMessage()) {
        <div style="background: #fee2e2; border-left: 4px solid #ef4444; color: #b91c1c; padding: 0.75rem 1rem; border-radius: 6px; margin-bottom: 1rem; font-size: 0.9rem;">
          {{ errorMessage() }}
        </div>
      }

      <form (ngSubmit)="onSubmit()">
        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-size: 0.85rem; font-weight: 600; color: #334155; margin-bottom: 0.25rem;">Nombre Completo *</label>
          <input type="text" [(ngModel)]="fullName" name="fullName" required placeholder="Ej: Juan David Rodríguez"
                 style="width: 100%; box-sizing: border-box; padding: 0.65rem 0.75rem; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 0.95rem;" />
        </div>

        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-size: 0.85rem; font-weight: 600; color: #334155; margin-bottom: 0.25rem;">Documento de Identificación *</label>
          <input type="text" [(ngModel)]="identificationNumber" name="identificationNumber" required placeholder="Ej: 1020304050"
                 style="width: 100%; box-sizing: border-box; padding: 0.65rem 0.75rem; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 0.95rem;" />
        </div>

        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-size: 0.85rem; font-weight: 600; color: #334155; margin-bottom: 0.25rem;">Correo Electrónico Institucional *</label>
          <input type="email" [(ngModel)]="email" name="email" required placeholder="juan.rodriguez@universidad.edu.co"
                 style="width: 100%; box-sizing: border-box; padding: 0.65rem 0.75rem; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 0.95rem;" />
        </div>

        <div style="margin-bottom: 1.5rem;">
          <label style="display: block; font-size: 0.85rem; font-weight: 600; color: #334155; margin-bottom: 0.25rem;">Contraseña (mínimo 6 caracteres) *</label>
          <input type="password" [(ngModel)]="password" name="password" required minlength="6" placeholder="Crea una contraseña segura"
                 style="width: 100%; box-sizing: border-box; padding: 0.65rem 0.75rem; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 0.95rem;" />
        </div>

        <button type="submit" [disabled]="loading()"
                style="width: 100%; padding: 0.75rem; background: #ea580c; color: white; border: none; border-radius: 6px; font-weight: 600; font-size: 1rem; cursor: pointer;">
          {{ loading() ? 'Registrando...' : 'Completar Registro' }}
        </button>
      </form>

      <div style="margin-top: 1.5rem; text-align: center; font-size: 0.9rem; color: #64748b;">
        ¿Ya estás registrado?
        <a routerLink="/login" style="color: #ea580c; font-weight: 600; text-decoration: none;">Inicia sesión</a>
      </div>
    </div>
  `
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  fullName = '';
  identificationNumber = '';
  email = '';
  password = '';
  loading = signal(false);
  errorMessage = signal<string | null>(null);

  onSubmit() {
    this.errorMessage.set(null);
    this.loading.set(true);

    this.authService.register({
      fullName: this.fullName,
      identificationNumber: this.identificationNumber,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/catalog']);
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.detail || 'Error en el registro. Verifique sus datos.');
      }
    });
  }
}
