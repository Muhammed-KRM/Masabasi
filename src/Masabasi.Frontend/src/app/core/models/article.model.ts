/**
 * Topic Cluster SEO blog ve kural rehberleri özet modeli.
 */
export interface ArticleSummaryModel {
  id: string;
  title: string;
  slug: string;
  topicCluster: string;
  tldrSummary: string;
  coverImageUrl?: string | null;
  estimatedReadTimeMinutes: number;
  publishedAtUtc: string;
  viewCount: number;
}

/**
 * Makale detay ve derlenmiş HTML içerik modeli.
 */
export interface ArticleModel {
  id: string;
  title: string;
  slug: string;
  topicCluster: string;
  tldrSummary: string;
  metaDescription: string;
  contentHtml: string;
  contentMarkdown: string;
  authorName: string;
  faqJson?: string | null;
  coverImageUrl?: string | null;
  estimatedReadTimeMinutes: number;
  publishedAtUtc: string;
  viewCount: number;
}
