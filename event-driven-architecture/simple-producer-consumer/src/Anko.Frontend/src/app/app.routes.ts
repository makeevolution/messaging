import { Routes } from '@angular/router';
import { ViewAllItemsComponent } from './components/view-all-items/view-all-items.component';
import { Paths } from './static-data/paths';
import { LoginComponent } from './components/login/login.component';
export const routes: Routes = [
    {path: Paths.ViewAllItems, component: ViewAllItemsComponent, title: 'View All Items'},
    {path: Paths.Login, component: LoginComponent, title: 'Login'},
    {path: '**', redirectTo: Paths.Login, pathMatch: 'full'},
];
