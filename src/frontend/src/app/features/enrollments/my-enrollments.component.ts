import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AcademicService } from '../../core/services/academic.service';
import { EnrolledCourseItemDto, ClassmateDto } from '../../core/models/models';

@Component({
  selector: 'app-my-enrollments',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-enrollments.component.html',
  styleUrl: './my-enrollments.component.css'
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
