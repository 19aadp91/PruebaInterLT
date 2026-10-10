import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AcademicService } from '../../core/services/academic.service';

@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div style="max-width: 900px; margin: 2rem auto; padding: 0 1rem;">
      <div style="margin-bottom: 1.5rem;">
        <h1 style="font-size: 1.6rem; font-weight: 700; color: #0f172a; margin: 0 0 0.25rem 0;">Directorio de Estudiantes Registrados</h1>
        <p style="color: #64748b; margin: 0; font-size: 0.95rem;">Comunidad académica vinculada al programa de créditos</p>
      </div>

      <div style="background: #f8fafc; border-left: 4px solid #0284c7; padding: 0.75rem 1rem; border-radius: 6px; font-size: 0.85rem; color: #0369a1; margin-bottom: 1.5rem;">
        🔒 <strong>Aislamiento de Privacidad (RF-6):</strong> En esta vista pública institucional solo se expone el nombre de los alumnos registrados. Los documentos de identidad y correos electrónicos permanecen protegidos.
      </div>

      <div style="background: white; border-radius: 12px; border: 1px solid #e2e8f0; overflow: hidden; box-shadow: 0 1px 3px rgba(0,0,0,0.05);">
        <table style="width: 100%; border-collapse: collapse; text-align: left; font-size: 0.95rem;">
          <thead>
            <tr style="background: #f1f5f9; color: #475569; font-size: 0.8rem; text-transform: uppercase; letter-spacing: 0.5px;">
              <th style="padding: 0.75rem 1.5rem; width: 60px;">#</th>
              <th style="padding: 0.75rem 1.5rem;">Nombre Completo del Estudiante</th>
              <th style="padding: 0.75rem 1.5rem; text-align: right;">Estado</th>
            </tr>
          </thead>
          <tbody>
            @for (student of academicService.publicStudents(); track student.id; let i = $index) {
              <tr style="border-bottom: 1px solid #f1f5f9;">
                <td style="padding: 1rem 1.5rem; color: #94a3b8; font-weight: 600;">{{ i + 1 }}</td>
                <td style="padding: 1rem 1.5rem; font-weight: 600; color: #0f172a; display: flex; align-items: center; gap: 0.75rem;">
                  <div style="width: 32px; height: 32px; border-radius: 50%; background: #ffedd5; color: #ea580c; display: flex; align-items: center; justify-content: center; font-weight: bold; font-size: 0.85rem;">
                    {{ student.fullName.charAt(0) }}
                  </div>
                  <span>{{ student.fullName }}</span>
                </td>
                <td style="padding: 1rem 1.5rem; text-align: right;">
                  <span style="background: #dcfce7; color: #166534; padding: 0.25rem 0.6rem; border-radius: 12px; font-size: 0.75rem; font-weight: 700;">
                    Activo
                  </span>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class StudentsComponent implements OnInit {
  readonly academicService = inject(AcademicService);

  ngOnInit() {
    this.academicService.loadPublicStudents().subscribe();
  }
}
