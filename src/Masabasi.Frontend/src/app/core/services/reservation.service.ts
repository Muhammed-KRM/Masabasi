import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateReservationModel,
  ReservationResultModel
} from '../models/event-table.model';

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private readonly http = inject(HttpClient);

  /**
   * Seçilen masadaki bir koltuğa oyuncu kaydı açar.
   */
  public makeReservation(
    request: CreateReservationModel
  ): Observable<ReservationResultModel> {
    return this.http.post<ReservationResultModel>(
      `${environment.apiUrl}/api/v1/public/reservations`,
      request
    );
  }
}
