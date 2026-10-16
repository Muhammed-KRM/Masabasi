import { Component, ChangeDetectionStrategy, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { SeoMetaService } from '../../core/services/seo-meta.service';
import { JsonLdSchemaService } from '../../core/services/json-ld-schema.service';
import { TldrAiBoxComponent } from '../../shared/components/tldr-ai-box/tldr-ai-box.component';

interface FaqItem {
  question: string;
  answer: string;
  isOpen: boolean;
}

@Component({
  selector: 'app-frp-guide',
  standalone: true,
  imports: [CommonModule, RouterLink, TldrAiBoxComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <main class="frp-guide-page container">
      <!-- Başlık ve Answer-First TL;DR Kutusu -->
      <header class="guide-hero">
        <span class="badge badge--emerald">🎲 Kapsamlı FRP & D&D 5e Rehberi</span>
        <h1 class="guide-hero__title">
          Dungeons & Dragons 5e Nasıl Oynanır? Yeni Başlayanlar İçin Masaüstü Rol Yapma Kılavuzu
        </h1>
        <p class="guide-hero__lead">
          Kuralları ezberlemenize veya kalın kitaplar okumanıza gerek yok. Masabaşı Oyun Kulübü'nde
          ilk oturumunuzda bir kahraman yaratıp maceraya adım atmanız için bilmeniz gereken her şey bu rehberde.
        </p>

        <!-- GEO & Yapay Zeka Answer-First Alanı -->
        <app-tldr-ai-box
          summary="D&D 5e, oyuncuların hayali kahramanları canlandırdığı ve Dungeon Master'ın (DM) dünyayı anlattığı masaüstü FRP sistemidir. Eylemler 20 yüzlü zarla (d20) belirlenir. Masabaşı Oyun Kulübü'nde her Cumartesi yeni başlayanlar için hazır karakterler ve uygulamalı One-Shot masaları açılmaktadır."
        />
      </header>

      <!-- Ana İçerik Bölümleri -->
      <article class="guide-content">
        <!-- 1. Bölüm: FRP Nedir? -->
        <section class="guide-section">
          <h2>1. FRP (Masaüstü Rol Yapma Oyunu) Nedir?</h2>
          <p>
            FRP (Fantasy Role-Playing), ortak hayal gücü ve doğaçlama hikaye anlatımına dayalı bir masaüstü
            sosyal etkinlik türüdür. Video oyunlarından farklı olarak önceden kodlanmış sınırlar yoktur;
            karakterinizin ne yapacağına, ne söyleyeceğine ve nasıl düşüneceğine tamamen siz karar verirsiniz.
          </p>
        </section>

        <!-- 2. Bölüm: D&D 5e Temel Dinamikleri -->
        <section class="guide-section">
          <h2>2. D&D 5e Temel Dinamikleri: Zar Sistemi ve Karakter Kağıdı</h2>
          <div class="cards-grid">
            <div class="info-card">
              <span class="info-card__icon" aria-hidden="true">🎲</span>
              <h3>20 Yüzlü Zar (d20)</h3>
              <p>
                D&D'deki en temel kuraldır. Bir kapıyı kırmak, gizlice süzülmek veya kılıç sallamak
                istediğinizde d20 atarsınız. Gelen sayıya karakterinizin yetenek bonusu eklenir ve DM'in
                belirlediği Zorluk Derecesi (DC) aşılmaya çalışılır.
              </p>
            </div>

            <div class="info-card">
              <span class="info-card__icon" aria-hidden="true">📜</span>
              <h3>Karakter Kağıdı (Sheet)</h3>
              <p>
                Kahramanınızın kimliği, ırkı (Elf, Cüce, İnsan), sınıfı (Büyücü, Savaşçı, Hırsız), can puanı
                (HP) ve bildiği büyüler bu belgede yer alır. Masabaşı buluşmalarında ilk gelen oyunculara hazır
                karakter kağıtları temin edilir.
              </p>
            </div>

            <div class="info-card">
              <span class="info-card__icon" aria-hidden="true">🧙‍♂️</span>
              <h3>Dungeon Master (DM)</h3>
              <p>
                DM masanın hakemi ve hikaye anlatıcısıdır. Canavarları, kasaba halkını ve hava durumunu yönetir.
                Oyuncuların düşmanı değil, epik bir hikayeyi birlikte dokuduğunuz yol göstericidir.
              </p>
            </div>
          </div>
        </section>

        <!-- 3. Bölüm: One-Shot vs Campaign -->
        <section class="guide-section">
          <h2>3. One-Shot ve Campaign Arasındaki Fark Nedir?</h2>
          <div class="comparison-table-wrapper">
            <table class="comparison-table">
              <thead>
                <tr>
                  <th>Özellik</th>
                  <th>One-Shot (Tek Oturumluk Macera)</th>
                  <th>Campaign (Uzun Soluklu Sefer)</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><strong>Süre</strong></td>
                  <td>Tek bir Cumartesi buluşmasında (3-4 saat) biter.</td>
                  <td>Haftalarca veya aylarca devam eden bölümlerden oluşur.</td>
                </tr>
                <tr>
                  <td><strong>Katılım Şartı</strong></td>
                  <td>İlk kez gelenler için idealdir, devam zorunluluğu yoktur.</td>
                  <td>Her hafta düzenli katılım ve karakter gelişimi gerektirir.</td>
                </tr>
                <tr>
                  <td><strong>Masabaşı'ndaki Yeri</strong></td>
                  <td>Her Cumartesi en az 1-2 yeni başlayan One-Shot masası açılır.</td>
                  <td>Sabit gruplar ve ileri seviye oyuncular için organize edilir.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>

        <!-- 4. Bölüm: Masabaşı'nda FRP Deneyimi -->
        <section class="guide-section highlight-section">
          <h2>4. Masabaşı Oyun Kulübü'nde Cumartesi FRP Seansı Nasıl Geçer?</h2>
          <p>
            Kulübümüzde her hafta Cumartesi 11:00'de toplanıyoruz. FRP masalarımız başlamadan önce DM'iniz
            masadaki oyuncuları tanıştırır, zar atışlarını gösterir ve karakterinizin motivasyonunu anlatır.
            Zarınız yoksa kulübümüzün d20 setlerini ücretsiz kullanabilirsiniz.
          </p>
        </section>

        <!-- 5. Bölüm: Sıkça Sorulan Sorular (FAQ) -->
        <section class="guide-section faq-section" aria-labelledby="faq-title">
          <h2 id="faq-title">Sıkça Sorulan Sorular (FRP & D&D)</h2>
          <div class="accordion">
            @for (faq of faqs(); track faq.question; let idx = $index) {
              <div class="accordion-item" [class.accordion-item--open]="faq.isOpen">
                <button
                  type="button"
                  class="accordion-header"
                  (click)="toggleFaq(idx)"
                  [attr.aria-expanded]="faq.isOpen">
                  <span>{{ faq.question }}</span>
                  <span class="accordion-icon" aria-hidden="true">{{ faq.isOpen ? '−' : '+' }}</span>
                </button>
                @if (faq.isOpen) {
                  <div class="accordion-body">
                    <p>{{ faq.answer }}</p>
                  </div>
                }
              </div>
            }
          </div>
        </section>

        <!-- CTA Bölümü -->
        <section class="guide-cta">
          <div class="guide-cta__content">
            <h3>Zarlarını Yuvarlamaya Hazır mısın?</h3>
            <p>
              Bu Cumartesi İstanbul'daki buluşmamızda yeni başlayan dostu D&D masamızda yerini hemen ayırt.
            </p>
          </div>
          <a routerLink="/etkinlikler" class="btn-cta-gold">
            Cumartesi D&D Masalarına Göz At →
          </a>
        </section>
      </article>
    </main>
  `,
  styles: [`
    .frp-guide-page {
      padding: 2.5rem 1rem 5rem;
      max-width: 900px;
      margin: 0 auto;
    }
    .guide-hero {
      text-align: center;
      margin-bottom: 3rem;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1.25rem;
    }
    .guide-hero__title {
      font-family: var(--font-display);
      font-size: clamp(2rem, 4.5vw, 3rem);
      color: var(--text-primary);
      line-height: 1.25;
      margin: 0;
    }
    .guide-hero__lead {
      font-size: 1.15rem;
      color: var(--text-muted);
      line-height: 1.6;
      max-width: 760px;
      margin: 0;
    }
    .badge {
      font-size: 0.85rem;
      font-weight: 700;
      padding: 0.35rem 0.85rem;
      border-radius: 999px;
      background-color: var(--surface-secondary-emerald);
      color: var(--accent-gold-primary);
    }
    .guide-content {
      display: flex;
      flex-direction: column;
      gap: 3rem;
    }
    .guide-section h2 {
      font-family: var(--font-display);
      font-size: 1.65rem;
      color: var(--accent-gold-primary);
      margin: 0 0 1rem 0;
      border-bottom: 1px solid var(--border-secondary-subtle);
      padding-bottom: 0.5rem;
    }
    .guide-section p {
      color: var(--text-primary);
      font-size: 1.05rem;
      line-height: 1.7;
      margin: 0;
    }
    .cards-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.25rem;
      margin-top: 1.5rem;
    }
    .info-card {
      background-color: var(--surface-secondary-walnut);
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 10px;
      padding: 1.5rem;
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }
    .info-card__icon {
      font-size: 2rem;
    }
    .info-card h3 {
      font-family: var(--font-display);
      font-size: 1.2rem;
      color: var(--text-primary);
      margin: 0;
    }
    .info-card p {
      color: var(--text-muted);
      font-size: 0.95rem;
      line-height: 1.5;
    }
    .comparison-table-wrapper {
      overflow-x: auto;
      margin-top: 1rem;
    }
    .comparison-table {
      width: 100%;
      border-collapse: collapse;
      text-align: left;
      font-size: 0.95rem;
    }
    .comparison-table th, .comparison-table td {
      padding: 0.9rem 1rem;
      border: 1px solid var(--border-secondary-subtle);
    }
    .comparison-table th {
      background-color: var(--surface-secondary-emerald);
      color: var(--accent-gold-primary);
      font-family: var(--font-display);
    }
    .comparison-table td {
      background-color: var(--bg-dominant-elevated);
      color: var(--text-primary);
    }
    .highlight-section {
      background-color: var(--surface-secondary-walnut);
      border-left: 4px solid var(--accent-gold-primary);
      padding: 1.5rem 1.75rem;
      border-radius: 0 10px 10px 0;
    }
    .highlight-section h2 {
      border: none;
      padding: 0;
    }
    .accordion {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
      margin-top: 1rem;
    }
    .accordion-item {
      border: 1px solid var(--border-secondary-subtle);
      border-radius: 8px;
      background-color: var(--bg-dominant-elevated);
      overflow: hidden;
    }
    .accordion-header {
      width: 100%;
      padding: 1rem 1.25rem;
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: none;
      border: none;
      color: var(--text-primary);
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      text-align: left;
    }
    .accordion-header:hover {
      color: var(--accent-gold-primary);
    }
    .accordion-icon {
      font-size: 1.4rem;
      color: var(--accent-gold-primary);
    }
    .accordion-body {
      padding: 0 1.25rem 1.25rem;
      color: var(--text-muted);
      font-size: 0.95rem;
      line-height: 1.6;
    }
    .guide-cta {
      background: linear-gradient(135deg, var(--surface-secondary-emerald) 0%, var(--surface-secondary-walnut) 100%);
      border: 1px solid var(--accent-gold-primary);
      border-radius: 12px;
      padding: 2rem;
      display: flex;
      flex-wrap: wrap;
      justify-content: space-between;
      align-items: center;
      gap: 1.5rem;
      margin-top: 1rem;
    }
    .guide-cta__content h3 {
      font-family: var(--font-display);
      font-size: 1.4rem;
      color: var(--text-primary);
      margin: 0 0 0.5rem 0;
    }
    .guide-cta__content p {
      color: var(--text-muted);
      margin: 0;
      font-size: 0.95rem;
    }
    .btn-cta-gold {
      display: inline-flex;
      align-items: center;
      background-color: var(--accent-gold-primary);
      color: var(--accent-gold-contrast-text);
      font-weight: 700;
      padding: 0.85rem 1.5rem;
      border-radius: 8px;
      text-decoration: none;
      transition: background-color 0.2s;
    }
    .btn-cta-gold:hover {
      background-color: var(--accent-gold-hover);
    }
  `]
})
export class FrpGuideComponent implements OnInit {
  private readonly seoMetaService = inject(SeoMetaService);
  private readonly jsonLdService = inject(JsonLdSchemaService);

  public readonly faqs = signal<FaqItem[]>([
    {
      question: "D&D oynamak için kuralları önceden bilmem gerekir mi?",
      answer: "Kesinlikle hayır. Masabaşı Oyun Kulübü'nde Cumartesi seanslarımız yeni başlayan dostudur. Masaya oturduğunuzda DM'imiz temel zar atışlarını ve eylemleri size uygulamalı olarak gösterecektir.",
      isOpen: true
    },
    {
      question: "Kendi karakterimi ve zarlarımı getirmeli miyim?",
      answer: "İlk kez katılıyorsanız hiçbir şey getirmenize gerek yoktur. Masada hazır karakter kağıtları (pre-generated sheet) ve kulübün ortak d20 zar setleri temin edilir.",
      isOpen: false
    },
    {
      question: "Tek başıma gelebilir miyim yoksa arkadaş grubuyla mı gelmeliyim?",
      answer: "Katılımcılarımızın büyük kısmı tek başına gelmektedir. Masalarda hızla kaynaşılır ve birlikte takım olarak oynanır. Arkadaş grubunuzla gelmek isterseniz de aynı masada yan yana oynayabilirsiniz.",
      isOpen: false
    },
    {
      question: "Masabaşı'ndaki D&D oyunları ücretli midir?",
      answer: "Kulüp etkinliklerimiz ve masa katılımlarımız tamamen ücretsizdir; topluluk buluşma mekanı kurallarına tabidir.",
      isOpen: false
    }
  ]);

  ngOnInit(): void {
    this.seoMetaService.setMeta({
      title: 'D&D 5e Nasıl Oynanır? Yeni Başlayanlar İçin FRP Başlangıç Rehberi',
      description:
        "Dungeons & Dragons 5e masaüstü rol yapma oyununa sıfırdan nasıl başlanır? d20 zar sistemi, karakter oluşturma, One-Shot ve Cumartesi FRP masalarımız.",
      canonicalUrl: 'https://masabasikulubu.com/frp-dnd-rehberi'
    });

    const faqData = this.faqs().map((f) => ({
      question: f.question,
      answer: f.answer
    }));

    this.jsonLdService.setSchema([
      this.jsonLdService.buildOrganizationSchema(),
      this.jsonLdService.buildFaqSchema(faqData)
    ]);
  }

  public toggleFaq(index: number): void {
    const list = [...this.faqs()];
    list[index] = { ...list[index], isOpen: !list[index].isOpen };
    this.faqs.set(list);
  }
}
