import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { ProductForm } from '../../../ProductForm';
import { ProductProvider } from '../../../../contexts/ProductContext';
import { productService } from '../../../../services/productService';
import { useNavigate } from 'react-router-dom';

// Mock react-router-dom
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useNavigate: jest.fn()
}));

// Mock the productService
jest.mock('../../../../services/productService', () => ({
  productService: {
    getAllProducts: jest.fn(),
    createProduct: jest.fn(),
    updateProduct: jest.fn(),
    deleteProduct: jest.fn(),
    addStock: jest.fn(),
    removeStock: jest.fn(),
  },
}));

const mockProduct = {
  id: 1,
  name: 'Test Product',
  description: 'Test Description',
  sku: 'SKU001',
  price: 99.99,
  stockQuantity: 10,
};

const mockNavigate = jest.fn();

describe('ProductForm Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    (useNavigate as jest.Mock).mockReturnValue(mockNavigate);
  });

  it('renders create product form correctly', () => {
    render(
      <ProductProvider>
        <ProductForm isNewProduct={true} />
      </ProductProvider>
    );

    expect(screen.getByTestId('name-input')).toBeInTheDocument();
    expect(screen.getByTestId('sku-input')).toBeInTheDocument();
    expect(screen.getByTestId('description-input')).toBeInTheDocument();
    expect(screen.getByTestId('price-input')).toBeInTheDocument();
    expect(screen.getByTestId('initial-stock-input')).toBeInTheDocument();
    expect(screen.getByTestId('save-btn')).toHaveTextContent('Create Product');
  });

  it('renders edit product form correctly', () => {
    render(
      <ProductProvider>
        <ProductForm product={mockProduct} />
      </ProductProvider>
    );

    expect(screen.getByTestId('name-input-field')).toHaveValue(mockProduct.name);
    expect(screen.getByTestId('sku-input-field')).toHaveValue(mockProduct.sku);
    expect(screen.getByTestId('description-input-field')).toHaveValue(mockProduct.description);
    expect(screen.getByTestId('price-input-field')).toHaveValue(mockProduct.price);
    expect(screen.getByTestId('stock-quantity-input-field')).toBeInTheDocument();
    expect(screen.getByTestId('save-btn')).toHaveTextContent('Save Changes');
  });

  it('validates required fields before submission', async () => {
    render(
      <ProductProvider>
        <ProductForm isNewProduct={true} />
      </ProductProvider>
    );

    const saveButton = screen.getByTestId('save-btn');
    fireEvent.click(saveButton);

    await waitFor(() => {
      expect(screen.getByText('Product name is required')).toBeInTheDocument();
      expect(screen.getByText('Product description is required')).toBeInTheDocument();
      expect(screen.getByText('Product SKU is required')).toBeInTheDocument();
      expect(screen.getByText('Product price must be greater than zero')).toBeInTheDocument();
    });
  });

  it('handles stock management correctly', async () => {
    render(
      <ProductProvider>
        <ProductForm product={mockProduct} />
      </ProductProvider>
    );

    // Test adding stock
    const addStockButton = screen.getByTestId('add-stock-button');
    const quantityInput = screen.getByTestId('stock-quantity-input-field');

    fireEvent.change(quantityInput, { target: { value: '5' } });
    fireEvent.click(addStockButton);

    await waitFor(() => {
      expect(productService.addStock).toHaveBeenCalledWith(mockProduct.id, 5);
    });

    // Test removing stock
    const removeStockButton = screen.getByTestId('remove-stock-button');
    fireEvent.click(removeStockButton);

    await waitFor(() => {
      expect(productService.removeStock).toHaveBeenCalledWith(mockProduct.id, 5);
    });
  });

  it('disables remove stock button when quantity exceeds current stock', () => {
    render(
      <ProductProvider>
        <ProductForm product={mockProduct} />
      </ProductProvider>
    );

    const quantityInput = screen.getByTestId('stock-quantity-input-field');
    const removeStockButton = screen.getByTestId('remove-stock-button');

    // Try to remove more than available
    fireEvent.change(quantityInput, { target: { value: String(mockProduct.stockQuantity + 1) } });
    
    expect(removeStockButton).toBeDisabled();

    // Try to remove exactly available amount
    fireEvent.change(quantityInput, { target: { value: String(mockProduct.stockQuantity) } });
    
    expect(removeStockButton).not.toBeDisabled();
  });

  it('navigates back to list on cancel', () => {
    render(
      <ProductProvider>
        <ProductForm isNewProduct={true} />
      </ProductProvider>
    );

    const cancelButton = screen.getByTestId('cancel-btn');
    fireEvent.click(cancelButton);

    expect(mockNavigate).toHaveBeenCalledWith('/');
  });

  it('handles successful product creation', async () => {
    const newProduct = {
      name: 'New Product',
      description: 'New Description',
      sku: 'NEW001',
      price: 49.99,
      initialStock: 5
    };

    (productService.createProduct as jest.Mock).mockResolvedValue({
      ...newProduct,
      id: 2
    });

    render(
      <ProductProvider>
        <ProductForm isNewProduct={true} />
      </ProductProvider>
    );

    // Fill in the form
    fireEvent.change(screen.getByTestId('name-input-field'), { target: { value: newProduct.name } });
    fireEvent.change(screen.getByTestId('description-input-field'), { target: { value: newProduct.description } });
    fireEvent.change(screen.getByTestId('sku-input-field'), { target: { value: newProduct.sku } });
    fireEvent.change(screen.getByTestId('price-input-field'), { target: { value: newProduct.price.toString() } });
    fireEvent.change(screen.getByTestId('initial-stock-input-field'), { target: { value: newProduct.initialStock.toString() } });

    // Submit the form
    fireEvent.click(screen.getByTestId('save-btn'));

    await waitFor(() => {
      expect(productService.createProduct).toHaveBeenCalledWith({
        name: newProduct.name,
        description: newProduct.description,
        sku: newProduct.sku,
        price: newProduct.price,
        stockQuantity: newProduct.initialStock
      });
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
  });

  it('handles successful product update', async () => {
    const updatedProduct = {
      ...mockProduct,
      name: 'Updated Name',
      price: 149.99
    };

    (productService.updateProduct as jest.Mock).mockResolvedValue(updatedProduct);

    render(
      <ProductProvider>
        <ProductForm product={mockProduct} />
      </ProductProvider>
    );

    // Update form fields
    fireEvent.change(screen.getByTestId('name-input-field'), { target: { value: updatedProduct.name } });
    fireEvent.change(screen.getByTestId('price-input-field'), { target: { value: updatedProduct.price.toString() } });

    // Submit the form
    fireEvent.click(screen.getByTestId('save-btn'));

    await waitFor(() => {
      expect(productService.updateProduct).toHaveBeenCalledWith(mockProduct.id, {
        name: updatedProduct.name,
        price: updatedProduct.price
      });
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });
  });

  it('handles successful product deletion when confirmed', async () => {
    // Mock window.confirm to return true
    const confirmSpy = jest.spyOn(window, 'confirm').mockImplementation(() => true);

    render(
      <ProductProvider>
        <ProductForm product={mockProduct} />
      </ProductProvider>
    );

    const deleteButton = screen.getByTestId('delete-btn');
    fireEvent.click(deleteButton);

    // Verify confirm was called with correct message
    expect(confirmSpy).toHaveBeenCalledWith(
      'Are you sure you want to delete the product "Test Product"? This action cannot be undone.'
    );

    await waitFor(() => {
      expect(productService.deleteProduct).toHaveBeenCalledWith(mockProduct.id);
      expect(mockNavigate).toHaveBeenCalledWith('/');
    });

    confirmSpy.mockRestore();
  });

  it('does not delete product when canceling confirmation', async () => {
    // Mock window.confirm to return false
    const confirmSpy = jest.spyOn(window, 'confirm').mockImplementation(() => false);

    render(
      <ProductProvider>
        <ProductForm product={mockProduct} />
      </ProductProvider>
    );

    const deleteButton = screen.getByTestId('delete-btn');
    fireEvent.click(deleteButton);

    // Verify confirm was called
    expect(confirmSpy).toHaveBeenCalled();

    // Verify delete was not called
    expect(productService.deleteProduct).not.toHaveBeenCalled();
    expect(mockNavigate).not.toHaveBeenCalled();

    confirmSpy.mockRestore();
  });
}); 