# Quick Start Guide - Angular Inventory Frontend

## 🚀 Get Started in 3 Steps

### Step 1: Install Dependencies
```bash
cd frontend-angular
npm install
```

### Step 2: Start Development Server
```bash
npm start
```

### Step 3: Open in Browser
Navigate to `http://localhost:4200`

## 🐳 Using Docker (Recommended)

From the project root directory:

```bash
# Build and start all services (Frontend + Backend + Database)
docker-compose -f docker-compose-angular.yml up --build

# Access the application
# Frontend: http://localhost:4200
# Backend API: http://localhost:5000
# Swagger Docs: http://localhost:5000/swagger
```

## 📋 Available Commands

| Command | Description |
|---------|-------------|
| `npm start` | Start development server |
| `npm run build` | Build for production |
| `npm test` | Run unit tests |
| `npm run test:coverage` | Run tests with coverage |
| `npm run lint` | Lint the code |

## 🎯 Main Features

### 1. Product List
- View all products in a table
- Edit, Delete, and View Inventory actions
- Add new products

### 2. Product Form (Create/Edit)
- Name validation (min 3 characters)
- Description validation (min 10 characters)  
- Price validation (must be > 0)
- Real-time validation feedback

### 3. Inventory Management
- View current stock
- Add stock to products
- Remove stock from products
- See total inventory value

## 🏗️ Project Structure

```
frontend-angular/
├── src/
│   ├── app/
│   │   ├── components/          # UI Components
│   │   │   ├── product-list/
│   │   │   ├── product-form/
│   │   │   └── inventory-manager/
│   │   ├── models/              # TypeScript interfaces
│   │   ├── services/            # API services
│   │   └── store/               # Signal store
│   ├── styles.scss              # Global styles
│   └── main.ts                  # App entry point
├── Dockerfile                   # Production container
├── nginx.conf                   # Nginx config
└── package.json                 # Dependencies
```

## 🧪 Testing

### Run All Tests
```bash
npm test
```

### Run Tests with Coverage
```bash
npm run test:coverage
```

### Test Files Location
- `*.spec.ts` files alongside components
- Service tests in `services/`
- Component tests in `components/`

## 🔧 Configuration

### API Endpoint
Edit `src/app/services/product-api.service.ts`:
```typescript
private readonly baseUrl = 'http://localhost:5000/api';
```

### Angular Material Theme
Edit `src/styles.scss` to customize colors:
```scss
$primary-palette: mat.$azure-palette;
$accent-palette: mat.$pink-palette;
```

## 📦 Build for Production

```bash
# Create production build
npm run build

# Output location
dist/frontend-angular/browser/

# Serve with any static server
npx http-server dist/frontend-angular/browser
```

## 🐛 Troubleshooting

### Port Already in Use
```bash
# Change port in angular.json or use:
ng serve --port 4201
```

### API Connection Issues
1. Ensure backend is running on port 5000
2. Check CORS settings on backend
3. Verify API URL in `product-api.service.ts`

### Build Errors
```bash
# Clean install
rm -rf node_modules package-lock.json
npm install
```

## 🎓 Learning Resources

### Key Concepts Used

1. **Signals**: New reactivity primitive in Angular
   - `signal()` - Create reactive values
   - `computed()` - Derived values
   - `effect()` - Side effects

2. **NgRx Signals**: State management
   - `signalStore` - Create stores
   - `withState` - Add state
   - `withComputed` - Add computations
   - `withMethods` - Add actions

3. **Reactive Forms**: Form handling
   - `FormGroup` - Form container
   - `FormControl` - Form fields
   - `Validators` - Validation rules

4. **Standalone Components**: Modern Angular
   - No NgModules needed
   - Import dependencies directly
   - Better tree-shaking

## 🔗 Useful Links

- [Angular Documentation](https://angular.dev)
- [Angular Material](https://material.angular.io)
- [NgRx Signals](https://ngrx.io/guide/signals)
- [RxJS Documentation](https://rxjs.dev)

## 💡 Pro Tips

1. **Use Angular DevTools**: Install browser extension for debugging
2. **Type Safety**: Always define interfaces for your data
3. **Code Splitting**: Use lazy loading for routes
4. **Performance**: Use OnPush change detection (signals do this automatically)
5. **Testing**: Write tests as you develop components

## 🚢 Deployment Options

### Docker Container
```bash
docker build -t inventory-angular .
docker run -p 80:80 inventory-angular
```

### Cloud Platforms
- **Azure**: Azure App Service
- **AWS**: Elastic Beanstalk / S3 + CloudFront
- **GCP**: Cloud Run / Firebase Hosting
- **Vercel**: `npm run build` + upload dist folder
- **Netlify**: Connect GitHub repo

## 🤝 Need Help?

1. Check the `ARCHITECTURE.md` for detailed documentation
2. Review `README.md` for comprehensive guide
3. Look at component tests for usage examples
4. Inspect browser console for errors

Happy coding! 🎉

