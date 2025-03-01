import { Routes } from '@angular/router';
import { ViewAllItemsComponent } from './components/view-all-items/view-all-items.component';
import { Paths } from './static-data/paths';
export const routes: Routes = [
    {path: Paths.ViewAllItems, component: ViewAllItemsComponent, title: 'View All Items'},
    {path: '**', redirectTo: Paths.ViewAllItems, pathMatch: 'full'},
];
