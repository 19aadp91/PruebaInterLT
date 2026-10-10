import { ProfessorDto } from './professor.model';

export interface CourseDto {
  id: string;
  code: string;
  name: string;
  credits: number;
  professor: ProfessorDto;
}
