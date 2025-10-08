import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MatToolbarModule, MatIconModule],
  template: `
    <mat-toolbar color="primary">
      <mat-icon>shopping_cart</mat-icon>
      <span>Inventory Management System</span>
    </mat-toolbar>
    <main>
      <router-outlet />
    </main>
  `,
  styles: [`
    mat-toolbar {
      mat-icon {
        margin-right: 12px;
        font-size: 28px;
        width: 28px;
        height: 28px;
      }
    }

    main {
      min-height: calc(100vh - 64px);
      background: #f5f5f5;
    }
  `]
})
export class App {
  title = 'Inventory Management';
}
