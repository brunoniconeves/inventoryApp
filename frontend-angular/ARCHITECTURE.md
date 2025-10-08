# Angular Frontend Architecture

## Overview

This Angular 20 application demonstrates modern Angular development practices with signal-based state management, composite component architecture, and comprehensive form validation.

## Key Technologies

- **Angular 20**: Latest version with standalone components
- **@ngrx/signals**: Signal-based state management
- **Angular Material**: UI component library
- **RxJS**: Reactive programming
- **TypeScript**: Type-safe development
- **Jasmine/Karma**: Testing framework

## Architecture Patterns

### 1. Signal-Based State Management

#### Global State (ProductStore)
Located in `src/app/store/product.store.ts`

```typescript
export const ProductStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withComputed((store) => ({
    productsCount: computed(() => store.products().length),
    hasProducts: computed(() => store.products().length > 0),
  })),
  withMethods((store, productApi) => ({
    loadProducts: rxMethod<void>(...),
    createProduct: rxMethod<ProductCreate>(...),
    updateProduct: rxMethod<ProductUpdate>(...),
    deleteProduct: rxMethod<number>(...),
  })),
  withHooks({
    onInit(store) {
      store.loadProducts();
    },
  })
);
```

**Features:**
- Centralized product state
- Computed signals for derived values
- RxMethod for async operations
- Automatic initialization with hooks
- Type-safe methods

#### Component-Level Signals (InventoryManagerComponent)
Located in `src/app/components/inventory-manager/inventory-manager.component.ts`

```typescript
export class InventoryManagerComponent {
  // Component signals
  product = signal<Product | null>(null);
  inventory = signal<Inventory | null>(null);
  loading = signal(true);
  
  // Computed signals
  currentStock = computed(() => this.inventory()?.quantity ?? 0);
  totalValue = computed(() => {
    const stock = this.currentStock();
    const price = this.product()?.price ?? 0;
    return stock * price;
  });
}
```

**Features:**
- Local component state
- Reactive computations
- Automatic UI updates
- No manual change detection

### 2. Composite Component Architecture

#### Component Hierarchy

```
App (Root)
├── ProductListComponent (Smart Component)
│   ├── Uses ProductStore
│   ├── Handles navigation
│   └── Displays product table
│
├── ProductFormComponent (Smart Component)
│   ├── Uses ProductStore
│   ├── Reactive forms
│   └── Validation logic
│
└── InventoryManagerComponent (Smart Component)
    ├── Local signals
    ├── API integration
    └── Stock management
```

#### Smart Components
- **ProductListComponent**: Consumes global store, handles user interactions
- **ProductFormComponent**: Form management with validation
- **InventoryManagerComponent**: Local state management with signals

### 3. Form Validation

#### Reactive Forms with Validators

```typescript
productForm: FormGroup = this.fb.group({
  name: ['', [Validators.required, Validators.minLength(3)]],
  description: ['', [Validators.required, Validators.minLength(10)]],
  price: [0, [Validators.required, Validators.min(0.01)]],
});
```

**Validation Features:**
- Built-in validators (required, minLength, min)
- Real-time validation feedback
- Custom error messages
- Touch state tracking
- Form-level and field-level validation

#### Dynamic Validators

```typescript
private updateRemoveStockValidator(): void {
  const currentStock = this.currentStock();
  this.removeStockForm.get('quantity')?.setValidators([
    Validators.required,
    Validators.min(1),
    Validators.max(currentStock)
  ]);
  this.removeStockForm.get('quantity')?.updateValueAndValidity();
}
```

### 4. Service Layer

#### ProductApiService
Located in `src/app/services/product-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class ProductApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api';

  getAllProducts(): Observable<Product[]> { ... }
  createProduct(product: ProductCreate): Observable<Product> { ... }
  updateProduct(id: number, product: ProductUpdate): Observable<Product> { ... }
  deleteProduct(id: number): Observable<void> { ... }
  // ... inventory methods
}
```

