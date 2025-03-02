import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {MatSort, MatSortModule} from '@angular/material/sort';
import {MatTableDataSource, MatTableModule} from '@angular/material/table';
import {TableContent} from '../../../models/table';
import {MatPaginator} from '@angular/material/paginator';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-table',
  standalone: true,
  imports: [
    MatTableModule,
    MatSortModule,
  ],
  templateUrl: './table.component.html',
  styleUrl: './table.component.css'
})
// Why do we use AfterViewInit, and not ngOnInit, to set sorting and pagination?
// ngOnInit is a lifecycle hook that is called after Angular has initialized all data-bound properties of a directive.
// But, when ngOnInit is called, the view may not fully initialized yet.
// This means you cannot access matSort and matPaginator (see the table html file for where they are in the html) in ngOnInit.
// AfterViewInit is a lifecycle hook that is called after Angular has fully initialized a component's view.
// This guarantees that the view is fully initialized and you can access the view's DOM.
export class TableComponent implements AfterViewInit {
  dataSource : MatTableDataSource<TableContent>;

  // ViewChild is a decorator that configures a view query. 
  // The change detector looks for the first element or the directive matching the selector in the view DOM. 
  // If the view DOM changes, and a new child matches the selector, the property is updated.
  @ViewChild(MatSort) sort!: MatSort;  // This is bound to matSort directive in the template.
  @ViewChild(MatPaginator) paginator!: MatPaginator;  // TOOO: Add this functionality
  displayedColumns: string[] = ['name', 'price'];
  constructor() {
    const items: TableContent[] = [
      {name: 'ftem 1', price: 100, category: 'Category 1'},
      {name: 'Item 2', price: 1003, category: 'Category 2'},
      {name: 'Item 9', price: 100, category: 'Category 2'},
    ]
    this.dataSource = new MatTableDataSource(items);
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }
}
