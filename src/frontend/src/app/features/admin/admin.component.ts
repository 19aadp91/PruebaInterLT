import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AcademicService } from '../../core/services/academic.service';
import { ProfessorDto, CourseDto } from '../../core/models/models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
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
