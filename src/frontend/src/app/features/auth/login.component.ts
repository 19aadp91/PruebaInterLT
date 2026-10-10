import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div style="max-width: 420px; margin: 3rem auto; padding: 2rem; background: white; border-radius: 12px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); border: 1px solid #e2e8f0;">
      <h2 style="font-size: 1.5rem; font-weight: 700; color: #0f172a; margin-bottom: 0.5rem; text-align: center;">Iniciar Sesión</h2>
      <p style="color: #64748b; font-size: 0.9rem; text-align: center; margin-bottom: 1.5rem;">Ingresa a tu cuenta de estudiante o administrador</p>

      @if (errorMessage()) {
        <div style="background: #fee2e2; border-left: 4px solid #ef4444; color: #b91c1c; padding: 0.75rem 1rem; border-radius: 6px; margin-bottom: 1rem; font-size: 0.9rem;">
          {{ errorMessage() }}
        </div>
      }

      <form (ngSubmit)="onSubmit()">
        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-size: 0.85rem; font-weight: 600; color: #334155; margin-bottom: 0.25rem;">Correo Electrónico</label>
          <input type="email" [(ngModel)]="email" name="email" required placeholder="tu.correo@universidad.edu.co"
                 style="width: 100%; box-sizing: border-box; padding: 0.65rem 0.75rem; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 0.95rem;" />
        </div>

        <div style="margin-bottom: 1.5rem;">
          <label style="display: block; font-size: 0.85rem; font-weight: 600; color: #334155; margin-bottom: 0.25rem;">Contraseña</label>
          <input type="password" [(ngModel)]="password" name="password" required placeholder="Tu contraseña"
                 style="width: 100%; box-sizing: border-box; padding: 0.65rem 0.75rem; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 0.95rem;" />
        </div>

        <button type="submit" [disabled]="loading()"
                style="width: 100%; padding: 0.75rem; background: #ea580c; color: white; border: none; border-radius: 6px; font-weight: 600; font-size: 1rem; cursor: pointer;">
          {{ loading() ? 'Ingresando...' : 'Iniciar Sesión' }}
        </button>
      </form>

      <div style="margin-top: 1.5rem; text-align: center; font-size: 0.9rem; color: #64748b;">
        ¿Aún no tienes cuenta?
        <a routerLink="/register" style="color: #ea580c; font-weight: 600; text-decoration: none;">Regístrate aquí</a>
      </div>
      
      <div style="margin-top: 1rem; padding-top: 1rem; border-top: 1px solid #f1f5f9; font-size: 0.8rem; color: #94a3b8; text-align: center;">
        Admin demo: <code>admin&#64;universidad.edu.co</code> / <code>Admin123!</code>
      </div>
    </div>
  `
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  email = '';
  password = '';
  loading = signal(false);
  errorMessage = signal<string | null>(null);

  onSubmit() {
    this.errorMessage.set(null);
    this.loading.set(true);

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/catalog']);
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.detail || 'Credenciales de acceso inválidas.');
      }
    });
  }
}
