export interface Student {
  id: number;
  name: string;
  email: string;
  age: number;
  course: string;
  createdDate: string;
}

export interface CreateStudentDto {
  name: string;
  email: string;
  age: number;
  course: string;
}

export interface UpdateStudentDto {
  name: string;
  email: string;
  age: number;
  course: string;
}
