import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AcademicService } from '../../core/services/academic.service';
import { CourseDto } from '../../core/models/models';

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div style="max-width: 1200px; margin: 2rem auto; padding: 0 1rem;">
      
      <!-- Panel de Estado y Reglas de Negocio -->
      <div style="background: linear-gradient(135deg, #1e293b, #0f172a); color: white; border-radius: 12px; padding: 1.5rem; margin-bottom: 2rem; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1);">
        <div style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 1rem;">
          <div>
            <h1 style="font-size: 1.6rem; font-weight: 700; margin: 0 0 0.5rem 0;">Catálogo de Asignaturas Disponibles</h1>
            <p style="color: #94a3b8; margin: 0; font-size: 0.95rem;">
              Programa de Créditos Académicos · Cada materia equivale a <strong>3 créditos</strong>.
            </p>
          </div>

          <!-- Medidor de Créditos Reactivo con Signals -->
          <div style="background: rgba(255,255,255,0.08); padding: 1rem 1.5rem; border-radius: 10px; border: 1px solid rgba(255,255,255,0.1); text-align: right;">
            <div style="font-size: 0.8rem; text-transform: uppercase; letter-spacing: 1px; color: #cbd5e1;">Tu Carga Actual</div>
            <div style="font-size: 1.75rem; font-weight: 800; color: #f97316;">
              {{ currentCredits() }} / 9 <span style="font-size: 1rem; font-weight: 400; color: #cbd5e1;">créditos</span>
            </div>
            <div style="font-size: 0.85rem; color: #94a3b8;">
              {{ currentCount() }} de 3 materias inscritas
            </div>
          </div>
        </div>

        <!-- Banner de Regla de Negocio Obligatoria -->
        <div style="margin-top: 1.25rem; background: rgba(234, 88, 12, 0.15); border-left: 4px solid #ea580c; padding: 0.75rem 1rem; border-radius: 6px; font-size: 0.9rem; color: #fed7aa; display: flex; align-items: center; gap: 0.5rem;">
          <span>⚖️</span>
          <span><strong>Regla institucional innegociable:</strong> El estudiante no puede tener materias con el mismo docente. Las materias incompatibles se inhabilitan automáticamente.</span>
        </div>
      </div>

      <!-- Alertas de éxito o error -->
      @if (actionMessage()) {
        <div style="background: #dcfce7; border-left: 4px solid #22c55e; color: #15803d; padding: 0.85rem 1rem; border-radius: 6px; margin-bottom: 1.5rem; font-weight: 500;">
          {{ actionMessage() }}
        </div>
      }
      @if (errorMessage()) {
        <div style="background: #fee2e2; border-left: 4px solid #ef4444; color: #b91c1c; padding: 0.85rem 1rem; border-radius: 6px; margin-bottom: 1.5rem; font-weight: 500;">
          {{ errorMessage() }}
        </div>
      }

      <!-- Grid de las 10 Materias -->
      <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(340px, 1fr)); gap: 1.5rem;">
        @for (course of academicService.courses(); track course.id) {
          <div [style.border]="isEnrolled(course) ? '2px solid #22c55e' : (isConflict(course) ? '1px dashed #cbd5e1' : '1px solid #e2e8f0')"
               [style.background]="isConflict(course) ? '#f8fafc' : (isEnrolled(course) ? '#f0fdf4' : 'white')"
               style="border-radius: 12px; padding: 1.5rem; box-shadow: 0 1px 3px rgba(0,0,0,0.05); display: flex; flex-direction: column; justify-content: space-between; transition: all 0.2s;">
            
            <div>
              <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 0.5rem;">
                <span style="background: #e2e8f0; color: #475569; padding: 0.25rem 0.6rem; border-radius: 6px; font-size: 0.8rem; font-weight: 700; letter-spacing: 0.5px;">
                  {{ course.code }}
                </span>
                <span style="background: #ffedd5; color: #c2410c; padding: 0.25rem 0.6rem; border-radius: 12px; font-size: 0.75rem; font-weight: 700;">
                  3 Créditos
                </span>
              </div>

              <h3 style="font-size: 1.15rem; font-weight: 700; color: #0f172a; margin: 0.5rem 0 0.75rem 0;">
                {{ course.name }}
              </h3>

              <div style="display: flex; align-items: center; gap: 0.5rem; color: #475569; font-size: 0.9rem; margin-bottom: 1rem;">
                <span>👨‍🏫</span>
                <span style="font-weight: 600;">{{ course.professor.fullName }}</span>
              </div>
            </div>

            <!-- Zona de Acción y Estado -->
            <div style="margin-top: 1rem; padding-top: 1rem; border-top: 1px solid #e2e8f0;">
              @if (isEnrolled(course)) {
                <div style="background: #dcfce7; color: #166534; padding: 0.6rem; border-radius: 6px; text-align: center; font-size: 0.85rem; font-weight: 700;">
                  ✓ Asignatura Inscrita
                </div>
              } @else if (isConflict(course)) {
                <div style="background: #f1f5f9; color: #64748b; padding: 0.6rem; border-radius: 6px; text-align: center; font-size: 0.8rem; font-weight: 600;">
                  ⛔ Incompatible: Docente ya seleccionado en otra materia
                </div>
              } @else if (isMaxReached()) {
                <div style="background: #f1f5f9; color: #94a3b8; padding: 0.6rem; border-radius: 6px; text-align: center; font-size: 0.85rem;">
                  Límite máximo de 3 materias alcanzado
                </div>
              } @else {
                <button (click)="onEnroll(course)" [disabled]="loadingCourseId() === course.id"
                        style="width: 100%; padding: 0.65rem; background: #ea580c; color: white; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; transition: background 0.2s;">
                  {{ loadingCourseId() === course.id ? 'Inscribiendo...' : '+ Inscribir Materia' }}
                </button>
              }
            </div>

          </div>
        }
      </div>

    </div>
  `
})
export class CatalogComponent implements OnInit {
  readonly academicService = inject(AcademicService);

  loadingCourseId = signal<string | null>(null);
  actionMessage = signal<string | null>(null);
  errorMessage = signal<string | null>(null);

  // Signals reactivos derivados del estado actual
  readonly currentCount = computed(() => this.academicService.myEnrollments()?.enrolledCoursesCount || 0);
  readonly currentCredits = computed(() => this.academicService.myEnrollments()?.totalCredits || 0);
  readonly isMaxReached = computed(() => this.currentCount() >= 3);

  readonly enrolledCourseIds = computed(() =>
    this.academicService.myEnrollments()?.courses.map(c => c.courseId) || []
  );

  readonly enrolledProfessorIds = computed(() =>
    this.academicService.myEnrollments()?.courses.map(c => c.professorId) || []
  );

  ngOnInit() {
    this.academicService.loadCourses().subscribe();
    this.academicService.loadMyEnrollments().subscribe();
  }

  isEnrolled(course: CourseDto): boolean {
    return this.enrolledCourseIds().includes(course.id);
  }

  isConflict(course: CourseDto): boolean {
    if (this.isEnrolled(course)) return false;
    return this.enrolledProfessorIds().includes(course.professor.id);
  }

  onEnroll(course: CourseDto) {
    this.actionMessage.set(null);
    this.errorMessage.set(null);
    this.loadingCourseId.set(course.id);

    this.academicService.enrollCourse(course.id).subscribe({
      next: () => {
        this.loadingCourseId.set(null);
        this.actionMessage.set(`¡Materia "${course.name}" inscrita con éxito!`);
      },
      error: (err) => {
        this.loadingCourseId.set(null);
        this.errorMessage.set(err.error?.detail || 'Error al inscribir la materia.');
      }
    });
  }
}
