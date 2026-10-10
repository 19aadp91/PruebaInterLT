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
