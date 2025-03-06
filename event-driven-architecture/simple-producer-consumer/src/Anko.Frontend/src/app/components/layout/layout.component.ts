import { ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { RouterModule, RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../shared/sidebar/sidebar.component';
import { LayoutService } from '../../services/layout.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    MatSidenavModule,
    SidebarComponent
  ],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  // The layout is controlled by MatSidenav, which is a Angular Material component
  // It consists of a main content (<mat-drawer-content>) and sidebar (<mat-drawer>)
  
  // The viewchild below targets the #sidebar element in the html file
  @ViewChild('sidebar') public sidebar!: MatSidenav;

  constructor(
    public layoutService: LayoutService,
    private cdr: ChangeDetectorRef
  ){}

  ngAfterViewInit(): void {
    this.layoutService.setSidebar(this.sidebar);
    this.layoutService.toggleSidebar();
    this.cdr.detectChanges();
  }
}
