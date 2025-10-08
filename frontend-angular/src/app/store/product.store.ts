import { computed, inject } from '@angular/core';
import {
  patchState,
  signalStore,
  withComputed,
  withHooks,
  withMethods,
  withState,
} from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { pipe, switchMap, tap, catchError, of } from 'rxjs';
import { Product, ProductCreate, ProductUpdate } from '../models/product.model';
import { ProductApiService } from '../services/product-api.service';

interface ProductState {
  products: Product[];
  selectedProduct: Product | null;
  loading: boolean;
  error: string | null;
}

const initialState: ProductState = {
  products: [],
  selectedProduct: null,
  loading: false,
  error: null,
};

export const ProductStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  
  withComputed((store) => ({
    productsCount: computed(() => store.products().length),
    hasProducts: computed(() => store.products().length > 0),
    isLoading: computed(() => store.loading()),
  })),
  
  withMethods((store, productApi = inject(ProductApiService)) => ({
    // Load all products
    loadProducts: rxMethod<void>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap(() =>
          productApi.getAllProducts().pipe(
            tap((products) => patchState(store, { products, loading: false })),
            catchError((error) => {
              patchState(store, { error: error.message, loading: false });
              return of([]);
            })
          )
        )
      )
    ),

    // Select a product
    selectProduct: (product: Product | null) => {
      patchState(store, { selectedProduct: product });
    },

    // Create a new product
    createProduct: rxMethod<ProductCreate>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((productData) =>
          productApi.createProduct(productData).pipe(
            tap((newProduct) => {
              patchState(store, {
                products: [...store.products(), newProduct],
                loading: false,
              });
            }),
            catchError((error) => {
              patchState(store, { error: error.message, loading: false });
              return of(null);
            })
          )
        )
      )
    ),

    // Update a product
    updateProduct: rxMethod<ProductUpdate>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((productData) =>
          productApi.updateProduct(productData.id, productData).pipe(
            tap((updatedProduct) => {
              const products = store.products().map((p) =>
                p.id === updatedProduct.id ? updatedProduct : p
              );
              patchState(store, { products, loading: false });
            }),
            catchError((error) => {
              patchState(store, { error: error.message, loading: false });
              return of(null);
            })
          )
        )
      )
    ),

    // Delete a product
    deleteProduct: rxMethod<number>(
      pipe(
        tap(() => patchState(store, { loading: true, error: null })),
        switchMap((id) =>
          productApi.deleteProduct(id).pipe(
            tap(() => {
              const products = store.products().filter((p) => p.id !== id);
              patchState(store, { products, loading: false, selectedProduct: null });
            }),
            catchError((error) => {
              patchState(store, { error: error.message, loading: false });
              return of(null);
            })
          )
        )
      )
    ),

    // Clear error
    clearError: () => {
      patchState(store, { error: null });
    },
  })),

  withHooks({
    onInit(store) {
      // Load products on initialization
      store.loadProducts();
    },
  })
);

