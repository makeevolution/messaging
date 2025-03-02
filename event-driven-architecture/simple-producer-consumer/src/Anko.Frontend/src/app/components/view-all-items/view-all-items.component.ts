import { Component, OnInit } from '@angular/core';
import { MatSortModule } from "@angular/material/sort";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatTableModule } from "@angular/material/table";
import { TableComponent } from 'src/app/components/shared/table/table.component';
import { TableContent } from 'src/app/models/table';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-view-all-items',
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    TableComponent
  ],
  standalone: true,
  templateUrl: './view-all-items.component.html',
  styleUrl: './view-all-items.component.css'
})
export class ViewAllItemsComponent implements OnInit {

  // Properties required for the table component, see html file for where these are used
  items: TableContent[] = []
  viewAllItemsColumns: string[] = ['name', 'category', 'price'];


  constructor(
    private apiService: ApiService
  ) { }

  ngOnInit() {
    this.apiService.getItems().subscribe((items: TableContent[]) => {
        this.items = items;
      }
    )
  }
}
