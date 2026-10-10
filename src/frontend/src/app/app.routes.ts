import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { CatalogComponent } from './features/catalog/catalog.component';
import { MyEnrollmentsComponent } from './features/enrollments/my-enrollments.component';
import { StudentsComponent } from './features/students/students.component';
import { AdminComponent } from './features/admin/admin.component';

export const routes: Routes = [
  { path: '', redirectTo: 'catalog', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'catalog', component: CatalogComponent },
  { path: 'my-enrollments', component: MyEnrollmentsComponent },
  { path: 'students', component: StudentsComponent },
  { path: 'admin', component: AdminComponent },
  { path: '**', redirectTo: 'catalog' }
];
