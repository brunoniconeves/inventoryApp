import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Container, CssBaseline, ThemeProvider } from '@mui/material';
import { ProductProvider } from './contexts/ProductContext';
import { ProductList } from './components/ProductList';
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
        <BrowserRouter>
          <Container maxWidth="lg" sx={{ py: 4 }}>
            <Routes>
              <Route path="/" element={<ProductList />} />
            </Routes>
          </Container>
        </BrowserRouter>
      </ProductProvider>
    </ThemeProvider>
  );
}

export default App;
