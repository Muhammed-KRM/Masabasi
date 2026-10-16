import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { HomePageComponent } from './home-page.component';
import { SeoMetaService } from '../../core/services/seo-meta.service';
import { JsonLdSchemaService } from '../../core/services/json-ld-schema.service';

describe('HomePageComponent', () => {
  let component: HomePageComponent;
  let fixture: ComponentFixture<HomePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomePageComponent],
      providers: [
        provideRouter([]),
        SeoMetaService,
        JsonLdSchemaService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HomePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should render club title and hero sections', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('#hero-title')?.textContent).toContain('MASABAŞI OYUN KULÜBÜ');
    expect(el.textContent).toContain('Bakırköy');
    expect(el.textContent).toContain('Periwinkle');
    expect(el.textContent).toContain('Modern Kutu Oyunları');
  });

  it('should render all 4 game categories', () => {
    expect(component.gameCategories.length).toBe(4);
    const el = fixture.nativeElement as HTMLElement;
    const cards = el.querySelectorAll('.game-category-card');
    expect(cards.length).toBe(4);
  });

  it('should render 4 steps for joining events', () => {
    expect(component.steps.length).toBe(4);
    const el = fixture.nativeElement as HTMLElement;
    const stepCards = el.querySelectorAll('.step-card');
    expect(stepCards.length).toBe(4);
  });

  it('should render FAQ items', () => {
    expect(component.faqs.length).toBeGreaterThan(0);
    const el = fixture.nativeElement as HTMLElement;
    const faqItems = el.querySelectorAll('.faq-item');
    expect(faqItems.length).toBe(component.faqs.length);
  });
});
