import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router } from '@angular/router';
import { ProductStore } from '../../store/product.store';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatSnackBarModule,
    MatTooltipModule,
  ],
  template: `
    <div class="product-list-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>
            <div class="title-with-icon">
              <mat-icon>shopping_cart</mat-icon>
              <h1>Products Inventory</h1>
            </div>
          </mat-card-title>
          <button mat-raised-button color="primary" (click)="onAddProduct()">
            <mat-icon>add_shopping_cart</mat-icon>
            Add Product
          </button>
        </mat-card-header>

        <mat-card-content>
          @if (store.isLoading()) {
            <div class="loading-spinner">
              <mat-spinner></mat-spinner>
            </div>
          } @else if (store.hasProducts()) {
            <table mat-table [dataSource]="store.products()" class="mat-elevation-z8">
              <!-- ID Column -->
              <ng-container matColumnDef="id">
                <th mat-header-cell *matHeaderCellDef>ID</th>
                <td mat-cell *matCellDef="let product">{{ product.id }}</td>
              </ng-container>

              <!-- Name Column -->
              <ng-container matColumnDef="name">
                <th mat-header-cell *matHeaderCellDef>Name</th>
                <td mat-cell *matCellDef="let product">{{ product.name }}</td>
              </ng-container>

              <!-- Description Column -->
              <ng-container matColumnDef="description">
                <th mat-header-cell *matHeaderCellDef>Description</th>
                <td mat-cell *matCellDef="let product">{{ product.description }}</td>
              </ng-container>

              <!-- Price Column -->
              <ng-container matColumnDef="price">
                <th mat-header-cell *matHeaderCellDef>Price</th>
                <td mat-cell *matCellDef="let product">{{ product.price | currency }}</td>
              </ng-container>

              <!-- Actions Column -->
              <ng-container matColumnDef="actions">
                <th mat-header-cell *matHeaderCellDef>Actions</th>
                <td mat-cell *matCellDef="let product">
                  <button mat-icon-button color="primary" (click)="onEditProduct(product)" matTooltip="Edit Product">
                    <mat-icon>edit_note</mat-icon>
                  </button>
                  <button mat-icon-button color="accent" (click)="onViewInventory(product)" matTooltip="Manage Stock">
                    <mat-icon>inventory_2</mat-icon>
                  </button>
                  <button mat-icon-button color="warn" (click)="onDeleteProduct(product)" matTooltip="Delete Product">
                    <mat-icon>delete_forever</mat-icon>
                  </button>
                </td>
              </ng-container>

              <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
              <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
            </table>
          } @else {
            <div class="empty-state">
              <mat-icon>production_quantity_limits</mat-icon>
              <p>No products found. Add your first product!</p>
            </div>
          }
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .product-list-container {
      padding: 20px;
    }

    mat-card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
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

    table {
      width: 100%;
    }

    .loading-spinner {
      display: flex;
      justify-content: center;
      padding: 40px;
    }

    .empty-state {
      text-align: center;
      padding: 60px 20px;
      color: rgba(0, 0, 0, 0.54);

      mat-icon {
        font-size: 72px;
        width: 72px;
        height: 72px;
        margin-bottom: 16px;
      }
    }
  `]
})
export class ProductListComponent {
  readonly store = inject(ProductStore);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  displayedColumns: string[] = ['id', 'name', 'description', 'price', 'actions'];

  onAddProduct(): void {
    this.router.navigate(['/products/new']);
  }

  onEditProduct(product: Product): void {
    this.router.navigate(['/products/edit', product.id]);
  }

  onViewInventory(product: Product): void {
    this.router.navigate(['/inventory', product.id]);
  }

  onDeleteProduct(product: Product): void {
    if (confirm(`Are you sure you want to delete "${product.name}"?`)) {
      this.store.deleteProduct(product.id);
      this.snackBar.open('Product deleted successfully', 'Close', {
        duration: 3000,
      });
    }
  }
}

