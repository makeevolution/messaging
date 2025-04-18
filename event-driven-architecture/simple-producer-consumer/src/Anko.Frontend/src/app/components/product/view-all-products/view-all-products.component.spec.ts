import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewAllProductsComponent } from './view-all-products.component';

describe('ViewAllItemsComponent', () => {
  let component: ViewAllProductsComponent;
  let fixture: ComponentFixture<ViewAllProductsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewAllProductsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewAllProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
