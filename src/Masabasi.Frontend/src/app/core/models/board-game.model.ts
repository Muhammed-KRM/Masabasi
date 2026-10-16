/**
 * Kutu oyunu ve FRP kural rehberi modeli.
 */
export interface BoardGameModel {
  id: string;
  title: string;
  slug: string;
  category: string;
  minPlayers: number;
  maxPlayers: number;
  recommendedPlayerCount?: string | null;
  playTimeMinutes: number;
  complexityRating: number;
  tldrSummary: string;
  rulesMarkdown: string;
  imageUrl?: string | null;
  isInClubInventory: boolean;
  yearPublished?: number | null;
}
