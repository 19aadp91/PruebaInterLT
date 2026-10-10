import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AcademicService } from '../../core/services/academic.service';

@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './students.component.html',
  styleUrl: './students.component.css'
})
export class StudentsComponent implements OnInit {
  readonly academicService = inject(AcademicService);

  ngOnInit() {
    this.academicService.loadPublicStudents().subscribe();
  }
}
