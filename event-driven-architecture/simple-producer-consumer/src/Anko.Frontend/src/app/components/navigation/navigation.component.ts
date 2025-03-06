import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { LayoutService } from 'src/app/services/layout.service';
@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
  ],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.scss'
})
export class NavigationComponent {
    constructor(
      private layoutService: LayoutService
    ) { }

    // The sidebar is initialized in layout.component.ts
    toggleSidebar(): void {
      this.layoutService.toggleSidebar();
    }
    
    isExpanded(): boolean {
      return this.layoutService.isSidebarExpanded();
    }
}