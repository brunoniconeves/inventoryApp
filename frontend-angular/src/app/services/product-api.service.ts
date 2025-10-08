import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, ProductCreate, ProductUpdate, Inventory, StockUpdate } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api';

  // Product endpoints
  getAllProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}/products`);
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/products/${id}`);
  }

  createProduct(product: ProductCreate): Observable<Product> {
    return this.http.post<Product>(`${this.baseUrl}/products`, product);
  }

  updateProduct(id: number, product: ProductUpdate): Observable<Product> {
    return this.http.put<Product>(`${this.baseUrl}/products/${id}`, product);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/products/${id}`);
  }

  // Inventory endpoints
  getProductInventory(productId: number): Observable<Inventory> {
    return this.http.get<Inventory>(`${this.baseUrl}/inventory/products/${productId}`);
  }

  addStock(productId: number, quantity: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/inventory/products/${productId}/stock`, { quantity });
  }

  removeStock(productId: number, quantity: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/inventory/products/${productId}/stock`, { 
      body: { quantity } 
    });
  }
}

