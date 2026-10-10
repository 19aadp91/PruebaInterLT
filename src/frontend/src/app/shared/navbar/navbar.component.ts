import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { AcademicService } from '../../core/services/academic.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  template: `
    <header style="background: #1e293b; color: white; padding: 1rem 2rem; box-shadow: 0 2px 4px rgba(0,0,0,0.1);">
      <div style="max-width: 1200px; margin: 0 auto; display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 1rem;">
        <div style="display: flex; align-items: center; gap: 0.75rem;">
          <div style="background: #f97316; width: 36px; height: 36px; border-radius: 8px; display: flex; align-items: center; justify-content: center; font-weight: bold; font-size: 1.25rem;">
            IR
          </div>
          <span style="font-size: 1.2rem; font-weight: 700; letter-spacing: -0.5px;">Registro Académico</span>
        </div>

        @if (authService.isAuthenticated()) {
          <nav style="display: flex; gap: 1rem; align-items: center;">
            <a routerLink="/catalog" routerLinkActive="active-nav" style="color: #cbd5e1; text-decoration: none; font-weight: 500; padding: 0.5rem 0.75rem; border-radius: 6px;">Catálogo</a>
            <a routerLink="/my-enrollments" routerLinkActive="active-nav" style="color: #cbd5e1; text-decoration: none; font-weight: 500; padding: 0.5rem 0.75rem; border-radius: 6px;">
              Mis Materias
              @if (academicService.myEnrollments(); as summary) {
                <span style="background: #ea580c; color: white; padding: 0.15rem 0.5rem; border-radius: 12px; font-size: 0.75rem; margin-left: 0.35rem;">
                  {{ summary.totalCredits }}/9 CR
                </span>
              }
            </a>
            <a routerLink="/students" routerLinkActive="active-nav" style="color: #cbd5e1; text-decoration: none; font-weight: 500; padding: 0.5rem 0.75rem; border-radius: 6px;">Estudiantes</a>
            
            @if (authService.isAdmin()) {
              <a routerLink="/admin" routerLinkActive="active-nav" style="color: #f59e0b; text-decoration: none; font-weight: 600; padding: 0.5rem 0.75rem; border-radius: 6px;">Admin</a>
            }

            <div style="border-left: 1px solid #334155; padding-left: 1rem; display: flex; align-items: center; gap: 0.75rem;">
              <span style="font-size: 0.85rem; color: #94a3b8;">{{ authService.currentUser()?.fullName }}</span>
              <button (click)="authService.logout()" style="background: #334155; color: #f1f5f9; border: none; padding: 0.4rem 0.8rem; border-radius: 6px; cursor: pointer; font-size: 0.85rem;">
                Salir
              </button>
            </div>
          </nav>
        } @else {
          <div style="display: flex; gap: 0.75rem;">
            <a routerLink="/login" style="color: #f1f5f9; text-decoration: none; padding: 0.5rem 1rem; border: 1px solid #475569; border-radius: 6px;">Ingresar</a>
            <a routerLink="/register" style="background: #f97316; color: white; text-decoration: none; padding: 0.5rem 1rem; border-radius: 6px; font-weight: 600;">Registrarse</a>
          </div>
        }
      </div>
    </header>
  `,
  styles: [`
    .active-nav {
      background: #334155 !important;
      color: white !important;
    }
  `]
})
export class NavbarComponent {
  readonly authService = inject(AuthService);
  readonly academicService = inject(AcademicService);
}
