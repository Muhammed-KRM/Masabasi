import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { FrpGuideComponent } from './frp-guide.component';

describe('FrpGuideComponent', () => {
  let component: FrpGuideComponent;
  let fixture: ComponentFixture<FrpGuideComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FrpGuideComponent],
      providers: [provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(FrpGuideComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and render hero title', () => {
    expect(component).toBeTruthy();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.guide-hero__title')?.textContent).toContain(
      'Dungeons & Dragons 5e Nasıl Oynanır?'
    );
  });

  it('should have initial FAQ list and toggle FAQ item open/close', () => {
    expect(component.faqs().length).toBeGreaterThan(0);
    expect(component.faqs()[0].isOpen).toBe(true);
    expect(component.faqs()[1].isOpen).toBe(false);

    // Toggle 2. soru
    component.toggleFaq(1);
    expect(component.faqs()[1].isOpen).toBe(true);

    // Toggle tekrar basınca kapanmalı
    component.toggleFaq(1);
    expect(component.faqs()[1].isOpen).toBe(false);
  });
});
