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
