import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { AuthResponse, UserProfileDto } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiUrl = 'http://localhost:5000/api/v1/auth';

  private readonly tokenKey = 'student_app_token';
  private readonly userKey = 'student_app_user';

  // Reactividad nativa con Signals
  readonly token = signal<string | null>(this.getStoredToken());
  readonly currentUser = signal<UserProfileDto | null>(this.getStoredUser());
  readonly isAuthenticated = computed(() => !!this.token());
  readonly isStudent = computed(() => this.currentUser()?.role === 'Estudiante');
  readonly isAdmin = computed(() => this.currentUser()?.role === 'Administrador');

  register(data: { fullName: string; identificationNumber: string; email: string; password: string }) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, data).pipe(
      tap(res => this.setSession(res))
    );
  }

  login(credentials: { email: string; password: string }) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(res => this.setSession(res))
    );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.token.set(null);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  private setSession(auth: AuthResponse) {
    localStorage.setItem(this.tokenKey, auth.token);
    localStorage.setItem(this.userKey, JSON.stringify(auth.user));
    this.token.set(auth.token);
    this.currentUser.set(auth.user);
  }

  private getStoredToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private getStoredUser(): UserProfileDto | null {
    const raw = localStorage.getItem(this.userKey);
    return raw ? JSON.parse(raw) : null;
  }
}
