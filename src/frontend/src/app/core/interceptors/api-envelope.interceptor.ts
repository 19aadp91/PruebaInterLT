import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Result } from '../models/models';

export const apiEnvelopeInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    map(event => {
      if (event instanceof HttpResponse) {
        const body = event.body as Result<unknown>;
        if (body && typeof body === 'object' && 'isSuccess' in body && 'result' in body) {
          return event.clone({ body: body.result });
        }
      }
      return event;
    })
  );
};