**Features:**
- Dependency injection with `inject()`
- Type-safe API calls
- Observable-based async operations
- Centralized HTTP logic

### 5. Testing Strategy

#### Service Testing
```typescript
describe('ProductApiService', () => {
  let service: ProductApiService;
  let httpMock: HttpTestingController;

  it('should fetch all products', () => {
    service.getAllProducts().subscribe(products => {
      expect(products).toEqual(mockProducts);
    });

    const req = httpMock.expectOne(`${baseUrl}/products`);
    expect(req.request.method).toBe('GET');
    req.flush(mockProducts);
  });
});
```

#### Component Testing
```typescript
describe('ProductFormComponent', () => {
  it('should validate required fields', () => {
    const form = component.productForm;
    expect(form.valid).toBeFalse();
    
    form.get('name')?.setValue('');
    expect(form.get('name')?.hasError('required')).toBeTrue();
  });
});
```

### 6. Routing Configuration

```typescript
export const routes: Routes = [
  { path: '', redirectTo: '/products', pathMatch: 'full' },
  { path: 'products', component: ProductListComponent },
  { path: 'products/new', component: ProductFormComponent },
  { path: 'products/edit/:id', component: ProductFormComponent },
  { path: 'inventory/:id', component: InventoryManagerComponent },
  { path: '**', redirectTo: '/products' }
];
```

## Best Practices Implemented

### 1. Standalone Components
- No NgModules required
- Better tree-shaking
- Simpler dependency management

### 2. Signal-Based Reactivity
- Fine-grained reactivity
- Better performance
- Simpler mental model

### 3. Dependency Injection
- Using `inject()` function
- Constructor injection alternative
- Type-safe injection

### 4. Type Safety
- Full TypeScript interfaces
- Generic types for type safety
- Strict null checks

### 5. Error Handling
- Try-catch blocks in async operations
- User-friendly error messages
- Graceful degradation

### 6. Code Organization
- Feature-based structure
- Single responsibility principle
- Separation of concerns

### 7. Performance Optimization
- OnPush change detection (via signals)
- Lazy loading ready
- Efficient rendering

## Data Flow

### Create Product Flow
```
User Input → ProductFormComponent
           → Validate Form
           → ProductStore.createProduct()
           → ProductApiService.createProduct()
           → Backend API
           → Update Store State
           → UI Auto-Updates
```

### Inventory Management Flow
```
User Action → InventoryManagerComponent
           → Local Signal Update
           → ProductApiService (add/remove stock)
           → Backend API
           → Reload Inventory
           → Computed Signals Update
           → UI Reflects Changes
```

## Running the Application

### Development
```bash
npm start
# Access at http://localhost:4200
```

### Production Build
```bash
npm run build
# Outputs to dist/frontend-angular/browser
```

### Docker
```bash
docker-compose -f docker-compose-angular.yml up --build
# Access at http://localhost:4200
```

### Testing
```bash
npm test                # Run tests
npm run test:coverage   # With coverage report
```

## Environment Configuration

The app connects to the backend API at `http://localhost:5000/api`. This can be configured in:
- `src/app/services/product-api.service.ts` (baseUrl property)
- Environment files (for production deployments)

## Deployment

The Dockerfile builds a production-ready image:

1. **Build Stage**: Compiles Angular app with AOT compilation
2. **Runtime Stage**: Serves with Nginx
3. **Optimizations**: Gzip compression, caching headers, security headers

## Future Enhancements

1. **State Persistence**: LocalStorage integration
2. **Real-time Updates**: WebSocket support
3. **Advanced Filtering**: Search and filter capabilities
4. **Pagination**: For large datasets
5. **Internationalization**: i18n support
6. **PWA Features**: Offline support, service workers
7. **E2E Tests**: Cypress or Playwright integration
8. **Performance Monitoring**: Angular DevTools integration

