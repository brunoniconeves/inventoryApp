import { Component, OnInit, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProductStore } from '../../store/product.store';
import { ProductCreate, ProductUpdate } from '../../models/product.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
  ],
  template: `
    <div class="product-form-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>
            <div class="title-with-icon">
              <mat-icon>{{ isEditMode() ? 'edit_note' : 'add_shopping_cart' }}</mat-icon>
              <h1>{{ isEditMode() ? 'Edit Product' : 'Add New Product' }}</h1>
            </div>
          </mat-card-title>
        </mat-card-header>

        <mat-card-content>
          <form [formGroup]="productForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Product Name</mat-label>
              <input matInput formControlName="name" placeholder="Enter product name">
              @if (productForm.get('name')?.hasError('required') && productForm.get('name')?.touched) {
                <mat-error>Product name is required</mat-error>
              }
              @if (productForm.get('name')?.hasError('minlength')) {
                <mat-error>Product name must be at least 3 characters</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>SKU</mat-label>
              <input matInput formControlName="sku" placeholder="Enter product SKU">
              @if (productForm.get('sku')?.hasError('required') && productForm.get('sku')?.touched) {
                <mat-error>SKU is required</mat-error>
              }
              @if (productForm.get('sku')?.hasError('minlength')) {
                <mat-error>SKU must be at least 3 characters</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Description</mat-label>
              <textarea 
                matInput 
                formControlName="description" 
                placeholder="Enter product description"
                rows="4"
              ></textarea>
              @if (productForm.get('description')?.hasError('required') && productForm.get('description')?.touched) {
                <mat-error>Description is required</mat-error>
              }
              @if (productForm.get('description')?.hasError('minlength')) {
                <mat-error>Description must be at least 10 characters</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Price</mat-label>
              <input matInput type="number" formControlName="price" placeholder="Enter price" step="0.01">
              <span matPrefix>$ &nbsp;</span>
              @if (productForm.get('price')?.hasError('required') && productForm.get('price')?.touched) {
                <mat-error>Price is required</mat-error>
              }
              @if (productForm.get('price')?.hasError('min')) {
                <mat-error>Price must be greater than 0</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Initial Stock Quantity</mat-label>
              <input matInput type="number" formControlName="stockQuantity" placeholder="Enter initial stock" min="0">
              @if (productForm.get('stockQuantity')?.hasError('required') && productForm.get('stockQuantity')?.touched) {
                <mat-error>Stock quantity is required</mat-error>
              }
              @if (productForm.get('stockQuantity')?.hasError('min')) {
                <mat-error>Stock quantity must be 0 or greater</mat-error>
              }
            </mat-form-field>

            <div class="form-actions">
              <button mat-raised-button type="button" (click)="onCancel()">
                <mat-icon>arrow_back</mat-icon>
                Cancel
              </button>
              <button 
                mat-raised-button 
                color="primary" 
                type="submit"
                [disabled]="productForm.invalid || isSubmitting()"
              >
                @if (isSubmitting()) {
                  <mat-spinner diameter="20"></mat-spinner>
                } @else {
                  <ng-container>
                    <mat-icon>{{ isEditMode() ? 'save' : 'add_circle' }}</mat-icon>
                    {{ isEditMode() ? 'Update' : 'Create' }}
                  </ng-container>
                }
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .product-form-container {
      padding: 20px;
      max-width: 600px;
      margin: 0 auto;
    }

    .title-with-icon {
      display: flex;
      align-items: center;
      gap: 12px;

      mat-icon {
        font-size: 32px;
        width: 32px;
        height: 32px;
        color: #1976d2;
      }

      h1 {
        margin: 0;
      }
    }

    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    .form-actions {
      display: flex;
      gap: 12px;
      justify-content: flex-end;
      margin-top: 24px;

      button {
        mat-icon {
          margin-right: 8px;
          vertical-align: middle;
        }
      }
    }

    mat-card-content {
      padding: 32px 16px;
    }

    mat-spinner {
      display: inline-block;
      margin: 0 auto;
    }
  `]
})
export class ProductFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  readonly store = inject(ProductStore);

  // Component-level signals for local state
  isEditMode = signal(false);
  isSubmitting = signal(false);
  productId = signal<number | null>(null);

  productForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    sku: ['', [Validators.required, Validators.minLength(3)]],
    description: ['', [Validators.required, Validators.minLength(10)]],
    price: [0, [Validators.required, Validators.min(0.01)]],
    stockQuantity: [0, [Validators.required, Validators.min(0)]],
  });

  // Effect to handle form population when editing
  constructor() {
    effect(() => {
      const selectedProduct = this.store.selectedProduct();
      if (selectedProduct && this.isEditMode()) {
        this.productForm.patchValue({
          name: selectedProduct.name,
          sku: selectedProduct.sku,
          description: selectedProduct.description,
          price: selectedProduct.price,
          stockQuantity: selectedProduct.stockQuantity,
        });
      }
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    
    if (id && id !== 'new') {
      this.isEditMode.set(true);
      this.productId.set(+id);
      
      // Find and select the product from store
      const product = this.store.products().find(p => p.id === +id);
      if (product) {
        this.store.selectProduct(product);
      }
    }
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      Object.keys(this.productForm.controls).forEach(key => {
        this.productForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.isSubmitting.set(true);
    const formValue = this.productForm.value;

    if (this.isEditMode() && this.productId()) {
      const updateData: ProductUpdate = {
        id: this.productId()!,
        ...formValue
      };
      
      this.store.updateProduct(updateData);
      this.snackBar.open('Product updated successfully', 'Close', { duration: 3000 });
    } else {
      const createData: ProductCreate = formValue;
      this.store.createProduct(createData);
      this.snackBar.open('Product created successfully', 'Close', { duration: 3000 });
    }

    // Navigate back after a short delay
    setTimeout(() => {
      this.isSubmitting.set(false);
      this.router.navigate(['/products']);
    }, 500);
  }

  onCancel(): void {
    this.router.navigate(['/products']);
  }
}

