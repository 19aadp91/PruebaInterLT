import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AcademicService } from '../../core/services/academic.service';
import { CourseDto } from '../../core/models/models';

@Component({
  selector: 'app-catalog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.css'
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
    this.academicService.myEnrollments()?.courses?.map(c => c.courseId) || []
  );

  readonly enrolledProfessorIds = computed(() =>
    this.academicService.myEnrollments()?.courses?.map(c => c.professorId) || []
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
