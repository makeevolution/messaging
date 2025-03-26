import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../environments/environment.development";
import { Observable } from "rxjs";

export const WAREHOUSE_URL = environment.warehouseUrl;

// Injectable is a decorator that marks a class as available to be provided and injected as a dependency through DI.
@Injectable({
    providedIn: 'root'  // This tells Angular that the service should be provided in the root injector (i.e. it becomes a singleton)
})
export class ApiService {
    constructor (
        private http: HttpClient
    ){}

    getProducts(page=1, limit=10): Observable<any> {
        return this.http.get(`${WAREHOUSE_URL}/items`);
    }
}