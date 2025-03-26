import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { LayoutService } from 'src/app/services/layout.service';
import { MatBadgeModule } from '@angular/material/badge';
import { Router } from '@angular/router';
import { Paths } from 'src/app/static-data/paths';
@Component({
  selector: 'app-navigation',
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatBadgeModule,
  ],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.scss'
})
export class NavigationComponent {
  
  loading=false;
  
  constructor(
      private layoutService: LayoutService,
      private router: Router
    ) { }

    // The sidebar is initialized in layout.component.ts
    toggleSidebar(): void {
      this.layoutService.toggleSidebar();
    }

    isExpanded(): boolean {
      return this.layoutService.isSidebarExpanded();
    }

      login(): void {
        this.loading = true;
        setTimeout(() => {
          this.router.navigate([Paths.Login])
        }, 2000);
      }
}