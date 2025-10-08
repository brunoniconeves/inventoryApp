export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  sku: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface ProductCreate {
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  sku: string;
}

export interface ProductUpdate extends ProductCreate {
  id: number;
}

export interface Inventory {
  productId: number;
  productName: string;
  sku: string;
  currentStock: number;
}

export interface StockUpdate {
  quantity: number;
}

