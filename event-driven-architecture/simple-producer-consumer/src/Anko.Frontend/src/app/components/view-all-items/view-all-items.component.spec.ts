import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewAllItemsComponent } from './view-all-items.component';

describe('ViewAllItemsComponent', () => {
  let component: ViewAllItemsComponent;
  let fixture: ComponentFixture<ViewAllItemsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewAllItemsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewAllItemsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
