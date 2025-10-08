# Inventory Management System - Angular Frontend

A modern Angular 20 frontend for the Inventory Management System, built with the latest Angular features and best practices.

## 🚀 Features

- **Angular 20** with standalone components
- **Signal-based state management** using `@ngrx/signals`
- **Angular Material** for UI components
- **Reactive Forms** with comprehensive validation
- **Composite Component Architecture**
- **Unit Tests** with Jasmine/Karma
- **TypeScript** for type safety
- **Responsive Design**

## 📋 Architecture

### State Management

The application uses **NgRx Signals** for global state management:

- **ProductStore**: Global signal store managing products state
- **Component Signals**: Local signals for component-specific state

### Component Structure

```
app/
├── components/
│   ├── product-list/          # Lists all products (uses global store)
│   ├── product-form/           # Create/Edit product form (with validation)
│   └── inventory-manager/      # Manage product inventory (local signals)
├── models/
│   └── product.model.ts        # TypeScript interfaces
├── services/
│   └── product-api.service.ts  # HTTP API service
└── store/
    └── product.store.ts        # NgRx Signal Store
```

### Key Concepts Demonstrated

1. **Signal Store (Global State)**
   - `ProductStore` manages products using `@ngrx/signals`
   - Computed signals for derived state
   - RxMethod for async operations

2. **Component Signals (Local State)**
   - `InventoryManagerComponent` uses signals for local state
   - Computed values for calculations
   - Effects for reactive updates

3. **Form Validation**
   - Reactive Forms with custom validators
   - Real-time validation feedback
   - Type-safe form controls

4. **Composite Architecture**
   - Reusable, single-responsibility components
   - Smart/Presentational component pattern
   - Dependency injection

## 🛠️ Development

### Prerequisites

- Node.js 20+
- npm or yarn

### Install Dependencies

```bash
npm install
```

### Development Server

```bash
npm start
```

Navigate to `http://localhost:4200/`

### Running Tests

```bash
# Run unit tests
npm test

# Run tests with coverage
npm run test:coverage
```

### Build

```bash
# Production build
npm run build

# The build artifacts will be in `dist/`
```

## 🐳 Docker

### Build Docker Image

```bash
docker build -t inventory-angular .
```

### Run with Docker Compose

From the project root:

```bash
docker-compose -f docker-compose-angular.yml up --build
```

Access the application at `http://localhost:4200`

## 📝 Form Validations

### Product Form

- **Name**: Required, minimum 3 characters
- **Description**: Required, minimum 10 characters
- **Price**: Required, must be greater than 0

### Inventory Form

- **Add Stock**: Positive integer required
- **Remove Stock**: Cannot exceed available stock

## 🧪 Testing

The project includes comprehensive unit tests:

- **Service Tests**: HTTP API mocking with HttpClientTestingModule
- **Component Tests**: Component behavior and form validation
- **Store Tests**: State management logic (can be added)

### Test Coverage

Run tests with coverage:

```bash
npm run test:coverage
```

## 🎨 Styling

- **Angular Material** theme with custom configuration
- **SCSS** for component styles
- **Responsive** design patterns

## 📚 Best Practices Applied

1. **Standalone Components**: No NgModules, modern Angular approach
2. **Signals**: New reactivity system for better performance
3. **Dependency Injection**: Using `inject()` function
4. **Type Safety**: Full TypeScript typing
5. **Reactive Programming**: RxJS for async operations
6. **Component Composition**: Small, focused components
7. **Lazy Loading**: Route-based code splitting ready
8. **Error Handling**: Comprehensive error management
9. **Accessibility**: Material components with a11y support
10. **Testing**: Unit tests for critical functionality

## 🔗 API Integration

The frontend communicates with the backend API at `http://localhost:5000/api`

### Endpoints Used

- `GET /products` - List all products
- `GET /products/:id` - Get product details
- `POST /products` - Create product
- `PUT /products/:id` - Update product
- `DELETE /products/:id` - Delete product
- `GET /inventory/products/:id` - Get inventory
- `POST /inventory/products/:id/stock` - Add stock
- `DELETE /inventory/products/:id/stock` - Remove stock

## 🚢 Deployment

The application is containerized and can be deployed to:

- **Docker/Kubernetes**
- **Azure App Service**
- **AWS Elastic Beanstalk**
- **Google Cloud Run**
- **Any static hosting** (after build)

## 📄 License

This project is part of the Inventory Management System.
