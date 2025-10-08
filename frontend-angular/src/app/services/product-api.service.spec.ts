import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController } from '@angular/common/http/testing';
import { ProductApiService } from './product-api.service';
import { Product, ProductCreate } from '../models/product.model';

describe('ProductApiService', () => {
  let service: ProductApiService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:5000/api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        ProductApiService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(ProductApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAllProducts', () => {
    it('should fetch all products', () => {
      const mockProducts: Product[] = [
        { id: 1, name: 'Product 1', description: 'Desc 1', price: 10.99, sku: 'SKU001', stockQuantity: 10 },
        { id: 2, name: 'Product 2', description: 'Desc 2', price: 20.99, sku: 'SKU002', stockQuantity: 5 }
      ];

      service.getAllProducts().subscribe(products => {
        expect(products).toEqual(mockProducts);
        expect(products.length).toBe(2);
      });

      const req = httpMock.expectOne(`${baseUrl}/products`);
      expect(req.request.method).toBe('GET');
      req.flush(mockProducts);
    });
  });

  describe('getProduct', () => {
    it('should fetch a single product by id', () => {
      const mockProduct: Product = {
        id: 1,
        name: 'Test Product',
        description: 'Test Description',
        price: 99.99,
        sku: 'SKU001',
        stockQuantity: 15
      };

      service.getProduct(1).subscribe(product => {
        expect(product).toEqual(mockProduct);
      });

      const req = httpMock.expectOne(`${baseUrl}/products/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockProduct);
    });
  });

  describe('createProduct', () => {
    it('should create a new product', () => {
      const newProduct: ProductCreate = {
        name: 'New Product',
        description: 'New Description',
        price: 15.99,
        sku: 'SKU003',
        stockQuantity: 20
      };

      const createdProduct: Product = {
        id: 3,
        ...newProduct
      };

      service.createProduct(newProduct).subscribe(product => {
        expect(product).toEqual(createdProduct);
      });

      const req = httpMock.expectOne(`${baseUrl}/products`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(newProduct);
      req.flush(createdProduct);
    });
  });

  describe('updateProduct', () => {
    it('should update an existing product', () => {
      const updatedProduct: Product = {
        id: 1,
        name: 'Updated Product',
        description: 'Updated Description',
        price: 25.99,
        sku: 'SKU001-UPDATED',
        stockQuantity: 30
      };

      service.updateProduct(1, updatedProduct).subscribe(product => {
        expect(product).toEqual(updatedProduct);
      });

      const req = httpMock.expectOne(`${baseUrl}/products/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updatedProduct);
      req.flush(updatedProduct);
    });
  });

  describe('deleteProduct', () => {
    it('should delete a product', () => {
      service.deleteProduct(1).subscribe(response => {
        expect(response).toBeUndefined();
      });

      const req = httpMock.expectOne(`${baseUrl}/products/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null);
    });
  });

  describe('inventory operations', () => {
    it('should get product inventory', () => {
      const mockInventory = { 
        productId: 1, 
        productName: 'Test Product',
        sku: 'SKU001',
        currentStock: 50 
      };

      service.getProductInventory(1).subscribe(inventory => {
        expect(inventory).toEqual(mockInventory);
      });

      const req = httpMock.expectOne(`${baseUrl}/inventory/products/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockInventory);
    });

    it('should add stock', () => {
      service.addStock(1, 10).subscribe();

      const req = httpMock.expectOne(`${baseUrl}/inventory/products/1/stock`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ quantity: 10 });
      req.flush({});
    });

    it('should remove stock', () => {
      service.removeStock(1, 5).subscribe();

      const req = httpMock.expectOne(`${baseUrl}/inventory/products/1/stock`);
      expect(req.request.method).toBe('DELETE');
      expect(req.request.body).toEqual({ quantity: 5 });
      req.flush({});
    });
  });
});

