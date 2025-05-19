import React, { useRef, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { 
  DataGrid, 
  GridColDef, 
  GridRenderCellParams,
  GridSortModel
} from '@mui/x-data-grid';
import { 
  Box, 
  IconButton, 
  Tooltip, 
  Stack,
  Typography,
  Button,
  Paper
} from '@mui/material';
import { 
  Edit as EditIcon, 
  Delete as DeleteIcon,
  Add as AddIcon,
  Refresh as RefreshIcon
} from '@mui/icons-material';
import { useProducts } from '../contexts/ProductContext';
import { Product } from '../services/productService';
import { DeleteConfirmationDialog } from './DeleteConfirmationDialog';

export const ProductList: React.FC = () => {
  const navigate = useNavigate();
  const { 
    products, 
    loading, 
    error, 
    initialized,
    initializeProducts,
    refreshProducts,
    deleteProduct
  } = useProducts();

  const initializationRef = useRef(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [productToDelete, setProductToDelete] = useState<Product | null>(null);

  const [sortModel, setSortModel] = React.useState<GridSortModel>([
    {
      field: 'name',
      sort: 'asc',
    },
  ]);

  useEffect(() => {
    if (!initialized && !loading) {
      initializeProducts();
    }
  }, [initialized, loading, initializeProducts]);

  const handleRefresh = async () => {
    await refreshProducts();
  };

  const handleEditClick = (product: Product) => {
    navigate(`/products/${product.id}/edit`);
  };

  const handleDeleteClick = (product: Product) => {
    setProductToDelete(product);
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (productToDelete) {
      try {
        await deleteProduct(productToDelete.id);
        await refreshProducts();
      } catch (error) {
        console.error('Failed to delete product:', error);
      } finally {
        setDeleteDialogOpen(false);
        setProductToDelete(null);
      }
    }
  };

  const handleDeleteCancel = () => {
    setDeleteDialogOpen(false);
    setProductToDelete(null);
  };

  const handleAddClick = () => {
    navigate('/products/new');
  };

  const columns: GridColDef[] = [
    { field: 'name', headerName: 'Name', width: 200 },
    { field: 'sku', headerName: 'SKU', width: 130 },
    { field: 'description', headerName: 'Description', width: 300 },
    {
      field: 'price',
      headerName: 'Price',
      width: 130,
      valueFormatter: ({ value }) => {
        if (value == null) return '';
        return `$${(value as number).toFixed(2)}`;
      },
    },
    { field: 'stockQuantity', headerName: 'Current Stock', width: 130 },
    {
      field: 'actions',
      headerName: 'Actions',
      width: 120,
      sortable: false,
      renderCell: (params: GridRenderCellParams<Product>) => (
        <Stack direction="row" spacing={1}>
          <Tooltip title="Edit Product" arrow>
            <IconButton
              onClick={() => handleEditClick(params.row)}
              size="small"
              sx={{
                backgroundColor: '#CCCCCC',
                color: '#E7E7E7',
                '&:hover': {
                  backgroundColor: '#333333',
                },
                width: 32,
                height: 32,
              }}
              aria-label="Edit Product"
              data-testid="edit-product-button"
            >
              <EditIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Delete Product" arrow>
            <IconButton
              onClick={() => handleDeleteClick(params.row)}
              size="small"
              sx={{
                backgroundColor: '#d32f2f',
                color: '#E7E7E7',
                '&:hover': {
                  backgroundColor: '#b71c1c',
                },
                width: 32,
                height: 32,
              }}
              aria-label="Delete Product"
              data-testid="delete-product-button"
            >
              <DeleteIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Stack>
      ),
    },
  ];

  return (
    <Box sx={{ height: 600, width: '100%' }}>
      <DeleteConfirmationDialog
        open={deleteDialogOpen}
        productName={productToDelete?.name || ''}
        onConfirm={handleDeleteConfirm}
        onCancel={handleDeleteCancel}
      />
      <Paper 
        elevation={0} 
        sx={{ 
          p: 2, 
          mb: 2, 
          display: 'flex', 
          justifyContent: 'space-between', 
          alignItems: 'center',
          bgcolor: 'transparent',
          borderRadius: 0
        }}
      >
        <Typography 
          variant="h4" 
          component="h1" 
          sx={{ 
            fontWeight: 600,
            color: 'text.primary',
            fontSize: { xs: '1.5rem', sm: '2rem' }
          }}
        >
          Products List - Inventory Management
        </Typography>
        <Stack direction="row" spacing={1} alignItems="center">
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={handleRefresh}
            sx={{
              borderRadius: 2,
              textTransform: 'none',
              px: 3,
              py: 1
            }}
            data-testid="refresh-products-button"
          >
            Refresh
          </Button>
          <Button
            variant="contained"
            color="success"
            startIcon={<AddIcon />}
            onClick={handleAddClick}
            sx={{
              borderRadius: 2,
              textTransform: 'none',
              px: 3,
              py: 1,
              '&:hover': {
                backgroundColor: 'success.dark',
              }
            }}
            data-testid="add-product-button"
          >
            Add Product
          </Button>
        </Stack>
      </Paper>

      <DataGrid
        rows={products}
        columns={columns}
        loading={loading}
        pageSizeOptions={[5, 10, 25]}
        sortModel={sortModel}
        onSortModelChange={(model) => setSortModel(model)}
        initialState={{
          pagination: { paginationModel: { pageSize: 10 } },
        }}
        disableRowSelectionOnClick
        sx={{
          '& .MuiDataGrid-columnHeaders': {
            backgroundColor: 'black',
            color: 'white',
            '& .MuiIconButton-root': {
              color: 'white'
            }
          },
          '& .MuiDataGrid-footerContainer': {
            backgroundColor: 'black',
            color: 'white',
            '& .MuiTablePagination-root': {
              color: 'white'
            },
            '& .MuiIconButton-root': {
              color: 'white'
            }
          },
          '& .MuiDataGrid-row:nth-of-type(odd)': {
            backgroundColor: '#f5f5f5'
          },
          '& .MuiDataGrid-row:nth-of-type(even)': {
            backgroundColor: 'white'
          },
          '& .MuiDataGrid-row:hover': {
            backgroundColor: '#e3f2fd'
          }
        }}
      />
    </Box>
  );
}; 