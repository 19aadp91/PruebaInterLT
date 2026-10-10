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
