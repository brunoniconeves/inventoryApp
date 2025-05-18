export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  stockQuantity: number;
  sku?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductDto {
  name: string;
  description?: string;
  price: number;
  stockQuantity: number;
  sku?: string;
}

export interface UpdateProductDto {
  name: string;
  description?: string;
  price: number;
  sku?: string;
}

export interface UpdateStockDto {
  quantity: number;
} 