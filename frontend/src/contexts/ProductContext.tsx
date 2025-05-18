import React, { createContext, useContext, useState, useCallback, ReactNode } from 'react';
import { Product, productService } from '../services/productService';

interface ProductContextData {
  products: Product[];
  loading: boolean;
  error: string | null;
  loadProducts: () => Promise<void>;
  updateProduct: (id: number, product: Partial<Product>) => Promise<void>;
  deleteProduct: (id: number) => Promise<void>;
  addStock: (productId: number, quantity: number) => Promise<void>;
  removeStock: (productId: number, quantity: number) => Promise<void>;
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

  const loadProducts = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await productService.getAllProducts();
      setProducts(data);
    } catch (err) {
      setError('Failed to load products');
      console.error(err);
    } finally {
      setLoading(false);
    }
  }, []);

  const updateProduct = useCallback(async (id: number, productData: Partial<Product>) => {
    try {
      setLoading(true);
      setError(null);
      await productService.updateProduct(id, productData);
      await loadProducts(); // Reload the products list
    } catch (err) {
      setError('Failed to update product');
      console.error(err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadProducts]);

  const deleteProduct = useCallback(async (id: number) => {
    try {
      setLoading(true);
      setError(null);
      await productService.deleteProduct(id);
      await loadProducts(); // Reload the products list
    } catch (err) {
      setError('Failed to delete product');
      console.error(err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadProducts]);

  const addStock = useCallback(async (productId: number, quantity: number) => {
    try {
      setLoading(true);
      setError(null);
      await productService.addStock(productId, quantity);
      await loadProducts(); // Reload to get updated stock
    } catch (err) {
      setError('Failed to add stock');
      console.error(err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadProducts]);

  const removeStock = useCallback(async (productId: number, quantity: number) => {
    try {
      setLoading(true);
      setError(null);
      await productService.removeStock(productId, quantity);
      await loadProducts(); // Reload to get updated stock
    } catch (err) {
      setError('Failed to remove stock');
      console.error(err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadProducts]);

  return (
    <ProductContext.Provider
      value={{
        products,
        loading,
        error,
        loadProducts,
        updateProduct,
        deleteProduct,
        addStock,
        removeStock,
      }}
    >
      {children}
    </ProductContext.Provider>
  );
}; 