import api from './api';

export interface Product {
  id: number;
  name: string;
  description: string;
  sku: string;
  price: number;
  stockQuantity: number;
}

export const productService = {
  getAllProducts: async (): Promise<Product[]> => {
    const response = await api.get('/products');
    return response.data;
  },

  getProduct: async (id: number): Promise<Product> => {
    const response = await api.get(`/products/${id}`);
    return response.data;
  },

  createProduct: async (product: Omit<Product, 'id'>): Promise<Product> => {
    const response = await api.post('/products', product);
    return response.data;
  },

  updateProduct: async (id: number, product: Partial<Product>): Promise<Product> => {
    const response = await api.put(`/products/${id}`, product);
    return response.data;
  },

  deleteProduct: async (id: number): Promise<void> => {
    await api.delete(`/products/${id}`);
  },

  addStock: async (productId: number, quantity: number): Promise<void> => {
    await api.post(`/inventory/products/${productId}/stock`, { quantity });
  },

  removeStock: async (productId: number, quantity: number): Promise<void> => {
    await api.delete(`/inventory/products/${productId}/stock`, { 
      data: { quantity } 
    });
  }
}; 