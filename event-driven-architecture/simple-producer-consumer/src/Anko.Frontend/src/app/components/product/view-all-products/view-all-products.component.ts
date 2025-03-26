import { Component, OnInit } from '@angular/core';
import { MatSortModule } from "@angular/material/sort";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatTableModule } from "@angular/material/table";
import { TableComponent } from 'src/app/components/shared/table/table.component';
import { TableContent } from 'src/app/models/table';
import { ApiService } from 'src/app/services/api.service';
import { SingleProductComponent } from '../single-product/single-product.component';
import {MatGridListModule} from '@angular/material/grid-list';
@Component({
  selector: 'app-view-all-products',
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    SingleProductComponent,
    MatGridListModule,
    TableComponent
  ],
  standalone: true,
  templateUrl: './view-all-products.component.html',
  styleUrl: './view-all-products.component.css'
})
export class ViewAllProductsComponent implements OnInit {

  // Properties required for the table component, see html file for where these are used
  products: TableContent[] = []
  viewAllProductsColumns: string[] = ['name', 'category', 'price'];


  constructor(
    private apiService: ApiService
  ) { }

  ngOnInit() {
    this.apiService.getProducts().subscribe((products: TableContent[]) => {
        this.products = products;
      }
    )
  }
}
