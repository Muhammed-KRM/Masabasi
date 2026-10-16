/**
 * Cumartesi etkinliğinde açılan bir oyun veya FRP masasının modeli.
 */
export interface GameTableModel {
  id: string;
  tableNumber: number;
  gameTitle: string;
  category: 'BoardGame' | 'SocialDeduction' | 'FRP_DnD' | 'CardGame' | string;
  hostOrDmName: string;
  maxPlayers: number;
  currentPlayers: number;
  isBeginnerFriendly: boolean;
}

/**
 * Cumartesi etkinliğinin kapsamlı detay modeli.
 */
export interface ClubEventModel {
  id: string;
  title: string;
  slug: string;
  tldrSummary: string;
  startDateUtc: string;
  endDateUtc: string;
  venueName: string;
  venueAddress: string;
  meetupEventUrl?: string | null;
  externalRegistrationUrl?: string | null;
  tables: GameTableModel[];
}

/**
 * Masa koltuk rezervasyonu istek modeli.
 */
export interface CreateReservationModel {
  gameTableId: string;
  playerName: string;
  contactHandle: string;
  experienceLevel: 'Beginner' | 'Intermediate' | 'Expert' | string;
  notes?: string | null;
}

/**
 * Başarılı rezervasyon sonucu modeli.
 */
export interface ReservationResultModel {
  reservationId: string;
  tableId: string;
  tableNumber: number;
  gameTitle: string;
  currentPlayers: number;
  maxPlayers: number;
}

/**
 * SignalR 'TableSeatUpdated' gerçek zamanlı bildirim modeli.
 */
export interface TableSeatUpdatedPayload {
  tableId: string;
  currentPlayers: number;
  maxPlayers: number;
}
