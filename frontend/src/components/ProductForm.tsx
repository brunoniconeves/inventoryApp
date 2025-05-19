import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Button,
  TextField,
  Typography,
  Box,
  IconButton,
  Alert,
  AlertProps,
  Divider,
  Paper,
  Container
} from '@mui/material';
import { Add as AddIcon, Remove as RemoveIcon } from '@mui/icons-material';
import { Product } from '../services/productService';
import { useProducts } from '../contexts/ProductContext';
import axios from 'axios';
import { DeleteConfirmationDialog } from './DeleteConfirmationDialog';

interface ProductFormProps {
  product?: Product;
  isNewProduct?: boolean;
}

interface ValidationErrors {
  name?: string;
  description?: string;
  sku?: string;
  price?: string;
  initialStock?: string;
}

const ErrorAlert: React.FC<AlertProps & { message: string }> = ({ message, ...props }) => (
  <Alert {...props}>{message}</Alert>
);

export const ProductForm: React.FC<ProductFormProps> = ({
  product,
  isNewProduct = false
}) => {
  const navigate = useNavigate();
  const { updateProduct, deleteProduct, addStock, removeStock, createProduct } = useProducts();
  const [formData, setFormData] = useState<Partial<Product> & { initialStock?: number }>({
    name: product?.name || '',
    description: product?.description || '',
    sku: product?.sku || '',
    price: product?.price || 0,
    initialStock: isNewProduct ? 0 : undefined,
  });
  const [stockQuantity, setStockQuantity] = useState<number>(1);
  const [error, setError] = useState<string | null>(null);
  const [currentStock, setCurrentStock] = useState<number>(product?.stockQuantity || 0);
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

  const validateForm = (): boolean => {
    const errors: ValidationErrors = {};
    
    if (!formData.name?.trim()) {
      errors.name = 'Product name is required';
    }
    
    if (!formData.description?.trim()) {
      errors.description = 'Product description is required';
    }
    
    if (!formData.sku?.trim()) {
      errors.sku = 'Product SKU is required';
    }
    
    if (!formData.price || formData.price <= 0) {
      errors.price = 'Product price must be greater than zero';
    }

    if (isNewProduct && (formData.initialStock === undefined || formData.initialStock < 0)) {
      errors.initialStock = 'Initial stock quantity must be zero or greater';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'price' || name === 'initialStock' ? parseFloat(value) : value
    }));
    // Clear validation error for the field being edited
    setValidationErrors(prev => ({
      ...prev,
      [name]: undefined
    }));
  };

  const handleStockQuantityChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(e.target.value) || 1;
    setStockQuantity(Math.max(1, value));
  };

  const getErrorMessage = (err: unknown): string => {
    if (axios.isAxiosError(err)) {
      const errorMessage = err.response?.data || err.message;
      // Check if the error message matches any of our validation fields
      if (typeof errorMessage === 'string') {
        if (errorMessage.includes('name')) {
          setValidationErrors(prev => ({ ...prev, name: errorMessage }));
        } else if (errorMessage.includes('SKU')) {
          setValidationErrors(prev => ({ ...prev, sku: errorMessage }));
        } else if (errorMessage.includes('price')) {
          setValidationErrors(prev => ({ ...prev, price: errorMessage }));
        } else if (errorMessage.includes('description')) {
          setValidationErrors(prev => ({ ...prev, description: errorMessage }));
        }
      }
      return errorMessage;
    }
    return err instanceof Error ? err.message : 'An unknown error occurred';
  };

  const handleSave = async () => {
    try {
      if (!validateForm()) {
        return;
      }

      if (isNewProduct) {
        const { initialStock, ...productData } = formData;
        await createProduct({
          ...productData,
          stockQuantity: initialStock || 0,
        } as Omit<Product, 'id'>);
      } else if (product) {
        await updateProduct(product.id, formData);
      }
      setError(null);
      navigate('/');
    } catch (err) {
      const backendError = getErrorMessage(err);
      setError(`Failed to ${isNewProduct ? 'create' : 'update'} product\n${backendError}`);
    }
  };

  const handleDelete = async () => {
    if (!product) return;
    
    try {
      await deleteProduct(product.id);
      setDeleteDialogOpen(false);
      navigate('/');
    } catch (err) {
      const backendError = getErrorMessage(err);
      setError(`Failed to delete product\n${backendError}`);
      setDeleteDialogOpen(false);
    }
  };

  const handleDeleteClick = () => {
    setError(null); // Clear any previous errors
    setDeleteDialogOpen(true);
  };

  const handleDeleteCancel = () => {
    setError(null); // Clear any previous errors
    setDeleteDialogOpen(false);
  };

  const handleStockChange = async (action: 'add' | 'remove') => {
    if (!product) return;

    try {
      if (action === 'add') {
        await addStock(product.id, stockQuantity);
        setCurrentStock(prev => prev + stockQuantity);
      } else {
        if (currentStock < stockQuantity) {
          setError('Cannot remove more stock than available');
          return;
        }
        await removeStock(product.id, stockQuantity);
        setCurrentStock(prev => prev - stockQuantity);
      }
      setError(null);
    } catch (err) {
      const backendError = getErrorMessage(err);
      setError(`Failed to ${action} stock\n${backendError}`);
    }
  };

  return (
    <Container maxWidth="md">
      <DeleteConfirmationDialog
        open={deleteDialogOpen}
        productName={product?.name || ''}
        onConfirm={handleDelete}
        onCancel={handleDeleteCancel}
      />
      <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
        <Box sx={{ mb: 4 }}>
          <Typography variant="h4" component="h1" gutterBottom>
            {isNewProduct ? 'Create New Product' : 'Edit Product'}
          </Typography>
        </Box>

        {error && (
          <Box sx={{ mb: 2 }}>
            <ErrorAlert 
              message={error} 
              severity="error" 
              sx={{ 
                whiteSpace: 'pre-line',
                '& .MuiAlert-message': {
                  whiteSpace: 'pre-line'
                }
              }} 
            />
          </Box>
        )}
        
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <TextField
            data-testid="name-input"
            fullWidth
            label="Name"
            name="name"
            value={formData.name}
            onChange={handleInputChange}
            error={!!validationErrors.name}
            helperText={validationErrors.name}
          />

          <TextField
            data-testid="sku-input"
            fullWidth
            label="SKU"
            name="sku"
            value={formData.sku}
            onChange={handleInputChange}
            error={!!validationErrors.sku}
            helperText={validationErrors.sku}
          />

          <TextField
            data-testid="description-input"
            fullWidth
            label="Description"
            name="description"
            value={formData.description}
            onChange={handleInputChange}
            multiline
            rows={3}
            error={!!validationErrors.description}
            helperText={validationErrors.description}
          />

          <TextField
            data-testid="price-input"
            fullWidth
            label="Price"
            name="price"
            type="number"
            value={formData.price}
            onChange={handleInputChange}
            error={!!validationErrors.price}
            helperText={validationErrors.price}
            InputProps={{
              startAdornment: <Typography>$</Typography>,
              inputProps: {
                min: 0.01,
                step: 0.01
              }
            }}
          />

          {isNewProduct ? (
            <TextField
              data-testid="initial-stock-input"
              fullWidth
              label="Initial Stock Quantity"
              name="initialStock"
              type="number"
              value={formData.initialStock}
              onChange={handleInputChange}
              error={!!validationErrors.initialStock}
              helperText={validationErrors.initialStock}
              InputProps={{
                inputProps: {
                  min: 0,
                  step: 1
                }
              }}
            />
          ) : (
            <Box>
              <Divider sx={{ my: 2 }} />
              <Typography variant="h6" gutterBottom>
                Stock Management
              </Typography>
              <Box sx={{ mb: 2 }}>
                Use "+" or "-" to add or remove the defined quantity of this product stock.
              </Box>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                <IconButton
                  color="primary"
                  onClick={() => handleStockChange('add')}
                >
                  <AddIcon />
                </IconButton>
                <TextField
                  data-testid="stock-quantity-input"
                  label="Quantity"
                  type="number"
                  value={stockQuantity}
                  onChange={handleStockQuantityChange}
                  InputProps={{
                    inputProps: {
                      min: 1,
                      step: 1
                    }
                  }}
                  size="small"
                  sx={{ width: 100 }}
                />
                <IconButton
                  color="secondary"
                  onClick={() => handleStockChange('remove')}
                  disabled={currentStock < stockQuantity}
                >
                  <RemoveIcon />
                </IconButton>
                <Typography>
                  Current Stock: {currentStock}
                </Typography>
              </Box>
            </Box>
          )}

          <Box sx={{ mt: 4, display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
            {!isNewProduct && (
              <Button 
                onClick={handleDeleteClick}
                color="error" 
                variant="contained"
                data-testid="delete-btn"
              >
                Delete Product
              </Button>
            )}
            <Button 
              onClick={() => navigate('/')} 
              data-testid="cancel-btn"
            >
              Cancel
            </Button>
            <Button 
              onClick={handleSave} 
              color="primary" 
              variant="contained" 
              data-testid="save-btn"
            >
              {isNewProduct ? 'Create Product' : 'Save Changes'}
            </Button>
          </Box>
        </Box>
      </Paper>
    </Container>
  );
}; 