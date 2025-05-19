import React, { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  DialogContentText,
  Button,
  TextField,
  Typography,
  Box,
  IconButton,
  Alert,
  AlertProps,
  Divider,
  useMediaQuery,
  useTheme,
} from '@mui/material';
import { Add as AddIcon, Remove as RemoveIcon } from '@mui/icons-material';
import { Product } from '../services/productService';
import { useProducts } from '../contexts/ProductContext';
import { DeleteConfirmationDialog } from './DeleteConfirmationDialog';
import axios, { AxiosError } from 'axios';

interface ProductEditModalProps {
  open: boolean;
  onClose: () => void;
  product: Product;
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

export const ProductEditModal: React.FC<ProductEditModalProps> = ({
  open,
  onClose,
  product: initialProduct,
  isNewProduct = false
}) => {
  const theme = useTheme();
  const fullScreen = useMediaQuery(theme.breakpoints.down('md'));
  const [isModalMounted, setIsModalMounted] = useState(false);
  const { updateProduct, deleteProduct, addStock, removeStock, createProduct } = useProducts();
  const [formData, setFormData] = useState<Partial<Product> & { initialStock?: number }>({
    name: initialProduct.name,
    description: initialProduct.description,
    sku: initialProduct.sku,
    price: initialProduct.price,
    initialStock: isNewProduct ? 0 : undefined,
  });
  const [stockQuantity, setStockQuantity] = useState<number>(1);
  const [error, setError] = useState<string | null>(null);
  const [currentStock, setCurrentStock] = useState<number>(initialProduct.stockQuantity);
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);

  useEffect(() => {
    if (open) {
      // Delay modal mounting to ensure document is ready
      const timer = setTimeout(() => {
        setIsModalMounted(true);
      }, 0);
      return () => clearTimeout(timer);
    } else {
      setIsModalMounted(false);
    }
  }, [open]);

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
      [name]: name === 'price' ? parseFloat(value) : value
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
        const createdProduct = await createProduct({
          ...productData,
          stockQuantity: initialStock || 0,
        } as Omit<Product, 'id'>);
      } else {
        await updateProduct(initialProduct.id, formData);
      }
      setError(null);
      onClose();
    } catch (err) {
      const backendError = getErrorMessage(err);
      setError(`Failed to ${isNewProduct ? 'create' : 'update'} product\n${backendError}`);
    }
  };

  const handleDeleteClick = () => {
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    try {
      await deleteProduct(initialProduct.id);
      setDeleteDialogOpen(false);
      onClose();
    } catch (err) {
      const backendError = getErrorMessage(err);
      setError(`Failed to delete product\n${backendError}`);
      setDeleteDialogOpen(false);
    }
  };

  const handleStockChange = async (action: 'add' | 'remove') => {
    try {
      if (action === 'add') {
        await addStock(initialProduct.id, stockQuantity);
        setCurrentStock(prev => prev + stockQuantity);
      } else {
        if (currentStock < stockQuantity) {
          setError('Cannot remove more stock than available');
          return;
        }
        await removeStock(initialProduct.id, stockQuantity);
        setCurrentStock(prev => prev - stockQuantity);
      }
      setError(null);
    } catch (err) {
      const backendError = getErrorMessage(err);
      setError(`Failed to ${action} stock\n${backendError}`);
    }
  };

  return (
    <>
      {isModalMounted && (
        <Dialog 
          open={open} 
          onClose={onClose} 
          maxWidth="sm" 
          fullWidth
          fullScreen={fullScreen}
          disablePortal={false}
          keepMounted={false}
          disableScrollLock={false}
          aria-labelledby="product-edit-dialog-title"
        >
          <DialogTitle id="product-edit-dialog-title" data-testid="product-edit-modal-title">
            {isNewProduct ? 'Create New Product' : 'Edit Product'}
          </DialogTitle>
          <DialogContent>
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
            
            <Box sx={{ mt: 2, display: 'flex', flexDirection: 'column', gap: 2 }}>
              <Box>
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
              </Box>
              <Box>
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
              </Box>
              <Box>
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
              </Box>
              <Box>
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
              </Box>

              {isNewProduct ? (
                <Box>
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
                </Box>
              ) : (
                <Box>
                  <Divider sx={{ my: 2 }} />
                  <Typography variant="h6" gutterBottom>
                    Stock Management
                  </Typography>
                  <Box sx={{ mb: 4 }}>Use "+" or "-" to add or remove the defined quantity of this product stock.</Box>
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
            </Box>
          </DialogContent>
          <DialogActions>
            {!isNewProduct && (
              <Button onClick={handleDeleteClick} color="error" data-testid="delete-btn">
                Delete Product
              </Button>
            )}
            <Button onClick={onClose} data-testid="edit-modal-cancel-btn">Cancel</Button>
            <Button onClick={handleSave} color="primary" variant="contained" data-testid="save-btn">
              {isNewProduct ? 'Create Product' : 'Save Changes'}
            </Button>
          </DialogActions>
        </Dialog>
      )}

      {deleteDialogOpen && (
        <DeleteConfirmationDialog
          open={deleteDialogOpen}
          onClose={() => setDeleteDialogOpen(false)}
          onConfirm={handleDeleteConfirm}
          productName={initialProduct.name}
        />
      )}
    </>
  );
}; 

