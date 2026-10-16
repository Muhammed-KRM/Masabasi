import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AdminDashboardComponent } from './admin-dashboard.component';
import { environment } from '../../../environments/environment';

describe('AdminDashboardComponent', () => {
  let component: AdminDashboardComponent;
  let fixture: ComponentFixture<AdminDashboardComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminDashboardComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDashboardComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create and default to events tab', () => {
    expect(component).toBeTruthy();
    expect(component.activeTab()).toBe('events');
  });

  it('should switch tabs', () => {
    component.setTab('games');
    expect(component.activeTab()).toBe('games');

    component.setTab('articles');
    expect(component.activeTab()).toBe('articles');

    component.setTab('snapshot');
    expect(component.activeTab()).toBe('snapshot');
  });

  it('should handle event creation feedback', () => {
    component.newEvent.title = 'Test Buluşması';
    component.handleCreateEvent();

    expect(component.snapshotFeedback()).toContain('Test Buluşması');
    expect(component.snapshotFeedbackSuccess()).toBe(true);
  });

  it('should handle game creation feedback', () => {
    component.newGame.title = 'Catan';
    component.handleCreateGame();

    expect(component.snapshotFeedback()).toContain('Catan');
    expect(component.snapshotFeedbackSuccess()).toBe(true);
  });

  it('should trigger snapshot export and handle successful API response', () => {
    component.triggerSnapshotExport();
    expect(component.isExporting()).toBe(true);

    const req = httpMock.expectOne(`${environment.apiUrl}/api/v1/admin/snapshots/export`);
    expect(req.request.method).toBe('POST');
    req.flush({ success: true });

    expect(component.isExporting()).toBe(false);
    expect(component.snapshotFeedback()).toContain('başarıyla güncellendi');
    expect(component.snapshotFeedbackSuccess()).toBe(true);
  });

  it('should fallback gracefully when snapshot export API fails', () => {
    component.triggerSnapshotExport();
    expect(component.isExporting()).toBe(true);

    const req = httpMock.expectOne(`${environment.apiUrl}/api/v1/admin/snapshots/export`);
    req.error(new ProgressEvent('Network error'));

    expect(component.isExporting()).toBe(false);
    expect(component.snapshotFeedback()).toContain('Statik Mod');
  });
});
