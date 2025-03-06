import { CommonModule } from '@angular/common';
import { AfterViewInit, Component } from '@angular/core';
import { SidebarNode } from 'src/app/static-data/sidebar';
import { MatTreeModule } from '@angular/material/tree';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';

const TREE_DATA: SidebarNode[] = [
  {
    name: 'Fruit',
    children: [{name: 'Apple'}, {name: 'Banana'}, {name: 'Fruit loops'}],
  },
  {
    name: 'Vegetables',
    children: [
      {
        name: 'Green',
        children: [{name: 'Broccoli'}, {name: 'Brussels sprouts'}],
      },
      {
        name: 'Orange',
        children: [{name: 'Pumpkins'}, {name: 'Carrots'}],
      },
    ],
  },
];
@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    MatTreeModule,
    MatSidenavModule,
    MatTreeModule,
    MatIconModule,
    MatButtonModule,
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements AfterViewInit {
  dataSource = TREE_DATA;

  childrenAccessor = (node: SidebarNode) => node.children ?? [];

  hasChild = (_: number, node: SidebarNode) => !!node.children && node.children.length > 0;
  
  ngAfterViewInit(): void {
    
  }
}
