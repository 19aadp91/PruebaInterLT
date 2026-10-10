import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AcademicService } from '../../core/services/academic.service';
import { ProfessorDto, CourseDto } from '../../core/models/models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div style="max-width: 1100px; margin: 2rem auto; padding: 0 1rem;">
      <h1 style="font-size: 1.6rem; font-weight: 700; color: #0f172a; margin-bottom: 0.5rem;">Panel Administrativo Académico</h1>
      <p style="color: #64748b; margin-bottom: 2rem;">Gestión de docentes, asignaturas y oferta académica (RF-8)</p>

      @if (alertMessage()) {
        <div style="background: #dcfce7; border-left: 4px solid #22c55e; color: #15803d; padding: 0.85rem 1rem; border-radius: 6px; margin-bottom: 1.5rem; font-weight: 500;">
          {{ alertMessage() }}
        </div>
      }
      @if (errorMessage()) {
        <div style="background: #fee2e2; border-left: 4px solid #ef4444; color: #b91c1c; padding: 0.85rem 1rem; border-radius: 6px; margin-bottom: 1.5rem; font-weight: 500;">
          {{ errorMessage() }}
        </div>
      }

      <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 2rem; align-items: start;">
        
        <!-- Sección: Profesores -->
        <div style="background: white; padding: 1.5rem; border-radius: 12px; border: 1px solid #e2e8f0; box-shadow: 0 1px 3px rgba(0,0,0,0.05);">
          <h2 style="font-size: 1.25rem; font-weight: 700; color: #1e293b; margin: 0 0 1rem 0;">Docentes</h2>

          <form (ngSubmit)="onCreateProfessor()" style="background: #f8fafc; padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem; border: 1px solid #e2e8f0;">
            <h4 style="margin: 0 0 0.5rem 0; font-size: 0.9rem;">+ Nuevo Profesor</h4>
            <div style="display: flex; gap: 0.5rem; margin-bottom: 0.5rem;">
              <input type="text" [(ngModel)]="newProfName" name="profName" placeholder="Nombre completo" required
                     style="flex: 1; padding: 0.5rem; border: 1px solid #cbd5e1; border-radius: 4px; font-size: 0.85rem;" />
              <input type="email" [(ngModel)]="newProfEmail" name="profEmail" placeholder="Correo institucional" required
                     style="flex: 1; padding: 0.5rem; border: 1px solid #cbd5e1; border-radius: 4px; font-size: 0.85rem;" />
            </div>
            <button type="submit" style="background: #ea580c; color: white; border: none; padding: 0.4rem 0.8rem; border-radius: 4px; font-size: 0.85rem; font-weight: 600; cursor: pointer;">
              Crear Profesor
            </button>
          </form>

          <ul style="list-style: none; padding: 0; margin: 0;">
            @for (prof of professors(); track prof.id) {
              <li style="display: flex; justify-content: space-between; align-items: center; padding: 0.75rem 0; border-bottom: 1px solid #f1f5f9;">
                <div>
                  <div style="font-weight: 600; color: #1e293b;">{{ prof.fullName }}</div>
                  <div style="font-size: 0.8rem; color: #64748b;">{{ prof.email }}</div>
                </div>
                <button (click)="onDeleteProfessor(prof.id)"
                        style="background: #fee2e2; color: #b91c1c; border: none; padding: 0.3rem 0.6rem; border-radius: 4px; font-size: 0.75rem; font-weight: 600; cursor: pointer;">
                  Desactivar
                </button>
              </li>
            }
          </ul>
        </div>

        <!-- Sección: Asignaturas -->
        <div style="background: white; padding: 1.5rem; border-radius: 12px; border: 1px solid #e2e8f0; box-shadow: 0 1px 3px rgba(0,0,0,0.05);">
          <h2 style="font-size: 1.25rem; font-weight: 700; color: #1e293b; margin: 0 0 1rem 0;">Asignaturas Ofertadas</h2>

          <form (ngSubmit)="onCreateCourse()" style="background: #f8fafc; padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem; border: 1px solid #e2e8f0;">
            <h4 style="margin: 0 0 0.5rem 0; font-size: 0.9rem;">+ Nueva Asignatura (3 Créditos)</h4>
            <div style="display: grid; grid-template-columns: 1fr 2fr; gap: 0.5rem; margin-bottom: 0.5rem;">
              <input type="text" [(ngModel)]="newCourseCode" name="courseCode" placeholder="Código (ej. MAT-103)" required
                     style="padding: 0.5rem; border: 1px solid #cbd5e1; border-radius: 4px; font-size: 0.85rem;" />
              <input type="text" [(ngModel)]="newCourseName" name="courseName" placeholder="Nombre de la materia" required
                     style="padding: 0.5rem; border: 1px solid #cbd5e1; border-radius: 4px; font-size: 0.85rem;" />
            </div>
            <div style="margin-bottom: 0.5rem;">
              <select [(ngModel)]="selectedProfId" name="selectedProfId" required
                      style="width: 100%; padding: 0.5rem; border: 1px solid #cbd5e1; border-radius: 4px; font-size: 0.85rem;">
                <option value="">-- Seleccionar Docente Asignado --</option>
                @for (p of professors(); track p.id) {
                  <option [value]="p.id">{{ p.fullName }}</option>
                }
              </select>
            </div>
            <button type="submit" style="background: #ea580c; color: white; border: none; padding: 0.4rem 0.8rem; border-radius: 4px; font-size: 0.85rem; font-weight: 600; cursor: pointer;">
              Crear Asignatura
            </button>
          </form>

          <ul style="list-style: none; padding: 0; margin: 0;">
            @for (course of academicService.courses(); track course.id) {
              <li style="display: flex; justify-content: space-between; align-items: center; padding: 0.75rem 0; border-bottom: 1px solid #f1f5f9;">
                <div>
                  <span style="font-weight: 700; color: #1e293b; margin-right: 0.5rem;">{{ course.code }}</span>
                  <span style="font-weight: 500; color: #0f172a;">{{ course.name }}</span>
                  <div style="font-size: 0.8rem; color: #64748b;">👨‍🏫 {{ course.professor.fullName }}</div>
                </div>
                <button (click)="onDeleteCourse(course.id)"
                        style="background: #fee2e2; color: #b91c1c; border: none; padding: 0.3rem 0.6rem; border-radius: 4px; font-size: 0.75rem; font-weight: 600; cursor: pointer;">
                  Desactivar
                </button>
              </li>
            }
          </ul>
        </div>

      </div>
    </div>
  `
})
export class AdminComponent implements OnInit {
  readonly academicService = inject(AcademicService);

  professors = signal<ProfessorDto[]>([]);
  alertMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  newProfName = '';
  newProfEmail = '';

  newCourseCode = '';
  newCourseName = '';
  selectedProfId = '';

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.academicService.loadCourses().subscribe();
    this.academicService.adminGetProfessors().subscribe({
      next: (profs) => this.professors.set(profs)
    });
  }

  onCreateProfessor() {
    this.clearAlerts();
    this.academicService.adminCreateProfessor({ fullName: this.newProfName, email: this.newProfEmail }).subscribe({
      next: () => {
        this.alertMessage.set('Profesor creado exitosamente.');
        this.newProfName = '';
        this.newProfEmail = '';
        this.loadData();
      },
      error: (err) => this.errorMessage.set(err.error?.detail || 'Error al crear profesor.')
    });
  }

  onDeleteProfessor(id: string) {
    if (!confirm('¿Desea desactivar este profesor?')) return;
    this.clearAlerts();
    this.academicService.adminDeleteProfessor(id).subscribe({
      next: () => {
        this.alertMessage.set('Profesor desactivado.');
        this.loadData();
      },
      error: (err) => this.errorMessage.set(err.error?.detail || 'No fue posible desactivar al docente.')
    });
  }

  onCreateCourse() {
    this.clearAlerts();
    if (!this.selectedProfId) {
      this.errorMessage.set('Debe seleccionar un profesor asignado.');
      return;
    }

    this.academicService.adminCreateCourse({
      code: this.newCourseCode,
      name: this.newCourseName,
      professorId: this.selectedProfId
    }).subscribe({
      next: () => {
        this.alertMessage.set('Asignatura creada exitosamente.');
        this.newCourseCode = '';
        this.newCourseName = '';
        this.selectedProfId = '';
        this.loadData();
      },
      error: (err) => this.errorMessage.set(err.error?.detail || 'Error al crear la asignatura.')
    });
  }

  onDeleteCourse(id: string) {
    if (!confirm('¿Desea desactivar esta asignatura?')) return;
    this.clearAlerts();
    this.academicService.adminDeleteCourse(id).subscribe({
      next: () => {
        this.alertMessage.set('Asignatura desactivada.');
        this.loadData();
      },
      error: (err) => this.errorMessage.set(err.error?.detail || 'No fue posible desactivar la asignatura.')
    });
  }

  private clearAlerts() {
    this.alertMessage.set(null);
    this.errorMessage.set(null);
  }
}
