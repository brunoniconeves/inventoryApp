import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Container, CssBaseline, ThemeProvider } from '@mui/material';
import { ProductProvider } from './contexts/ProductContext';
import { ProductList } from './components/ProductList';
import { CreateProductPage } from './pages/CreateProductPage';
import { EditProductPage } from './pages/EditProductPage';
import { createTheme} from '@mui/material/styles';

//add color theme
const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <ProductProvider>
        <Router>
          <Container maxWidth="lg" sx={{ py: 4 }}>
            <Routes>
              <Route path="/" element={<ProductList />} />
              <Route path="/products/new" element={<CreateProductPage />} />
              <Route path="/products/:id/edit" element={<EditProductPage />} />
            </Routes>
          </Container>
        </Router>
      </ProductProvider>
    </ThemeProvider>
  );
}

export default App;
