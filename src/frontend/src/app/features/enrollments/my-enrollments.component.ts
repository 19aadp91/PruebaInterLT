import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AcademicService } from '../../core/services/academic.service';
import { EnrolledCourseItemDto, ClassmateDto } from '../../core/models/models';

@Component({
  selector: 'app-my-enrollments',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div style="max-width: 1000px; margin: 2rem auto; padding: 0 1rem;">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; flex-wrap: wrap; gap: 1rem;">
        <div>
          <h1 style="font-size: 1.6rem; font-weight: 700; color: #0f172a; margin: 0 0 0.25rem 0;">Mis Materias Inscritas</h1>
          <p style="color: #64748b; margin: 0; font-size: 0.95rem;">Gestiona tu carga académica y consulta a tus compañeros de clase</p>
        </div>
        <a routerLink="/catalog" style="background: #ea580c; color: white; padding: 0.6rem 1.2rem; border-radius: 6px; text-decoration: none; font-weight: 600; font-size: 0.9rem;">
          + Inscribir Más Asignaturas
        </a>
      </div>

      @if (actionMessage()) {
        <div style="background: #dcfce7; border-left: 4px solid #22c55e; color: #15803d; padding: 0.85rem 1rem; border-radius: 6px; margin-bottom: 1.5rem; font-weight: 500;">
          {{ actionMessage() }}
        </div>
      }

      @if (academicService.myEnrollments(); as summary) {
        @if (summary.courses.length === 0) {
          <div style="background: white; border: 1px dashed #cbd5e1; border-radius: 12px; padding: 3rem; text-align: center; color: #64748b;">
            <div style="font-size: 2.5rem; margin-bottom: 0.5rem;">📚</div>
            <h3 style="font-size: 1.2rem; font-weight: 600; color: #1e293b; margin-bottom: 0.5rem;">No tienes asignaturas inscritas</h3>
            <p style="font-size: 0.9rem; margin-bottom: 1.5rem;">Explora el catálogo de 10 materias disponibles e inscribe hasta 3 materias sin repetir profesor.</p>
            <a routerLink="/catalog" style="background: #ea580c; color: white; padding: 0.6rem 1.25rem; border-radius: 6px; text-decoration: none; font-weight: 600;">
              Ir al Catálogo de Materias
            </a>
          </div>
        } @else {
          <div style="background: white; border-radius: 12px; border: 1px solid #e2e8f0; overflow: hidden; box-shadow: 0 1px 3px rgba(0,0,0,0.05);">
            <div style="background: #f8fafc; padding: 1rem 1.5rem; border-bottom: 1px solid #e2e8f0; display: flex; justify-content: space-between; align-items: center;">
              <span style="font-weight: 600; color: #334155;">Resumen de Créditos</span>
              <span style="background: #ea580c; color: white; padding: 0.25rem 0.75rem; border-radius: 12px; font-weight: 700; font-size: 0.85rem;">
                {{ summary.totalCredits }} de 9 Créditos ({{ summary.enrolledCoursesCount }}/3 Materias)
              </span>
            </div>

            <table style="width: 100%; border-collapse: collapse; text-align: left; font-size: 0.9rem;">
              <thead>
                <tr style="background: #f1f5f9; color: #475569; font-size: 0.8rem; text-transform: uppercase; letter-spacing: 0.5px;">
                  <th style="padding: 0.75rem 1.5rem;">Código</th>
                  <th style="padding: 0.75rem 1.5rem;">Materia</th>
                  <th style="padding: 0.75rem 1.5rem;">Docente Asignado</th>
                  <th style="padding: 0.75rem 1.5rem; text-align: center;">Créditos</th>
                  <th style="padding: 0.75rem 1.5rem; text-align: right;">Acciones</th>
                </tr>
              </thead>
              <tbody>
                @for (item of summary.courses; track item.enrollmentId) {
                  <tr style="border-bottom: 1px solid #f1f5f9;">
                    <td style="padding: 1rem 1.5rem; font-weight: 700; color: #1e293b;">{{ item.courseCode }}</td>
                    <td style="padding: 1rem 1.5rem; font-weight: 600; color: #0f172a;">{{ item.courseName }}</td>
                    <td style="padding: 1rem 1.5rem; color: #475569;">👨‍🏫 {{ item.professorName }}</td>
                    <td style="padding: 1rem 1.5rem; text-align: center;">
                      <span style="background: #ffedd5; color: #9a3412; padding: 0.2rem 0.5rem; border-radius: 6px; font-weight: 600;">3 CR</span>
                    </td>
                    <td style="padding: 1rem 1.5rem; text-align: right;">
                      <button (click)="openClassmatesModal(item)"
                              style="background: #e0f2fe; color: #0369a1; border: none; padding: 0.4rem 0.8rem; border-radius: 6px; font-weight: 600; cursor: pointer; margin-right: 0.5rem; font-size: 0.85rem;">
                        👥 Ver Compañeros
                      </button>
                      <button (click)="onDrop(item)"
                              style="background: #fee2e2; color: #b91c1c; border: none; padding: 0.4rem 0.8rem; border-radius: 6px; font-weight: 600; cursor: pointer; font-size: 0.85rem;">
                        Retirar
                      </button>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        }
      }

      <!-- Modal de Compañeros de Clase (Privacidad Preservada: Solo Nombres) -->
      @if (activeModalCourse()) {
        <div style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 1000; padding: 1rem;">
          <div style="background: white; border-radius: 12px; width: 100%; max-width: 500px; padding: 1.5rem; box-shadow: 0 20px 25px -5px rgba(0,0,0,0.2);">
            <div style="display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #e2e8f0; padding-bottom: 0.75rem; margin-bottom: 1rem;">
              <div>
                <h3 style="margin: 0; font-size: 1.15rem; color: #0f172a;">Compañeros de Clase</h3>
                <span style="font-size: 0.85rem; color: #64748b;">{{ activeModalCourse()?.courseName }} ({{ activeModalCourse()?.courseCode }})</span>
              </div>
              <button (click)="closeModal()" style="background: none; border: none; font-size: 1.25rem; cursor: pointer; color: #64748b;">✕</button>
            </div>

            <div style="background: #f8fafc; border-left: 3px solid #3b82f6; padding: 0.5rem 0.75rem; border-radius: 4px; font-size: 0.8rem; color: #475569; margin-bottom: 1rem;">
              🔒 <strong>Política de Privacidad:</strong> Por protección de datos institucionales, en esta consulta únicamente se visualiza el nombre completo de los alumnos matriculados.
            </div>

            @if (loadingClassmates()) {
              <div style="text-align: center; padding: 2rem; color: #64748b;">Cargando lista de compañeros...</div>
            } @else if (classmates().length === 0) {
              <div style="text-align: center; padding: 2rem; color: #94a3b8;">Aún no hay otros compañeros matriculados en esta clase.</div>
            } @else {
              <ul style="list-style: none; padding: 0; margin: 0; max-height: 250px; overflow-y: auto;">
                @for (c of classmates(); track c.id) {
                  <li style="padding: 0.65rem 0.75rem; border-bottom: 1px solid #f1f5f9; display: flex; align-items: center; gap: 0.75rem; font-weight: 500; color: #1e293b;">
                    <div style="width: 28px; height: 28px; border-radius: 50%; background: #e2e8f0; display: flex; align-items: center; justify-content: center; font-size: 0.8rem; color: #475569;">
                      🎓
                    </div>
                    <span>{{ c.fullName }}</span>
                  </li>
                }
              </ul>
            }

            <div style="text-align: right; margin-top: 1.5rem;">
              <button (click)="closeModal()" style="background: #1e293b; color: white; border: none; padding: 0.5rem 1rem; border-radius: 6px; font-weight: 600; cursor: pointer;">
                Cerrar
              </button>
            </div>
          </div>
        </div>
      }

    </div>
  `
})
export class MyEnrollmentsComponent implements OnInit {
  readonly academicService = inject(AcademicService);

  actionMessage = signal<string | null>(null);
  activeModalCourse = signal<EnrolledCourseItemDto | null>(null);
  classmates = signal<ClassmateDto[]>([]);
  loadingClassmates = signal(false);

  ngOnInit() {
    this.academicService.loadMyEnrollments().subscribe();
  }

  onDrop(item: EnrolledCourseItemDto) {
    if (!confirm(`¿Confirmas que deseas retirar la asignatura "${item.courseName}"?`)) return;

    this.academicService.dropCourse(item.courseId).subscribe({
      next: () => {
        this.actionMessage.set(`La asignatura "${item.courseName}" ha sido retirada exitosamente.`);
      }
    });
  }

  openClassmatesModal(item: EnrolledCourseItemDto) {
    this.activeModalCourse.set(item);
    this.loadingClassmates.set(true);

    this.academicService.getClassmates(item.courseId).subscribe({
      next: (list) => {
        this.loadingClassmates.set(false);
        this.classmates.set(list);
      },
      error: () => {
        this.loadingClassmates.set(false);
      }
    });
  }

  closeModal() {
    this.activeModalCourse.set(null);
    this.classmates.set([]);
  }
}
