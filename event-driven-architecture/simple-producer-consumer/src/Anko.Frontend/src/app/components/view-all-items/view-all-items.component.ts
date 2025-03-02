import { Component } from '@angular/core';
import {MatSortModule} from "@angular/material/sort";
import {MatPaginatorModule} from "@angular/material/paginator";
import {MatTableModule} from "@angular/material/table";
import { TableComponent } from '../shared/table/table.component';

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
export class ViewAllItemsComponent {

}
