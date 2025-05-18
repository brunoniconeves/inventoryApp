import React, { act } from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { ProductList } from '../../../ProductList';
import { ProductProvider } from '../../../../contexts/ProductContext';
import { productService } from '../../../../services/productService';

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

jest.mock('@mui/utils', () => {
  const originalModule = jest.requireActual('@mui/utils'); // Get the actual module
  return {
    __esModule: true, // Necessary for ES modules
    ...originalModule, // Spread all original exports
    unstable_getScrollbarSize: () => 0, // Override only the problematic function
                                      // You can return any number, e.g., 0 or 15.
  };
});

const mockProducts = [
  {
    id: 1,
    name: 'Test Product 1',
    description: 'Test Description 1',
    sku: 'SKU001',
    price: 99.99,
    stockQuantity: 10,
  },
  {
    id: 2,
    name: 'Test Product 2',
    description: 'Test Description 2',
    sku: 'SKU002',
    price: 149.99,
    stockQuantity: 5,
  },
];

describe('ProductList Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    (productService.getAllProducts as jest.Mock).mockResolvedValue(mockProducts);
  });

  it('renders the product list header', () => {
    render(
      <ProductProvider>
        <ProductList />
      </ProductProvider>
    );

    expect(screen.getByText('Products List - Inventory Management')).toBeInTheDocument();
    expect(screen.getByTestId('add-product-button')).toBeInTheDocument();
  });

  it('displays products in the grid', async () => {
    // Render the component with mock data
    act(() => {
      render(
        <ProductProvider>
          <ProductList />
        </ProductProvider>
      );
    });
    
    
    // Wait for the products to appear in the DOM
    expect(await screen.findByText('Test Product 1')).toBeInTheDocument();
    expect(await screen.findByText('Test Product 2')).toBeInTheDocument();
  });

  it('opens add product modal when clicking Add Product button', async () => {
    // Mock the necessary state or props
    render(
      <ProductProvider>
        <ProductList />
      </ProductProvider>
    );
  
    // Wait for initial load
    await waitFor(() => {
      expect(productService.getAllProducts).toHaveBeenCalledTimes(1);
    });
  
    // Ensure the "Add Product" button is present
    const addButton = screen.queryByTestId('add-product-button');
    expect(addButton).toBeInTheDocument();  

    addButton!.click();  
  
    // Wait for the modal to appear
    expect(await screen.findByTestId('product-edit-modal-title')).toBeInTheDocument();
  });

  it('refreshes product list when clicking refresh button', async () => {
    render(
      <ProductProvider>
        <ProductList />
      </ProductProvider>
    );

    // Wait for initial load
    await waitFor(() => {
      expect(productService.getAllProducts).toHaveBeenCalledTimes(1);
    });

    const refreshButton = screen.getByTestId('refresh-products-button');
    // Click refresh button
    refreshButton.click();    

    // Check if products were reloaded
    await waitFor(() => {
      expect(productService.getAllProducts).toHaveBeenCalledTimes(1);
    });
  });
}); 