import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import {
  CourseDto,
  PublicStudentDto,
  StudentProfileDto,
  StudentEnrollmentSummaryDto,
  ClassmateDto,
  ProfessorDto
} from '../models/models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AcademicService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  // Signals globales de estado académico
  readonly courses = signal<CourseDto[]>([]);
  readonly myEnrollments = signal<StudentEnrollmentSummaryDto | null>(null);
  readonly publicStudents = signal<PublicStudentDto[]>([]);

  loadCourses() {
    return this.http.get<CourseDto[]>(`${this.baseUrl}/courses`).pipe(
      tap(data => this.courses.set(data))
    );
  }

  loadMyEnrollments() {
    return this.http.get<StudentEnrollmentSummaryDto>(`${this.baseUrl}/students/me/enrollments`).pipe(
      tap(data => this.myEnrollments.set(data))
    );
  }

  loadPublicStudents() {
    return this.http.get<PublicStudentDto[]>(`${this.baseUrl}/students`).pipe(
      tap(data => this.publicStudents.set(data))
    );
  }

  getMyProfile() {
    return this.http.get<StudentProfileDto>(`${this.baseUrl}/students/me`);
  }

  enrollCourse(courseId: string) {
    return this.http.post<StudentEnrollmentSummaryDto>(`${this.baseUrl}/students/me/enrollments`, { courseId }).pipe(
      tap(data => this.myEnrollments.set(data))
    );
  }

  dropCourse(courseId: string) {
    return this.http.delete<StudentEnrollmentSummaryDto>(`${this.baseUrl}/students/me/enrollments/${courseId}`).pipe(
      tap(data => this.myEnrollments.set(data))
    );
  }

  getClassmates(courseId: string) {
    return this.http.get<ClassmateDto[]>(`${this.baseUrl}/courses/${courseId}/classmates`);
  }

  // Métodos de Administración
  adminGetProfessors() {
    return this.http.get<ProfessorDto[]>(`${this.baseUrl}/admin/professors`);
  }

  adminCreateProfessor(data: { fullName: string; email: string }) {
    return this.http.post<ProfessorDto>(`${this.baseUrl}/admin/professors`, data);
  }

  adminDeleteProfessor(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/admin/professors/${id}`);
  }

  adminCreateCourse(data: { code: string; name: string; professorId: string }) {
    return this.http.post<CourseDto>(`${this.baseUrl}/admin/courses`, data);
  }

  adminDeleteCourse(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/admin/courses/${id}`);
  }
}
