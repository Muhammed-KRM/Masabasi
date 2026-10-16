import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TldrAiBoxComponent } from './tldr-ai-box.component';

describe('TldrAiBoxComponent', () => {
  let component: TldrAiBoxComponent;
  let fixture: ComponentFixture<TldrAiBoxComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TldrAiBoxComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(TldrAiBoxComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('summary', 'Masabaşı Kulübü bu Cumartesi Kadıköy’de toplanıyor.');
    fixture.detectChanges();
  });

  it('should render TL;DR text and badge', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.querySelector('.tldr-box__badge')?.textContent).toContain('GEO & AI');
    expect(el.querySelector('.tldr-box__text')?.textContent).toContain('Masabaşı Kulübü bu Cumartesi Kadıköy’de toplanıyor.');
  });
});
