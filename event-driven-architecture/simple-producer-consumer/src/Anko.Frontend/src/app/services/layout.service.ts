import { Injectable } from "@angular/core";
import { MatSidenav } from "@angular/material/sidenav";

// This service is used to control the layout of the application
// For now, it only controls the sidebar
@Injectable({
    providedIn: 'root'
  })
export class LayoutService {
    isExpanded: boolean = false;
    private sidebar!: MatSidenav;

    setSidebar(sidebar: MatSidenav): void {
        this.sidebar = sidebar;
    }

    isSidebarExpanded(): boolean {
        return this.isExpanded;
    }

    toggleSidebar(): void {
        this.sidebar.toggle();
        this.isExpanded = !this.isExpanded;
    }
}