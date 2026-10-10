import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { CatalogComponent } from './features/catalog/catalog.component';
import { MyEnrollmentsComponent } from './features/enrollments/my-enrollments.component';
import { StudentsComponent } from './features/students/students.component';
import { AdminComponent } from './features/admin/admin.component';
import { studentGuard, adminGuard, homeGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', canActivate: [homeGuard], component: LoginComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'catalog', component: CatalogComponent, canActivate: [studentGuard] },
  { path: 'my-enrollments', component: MyEnrollmentsComponent, canActivate: [studentGuard] },
  { path: 'students', component: StudentsComponent },
  { path: 'admin', component: AdminComponent, canActivate: [adminGuard] },
  { path: '**', redirectTo: '' }
];
