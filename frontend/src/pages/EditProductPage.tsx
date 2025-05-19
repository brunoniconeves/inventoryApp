import React from 'react';
import { useParams, Navigate } from 'react-router-dom';
import { ProductForm } from '../components/ProductForm';
import { useProducts } from '../contexts/ProductContext';

export const EditProductPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { products } = useProducts();
  
  const product = products.find(p => p.id === Number(id));
  
  if (!product) {
    return <Navigate to="/" />;
  }

  return <ProductForm product={product} />;
}; 