import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { TableContent } from 'src/app/models/table';
import { MatPaginator } from '@angular/material/paginator';

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
  // Assert dataSource is undefined since TableComponent is meant to be used by other components.
  // They have to set the dataSource using the setContents method.
  dataSource! : MatTableDataSource<TableContent>;
  displayedColumns!: string[] 

  // ViewChild is a decorator that configures a view query.
  // The change detector looks for the first element or the directive matching the selector in the view DOM.
  // If the view DOM changes, and a new child matches the selector, the property is updated.
  @ViewChild(MatSort) sort!: MatSort;  // This is bound to matSort directive in the template.
  @ViewChild(MatPaginator) paginator!: MatPaginator;  // TOOO: Add this functionality if desired

  /* this function is called when the setColumns input is set in the parent component */
  @Input() set setColumns(columns: string[]) {
    this.displayedColumns = columns;
  }

  /* this function is called when the setContents input is set in the parent component */
  @Input() set setContents(contents: TableContent[]) {
    this.dataSource = new MatTableDataSource(contents);
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }
}
