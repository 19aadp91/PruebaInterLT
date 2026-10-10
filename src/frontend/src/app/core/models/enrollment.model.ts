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
