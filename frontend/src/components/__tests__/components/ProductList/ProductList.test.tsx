import React from 'react';
import { render, screen, fireEvent, waitFor, within, prettyDOM } from '@testing-library/react';
import { ProductList } from '../../../ProductList';
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

const mockNavigate = jest.fn();

describe('ProductList Component', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    (productService.getAllProducts as jest.Mock).mockResolvedValue(mockProducts);
    (useNavigate as jest.Mock).mockReturnValue(mockNavigate);
  });

  it('renders the product list header', async () => {
    render(
      <ProductProvider>
        <ProductList />
      </ProductProvider>
    );

    await waitFor(() => {
      expect(screen.getByText('Products List - Inventory Management')).toBeInTheDocument();
      expect(screen.getByTestId('add-product-button')).toBeInTheDocument();
      expect(screen.getByTestId('refresh-products-button')).toBeInTheDocument();
    });
  });

  it('displays products in the grid', async () => {
    render(
      <ProductProvider>
        <ProductList />
      </ProductProvider>
    );
    
    await waitFor(() => {
      expect(screen.getByText('Test Product 1')).toBeInTheDocument();
      expect(screen.getByText('Test Product 2')).toBeInTheDocument();
      expect(screen.getByText('$99.99')).toBeInTheDocument();
      expect(screen.getByText('$149.99')).toBeInTheDocument();
    });
  });

  it('navigates to create product page when clicking Add Product button', async () => {
    render(
      <ProductProvider>
        <ProductList />
      </ProductProvider>
    );
  
    await waitFor(() => {
      expect(productService.getAllProducts).toHaveBeenCalledTimes(1);
    });
  
    const addButton = screen.getByTestId('add-product-button');
    expect(addButton).toBeInTheDocument();  

    fireEvent.click(addButton);  
  
    expect(mockNavigate).toHaveBeenCalledWith('/products/new');
  });

  it('refreshes product list when clicking refresh button', async () => {
    render(
      <ProductProvider>
        <ProductList />
      </ProductProvider>
    );

    await waitFor(() => {
      expect(productService.getAllProducts).toHaveBeenCalledTimes(1);
    });

    const refreshButton = screen.getByTestId('refresh-products-button');
    fireEvent.click(refreshButton);    

    await waitFor(() => {
      expect(productService.getAllProducts).toHaveBeenCalledTimes(2);
    });
  });

  it('navigates to edit page when clicking edit button', async () => {
    render(
      <ProductProvider>
        <ProductList />
      </ProductProvider>
    );

    await waitFor(() => {
      expect(screen.getByText('Test Product 1')).toBeInTheDocument();
    });

    const editButton = screen.getAllByTestId('edit-product-button')[0];
    fireEvent.click(editButton);

    expect(mockNavigate).toHaveBeenCalledWith('/products/1/edit');
  });

  it('opens delete confirmation when clicking delete button', async () => {
    const { container } = render(
      <ProductProvider>
        <ProductList />
      </ProductProvider>
    );

    // Wait for data to load
    await waitFor(() => {
      expect(screen.getByText('Test Product 1')).toBeInTheDocument();
    });

    // Wait for the grid to be ready
    await waitFor(() => {
      const cells = container.querySelectorAll('.MuiDataGrid-cell');
      expect(cells.length).toBeGreaterThan(0);
    });

    // Find the actions cell in the first row
    const actionsCells = container.querySelectorAll('.MuiDataGrid-cell[data-field="actions"]');
    expect(actionsCells.length).toBeGreaterThan(0);
    
    // Click the delete button in the first row's actions cell
    const deleteButton = within(actionsCells[0] as HTMLElement).getByRole('button', {
      name: /delete product/i,
    });
    fireEvent.click(deleteButton);

    expect(screen.getByText(/Are you sure you want to delete the product/)).toBeInTheDocument();
  });

  it('deletes product when confirming deletion', async () => {
    const { container } = render(
      <ProductProvider>
        <ProductList />
      </ProductProvider>
    );

    // Wait for data to load
    await waitFor(() => {
      expect(screen.getByText('Test Product 1')).toBeInTheDocument();
    });

    // Wait for the grid to be ready
    await waitFor(() => {
      const cells = container.querySelectorAll('.MuiDataGrid-cell');
      expect(cells.length).toBeGreaterThan(0);
    });

    // Find the actions cell in the first row
    const actionsCells = container.querySelectorAll('.MuiDataGrid-cell[data-field="actions"]');
    const deleteButton = within(actionsCells[0] as HTMLElement).getByRole('button', {
      name: /delete product/i,
    });
    fireEvent.click(deleteButton);

    const confirmButton = screen.getByTestId('confirm-delete-btn');
    fireEvent.click(confirmButton);

    await waitFor(() => {
      expect(productService.deleteProduct).toHaveBeenCalledWith(1);
      expect(productService.getAllProducts).toHaveBeenCalledTimes(2);
    });
  });
}); 