import React, { useEffect, useState } from 'react';
import { 
  DataGrid, 
  GridColDef, 
  GridRenderCellParams,
  GridSortModel
} from '@mui/x-data-grid';
import { Box, IconButton, Tooltip, Stack } from '@mui/material';
import { Edit as EditIcon, Delete as DeleteIcon } from '@mui/icons-material';
import { useProducts } from '../contexts/ProductContext';
import { ProductEditModal } from './ProductEditModal';
import { DeleteConfirmationDialog } from './DeleteConfirmationDialog';
import { Product } from '../services/productService';

export const ProductList: React.FC = () => {
  const { products, loading, error, deleteProduct, loadProducts } = useProducts();
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [productToDelete, setProductToDelete] = useState<Product | null>(null);
  const [sortModel, setSortModel] = useState<GridSortModel>([
    {
      field: 'name',
      sort: 'asc',
    },
  ]);

  useEffect(() => {
    loadProducts();
  }, [loadProducts]);

  const handleEditClick = (product: Product) => {
    setSelectedProduct(product);
    setIsModalOpen(true);
  };

  const handleDeleteClick = (product: Product) => {
    setProductToDelete(product);
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (productToDelete) {
      try {
        await deleteProduct(productToDelete.id);
      } catch (error) {
        console.error('Failed to delete product:', error);
      } finally {
        setDeleteDialogOpen(false);
        setProductToDelete(null);
      }
    }
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
            >
              <EditIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Delete Product" arrow>
            <IconButton
              onClick={() => handleDeleteClick(params.row)}
              size="small"
              sx={{
                backgroundColor: '#ff4444',
                color: 'white',
                '&:hover': {
                  backgroundColor: '#cc0000',
                },
                width: 32,
                height: 32,
              }}
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
      
      {selectedProduct && (
        <ProductEditModal
          open={isModalOpen}
          onClose={() => {
            setIsModalOpen(false);
            setSelectedProduct(null);
          }}
          product={selectedProduct}
        />
      )}

      <DeleteConfirmationDialog
        open={deleteDialogOpen}
        onClose={() => {
          setDeleteDialogOpen(false);
          setProductToDelete(null);
        }}
        onConfirm={handleDeleteConfirm}
        productName={productToDelete?.name || ''}
      />
    </Box>
  );
}; 