import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProductApiService } from '../../services/product-api.service';
import { Product, Inventory } from '../../models/product.model';

@Component({
  selector: 'app-inventory-manager',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
  ],
  template: `
    <div class="inventory-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>
            <div class="title-with-icon">
              <mat-icon>inventory_2</mat-icon>
              <div>
                <h1>Inventory Management</h1>
                @if (product()) {
                  <h2>{{ product()!.name }}</h2>
                }
              </div>
            </div>
          </mat-card-title>
        </mat-card-header>

        <mat-card-content>
          @if (loading()) {
            <div class="loading-spinner">
              <mat-spinner></mat-spinner>
            </div>
          } @else {
            <div class="inventory-info">
              <div class="info-item">
                <mat-icon>warehouse</mat-icon>
                <div>
                  <p class="label">Current Stock</p>
                  <p class="value">{{ currentStock() }} units</p>
                </div>
              </div>

              <div class="info-item">
                <mat-icon>payments</mat-icon>
                <div>
                  <p class="label">Product Price</p>
                  <p class="value">{{ product()?.price | currency }}</p>
                </div>
              </div>

              <div class="info-item">
                <mat-icon>account_balance_wallet</mat-icon>
                <div>
                  <p class="label">Total Value</p>
                  <p class="value">{{ totalValue() | currency }}</p>
                </div>
              </div>
            </div>

            <div class="stock-actions">
              <form [formGroup]="addStockForm" (ngSubmit)="addStock()">
                <mat-form-field appearance="outline">
                  <mat-label>Add Stock</mat-label>
                  <input matInput type="number" formControlName="quantity" min="1">
                  @if (addStockForm.get('quantity')?.hasError('required')) {
                    <mat-error>Quantity is required</mat-error>
                  }
                  @if (addStockForm.get('quantity')?.hasError('min')) {
                    <mat-error>Quantity must be at least 1</mat-error>
                  }
                </mat-form-field>
                <button mat-raised-button color="primary" type="submit" [disabled]="addStockForm.invalid">
                  <mat-icon>add_circle_outline</mat-icon>
                  Add Stock
                </button>
              </form>

              <form [formGroup]="removeStockForm" (ngSubmit)="removeStock()">
                <mat-form-field appearance="outline">
                  <mat-label>Remove Stock</mat-label>
                  <input matInput type="number" formControlName="quantity" min="1">
                  @if (removeStockForm.get('quantity')?.hasError('required')) {
                    <mat-error>Quantity is required</mat-error>
                  }
                  @if (removeStockForm.get('quantity')?.hasError('min')) {
                    <mat-error>Quantity must be at least 1</mat-error>
                  }
                  @if (removeStockForm.get('quantity')?.hasError('max')) {
                    <mat-error>Cannot remove more than available stock</mat-error>
                  }
                </mat-form-field>
                <button mat-raised-button color="warn" type="submit" [disabled]="removeStockForm.invalid">
                  <mat-icon>remove_circle_outline</mat-icon>
                  Remove Stock
                </button>
              </form>
            </div>

            <div class="form-actions">
              <button mat-raised-button (click)="goBack()">
                <mat-icon>arrow_back</mat-icon>
                Back to Products
              </button>
            </div>
          }
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .inventory-container {
      padding: 20px;
      max-width: 800px;
      margin: 0 auto;
    }

    .title-with-icon {
      display: flex;
      align-items: flex-start;
      gap: 16px;

      > mat-icon {
        font-size: 40px;
        width: 40px;
        height: 40px;
        color: #1976d2;
        margin-top: 4px;
      }

      h1 {
        margin: 0 0 8px 0;
      }

      h2 {
        color: rgba(0, 0, 0, 0.6);
        font-size: 18px;
        font-weight: 400;
        margin: 0;
      }
    }

    .loading-spinner {
      display: flex;
      justify-content: center;
      padding: 40px;
    }

    .inventory-info {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 20px;
      margin: 20px 0;
      padding: 20px;
      background: #f5f5f5;
      border-radius: 8px;
    }

    .info-item {
      display: flex;
      align-items: center;
      gap: 12px;

      mat-icon {
        color: #1976d2;
        font-size: 36px;
        width: 36px;
        height: 36px;
      }

      .label {
        font-size: 12px;
        color: rgba(0, 0, 0, 0.6);
        margin: 0;
      }

      .value {
        font-size: 24px;
        font-weight: 500;
        margin: 4px 0 0 0;
      }
    }

    .stock-actions {
      display: flex;
      gap: 20px;
      margin: 30px 0;

      form {
        flex: 1;
        display: flex;
        gap: 12px;
        align-items: flex-start;

        mat-form-field {
          flex: 1;
        }

        button {
          mat-icon {
            margin-right: 8px;
            vertical-align: middle;
          }
        }
      }
    }

    .form-actions {
      margin-top: 20px;

      button {
        mat-icon {
          margin-right: 8px;
          vertical-align: middle;
        }
      }
    }
  `]
})
export class InventoryManagerComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productApi = inject(ProductApiService);
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  // Component signals for local state management
  product = signal<Product | null>(null);
  inventory = signal<Inventory | null>(null);
  loading = signal(true);
  
  currentStock = computed(() => this.inventory()?.currentStock ?? 0);
  totalValue = computed(() => {
    const stock = this.currentStock();
    const price = this.product()?.price ?? 0;
    return stock * price;
  });

  addStockForm: FormGroup = this.fb.group({
    quantity: [1, [Validators.required, Validators.min(1)]],
  });

  removeStockForm: FormGroup = this.fb.group({
    quantity: [1, [Validators.required, Validators.min(1)]],
  });

  ngOnInit(): void {
    const productId = this.route.snapshot.paramMap.get('id');
    
    if (productId) {
      this.loadProductData(+productId);
    }
  }

  private loadProductData(productId: number): void {
    this.loading.set(true);

    this.productApi.getProduct(productId).subscribe({
      next: (product) => {
        this.product.set(product);
        this.loadInventory(productId);
      },
      error: (error) => {
        this.snackBar.open('Error loading product', 'Close', { duration: 3000 });
        this.loading.set(false);
      }
    });
  }

  private loadInventory(productId: number): void {
    this.productApi.getProductInventory(productId).subscribe({
      next: (inventory) => {
        console.log('Inventory data received:', inventory);
        this.inventory.set(inventory);
        this.updateRemoveStockValidator();
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading inventory:', error);
        this.snackBar.open('Error loading inventory', 'Close', { duration: 3000 });
        this.loading.set(false);
      }
    });
  }

  private updateRemoveStockValidator(): void {
    const currentStock = this.currentStock();
    this.removeStockForm.get('quantity')?.setValidators([
      Validators.required,
      Validators.min(1),
      Validators.max(currentStock)
    ]);
    this.removeStockForm.get('quantity')?.updateValueAndValidity();
  }

  addStock(): void {
    if (this.addStockForm.invalid || !this.product()) return;

    const quantity = this.addStockForm.value.quantity;
    const productId = this.product()!.id;

    this.productApi.addStock(productId, quantity).subscribe({
      next: () => {
        this.loadInventory(productId);
        this.addStockForm.reset({ quantity: 1 });
        this.snackBar.open(`Added ${quantity} units to stock`, 'Close', { duration: 3000 });
      },
      error: (error) => {
        this.snackBar.open('Error adding stock', 'Close', { duration: 3000 });
      }
    });
  }

  removeStock(): void {
    if (this.removeStockForm.invalid || !this.product()) return;

    const quantity = this.removeStockForm.value.quantity;
    const productId = this.product()!.id;

    this.productApi.removeStock(productId, quantity).subscribe({
      next: () => {
        this.loadInventory(productId);
        this.removeStockForm.reset({ quantity: 1 });
        this.snackBar.open(`Removed ${quantity} units from stock`, 'Close', { duration: 3000 });
      },
      error: (error) => {
        this.snackBar.open(error.error?.detail || 'Error removing stock', 'Close', { duration: 3000 });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/products']);
  }
}

