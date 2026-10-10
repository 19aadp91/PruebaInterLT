export enum ErrorType {
  None = 0,
  Validation = 1,
  BadRequest = 2,
  Unauthorized = 3,
  Forbidden = 4,
  NotFound = 5,
  Conflict = 6,
  InternalServerError = 7
}

export interface Result<T = unknown> {
  isSuccess: boolean;
  message?: string;
  errors: string[];
  result?: T;
  errorType: ErrorType;
}

export interface UserProfileDto {
  id: string;
  fullName: string;
  email: string;
  role: 'Estudiante' | 'Administrador';
}

export interface AuthResponse {
  token: string;
  expiresInSeconds: number;
  user: UserProfileDto;
}

export interface PublicStudentDto {
  id: string;
  fullName: string;
}

export interface StudentProfileDto {
  id: string;
  fullName: string;
  identificationNumber: string;
  email: string;
  totalCredits: number;
  enrolledCoursesCount: number;
}

export interface ProfessorDto {
  id: string;
  fullName: string;
  email: string;
  isAvailable: boolean;
}

export interface CourseDto {
  id: string;
  code: string;
  name: string;
  credits: number;
  professor: ProfessorDto;
}

export interface EnrolledCourseItemDto {
  enrollmentId: string;
  courseId: string;
  courseCode: string;
  courseName: string;
  credits: number;
  professorId: string;
  professorName: string;
  enrolledAtUtc: string;
}

export interface StudentEnrollmentSummaryDto {
  totalCredits: number;
  maxCredits: number;
  enrolledCoursesCount: number;
  maxCourses: number;
  courses: EnrolledCourseItemDto[];
}

export interface ClassmateDto {
  id: string;
  fullName: string;
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}
