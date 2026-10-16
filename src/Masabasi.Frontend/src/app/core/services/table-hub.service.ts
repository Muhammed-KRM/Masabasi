import { Injectable, inject, PLATFORM_ID, OnDestroy } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import * as signalR from '@microsoft/signalr';
import { Subject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TableSeatUpdatedPayload } from '../models/event-table.model';

@Injectable({ providedIn: 'root' })
export class TableHubService implements OnDestroy {
  private readonly platformId = inject(PLATFORM_ID);
  private hubConnection: signalR.HubConnection | null = null;
  private readonly seatUpdatedSubject = new Subject<TableSeatUpdatedPayload>();

  public readonly seatUpdated$: Observable<TableSeatUpdatedPayload> =
    this.seatUpdatedSubject.asObservable();

  constructor() {
    if (isPlatformBrowser(this.platformId) && !environment.useStaticOnly) {
      this.initSignalR();
    }
  }

  private initSignalR(): void {
    try {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${environment.apiUrl}/hubs/table`, {
          skipNegotiation: false,
          transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Warning)
        .build();

      this.hubConnection.on(
        'TableSeatUpdated',
        (tableId: string, currentPlayers: number, maxPlayers: number) => {
          this.seatUpdatedSubject.next({ tableId, currentPlayers, maxPlayers });
        }
      );

      this.hubConnection.start().catch(() => {
        // API kapalıysa sessizce logla, uygulama çalışmaya devam eder
      });
    } catch {
      // SignalR başlatma hatası yutulur (resilient)
    }
  }

  public async joinTableGroup(tableId: string): Promise<void> {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      try {
        await this.hubConnection.invoke('JoinTableGroup', tableId);
      } catch {
        // Hata sessizce yutulur
      }
    }
  }

  public async leaveTableGroup(tableId: string): Promise<void> {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      try {
        await this.hubConnection.invoke('LeaveTableGroup', tableId);
      } catch {
        // Hata sessizce yutulur
      }
    }
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection.stop().catch(() => {});
    }
  }
}
