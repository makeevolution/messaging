import { Routes } from '@angular/router';
import { ViewAllProductsComponent } from './components/product/view-all-products/view-all-products.component';
import { Paths } from './static-data/paths';
import { LoginComponent } from './components/login/login.component';
export const routes: Routes = [
    {path: Paths.ViewAllProducts, component: ViewAllProductsComponent, title: 'View All Items'},
    {path: Paths.Login, component: LoginComponent, title: 'Login'},
    {path: '**', redirectTo: Paths.Login, pathMatch: 'full'},
];
