import React, { createContext, useContext, useState, useCallback, ReactNode } from 'react';
import { Product, productService } from '../services/productService';

interface ProductContextData {
  products: Product[];
  loading: boolean;
  error: string | null;
  initialized: boolean;
  initializeProducts: () => Promise<void>;
  refreshProducts: () => Promise<void>;
  updateProduct: (id: number, product: Partial<Product>) => Promise<void>;
  deleteProduct: (id: number) => Promise<void>;
  addStock: (productId: number, quantity: number) => Promise<void>;
  removeStock: (productId: number, quantity: number) => Promise<void>;
  createProduct: (product: Omit<Product, 'id'>) => Promise<void>;
}

const ProductContext = createContext<ProductContextData>({} as ProductContextData);

export const useProducts = () => {
  const context = useContext(ProductContext);
  if (!context) {
    throw new Error('useProducts must be used within a ProductProvider');
  }
  return context;
};

export const ProductProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [initialized, setInitialized] = useState(false);

  const fetchProducts = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await productService.getAllProducts();
      setProducts(data);
      return data;
    } catch (err) {
      setError('Failed to load products');
      console.error('Error loading products:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const initializeProducts = useCallback(async () => {
    if (initialized) return;
    
    try {
      await fetchProducts();
      setInitialized(true);
    } catch (err) {
      // Error already handled in fetchProducts
    }
  }, [initialized, fetchProducts]);

  const refreshProducts = useCallback(async () => {
    try {
      await fetchProducts();
    } catch (err) {
      // Error already handled in fetchProducts
    }
  }, [fetchProducts]);

  const updateProduct = useCallback(async (id: number, productData: Partial<Product>) => {
    try {
      setLoading(true);
      setError(null);
      const updatedProduct = await productService.updateProduct(id, productData);
      setProducts(prevProducts =>
        prevProducts.map(p => (p.id === id ? updatedProduct : p))
      );
    } catch (err) {
      setError('Failed to update product');
      console.error(err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const createProduct = useCallback(async (product: Omit<Product, 'id'>) => {
    try {
      const newProduct = await productService.createProduct(product);
      setProducts(prevProducts => [...prevProducts, newProduct]);
      setError(null);
    } catch (err) {
      setError('Failed to create product');
      console.error(err);
      throw err;
    }
  }, []);

  const deleteProduct = useCallback(async (id: number) => {
    try {
      setLoading(true);
      setError(null);
      await productService.deleteProduct(id);
      setProducts(prevProducts => prevProducts.filter(p => p.id !== id));
    } catch (err) {
      setError('Failed to delete product');
      console.error(err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const addStock = useCallback(async (productId: number, quantity: number) => {
    try {
      setLoading(true);
      setError(null);
      await productService.addStock(productId, quantity);
      setProducts(prevProducts =>
        prevProducts.map(p =>
          p.id === productId
            ? { ...p, stockQuantity: p.stockQuantity + quantity }
            : p
        )
      );
    } catch (err) {
      setError('Failed to add stock');
      console.error(err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const removeStock = useCallback(async (productId: number, quantity: number) => {
    try {
      setLoading(true);
      setError(null);
      await productService.removeStock(productId, quantity);
      setProducts(prevProducts =>
        prevProducts.map(p =>
          p.id === productId
            ? { ...p, stockQuantity: p.stockQuantity - quantity }
            : p
        )
      );
    } catch (err) {
      setError('Failed to remove stock');
      console.error(err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return (
    <ProductContext.Provider
      value={{
        products,
        loading,
        error,
        initialized,
        initializeProducts,
        refreshProducts,
        updateProduct,
        deleteProduct,
        addStock,
        removeStock,
        createProduct
      }}
    >
      {children}
    </ProductContext.Provider>
  );
}; 