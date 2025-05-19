/// <reference types="cypress" />
describe('Product List Page', () => {
  beforeEach(() => {
    // Visit the page where ProductList is rendered
    cy.visit('http://localhost:3000');
  });

  it('should display the header with the Add Product button', () => {
    // Verify the header text
    cy.contains('h1', 'Products List - Inventory Management').should('be.visible');

    // Verify the Add button is visible
    cy.get('[data-testid="add-product-button"]').should('be.visible');
  });

  it('should display the datagrid of products list of products', () => {
    // Verify that the div with MuiDataGrid-root class is present
    cy.get('.MuiDataGrid-root').should('exist');
  });

  it('should navigate to create new product page', () => {    
    // Simulate a click on the Add button
    cy.get('[data-testid="add-product-button"]').click();

    // Verify we're on the create product page
    cy.contains('h1', 'Create New Product').should('be.visible');
  });

  it('should navigate back to list from create product page', () => {
    // Go to create product page
    cy.get('[data-testid="add-product-button"]').click();
    cy.contains('h1', 'Create New Product').should('be.visible');

    // Click the cancel button
    cy.get('[data-testid="cancel-btn"]').click();

    // Verify we're back on the list page
    cy.contains('h1', 'Products List - Inventory Management').should('be.visible');
  });

  it('should save the new product', () => {
    // Go to create product page
    cy.get('[data-testid="add-product-button"]').click();

    // Fill in the form fields with new data
    cy.get('[data-testid="name-input-field"]').should('be.visible').clear().type('000 New Test Product Name');
    cy.get('[data-testid="sku-input-field"]').should('be.visible').clear().type('NewTestSKU123');
    cy.get('[data-testid="description-input-field"]').should('be.visible').clear().type('New Test Product Description');
    cy.get('[data-testid="price-input-field"]').should('be.visible').clear().type('55.55');
    cy.get('[data-testid="initial-stock-input-field"]').should('be.visible').clear().type('200');

    // Click the Save button
    cy.get('[data-testid="save-btn"]').click();

    // Verify we're back on the list page and the new product is displayed
    cy.contains('h1', 'Products List - Inventory Management').should('be.visible');
    cy.contains('000 New Test Product Name').should('be.visible');
    cy.contains('NewTestSKU123').should('be.visible');
    cy.contains('New Test Product Description').should('be.visible');
    cy.contains('55.55').should('be.visible');
    cy.contains('200').should('be.visible');    
  });

  it('should navigate to edit product page', () => {
    // Wait for the data grid to be loaded
    cy.get('.MuiDataGrid-root').should('be.visible');
    
    // Find and click the edit button in the first row
    cy.get('.MuiDataGrid-row')
      .first()
      .find('button[aria-label="Edit Product"]')
      .should('be.visible')
      .click();

    // Verify we're on the edit page
    cy.contains('h1', 'Edit Product').should('be.visible');
  });

  it('should save the edited product', () => {
    // Wait for the data grid to be loaded
    cy.get('.MuiDataGrid-root').should('be.visible');
    
    // Find and click the edit button in the first row
    cy.get('.MuiDataGrid-row')
      .first()
      .find('button[aria-label="Edit Product"]')
      .should('be.visible')
      .click();

    // Fill in the form fields with new data
    cy.get('[data-testid="name-input-field"]').should('be.visible').clear().type('000 Updated Product Name');
    cy.get('[data-testid="sku-input-field"]').should('be.visible').clear().type('NewSKU123');
    cy.get('[data-testid="description-input-field"]').should('be.visible').clear().type('Updated Product Description');
    cy.get('[data-testid="price-input-field"]').should('be.visible').clear().type('99.99');
    cy.get('[data-testid="stock-quantity-input-field"]').should('be.visible').clear().type('10');

    // Click the Save button
    cy.get('[data-testid="save-btn"]').click();

    // Verify we're back on the list page and the updated product is displayed
    cy.contains('h1', 'Products List - Inventory Management').should('be.visible');
    cy.contains('000 Updated Product Name').should('be.visible');
    cy.contains('NewSKU123').should('be.visible');
    cy.contains('Updated Product Description').should('be.visible');
    cy.contains('99.99').should('be.visible');
    cy.contains('10').should('be.visible');    
  });

  it('should show confirmation when deleting product', () => {
    // Stub the window.confirm to always return false and verify it was called
    cy.window().then(win => {
      cy.stub(win, 'confirm').as('confirm').returns(false);
    });

    // Wait for the data grid to be loaded
    cy.get('.MuiDataGrid-root').should('be.visible');
    
    // Find and click the delete button in the first row
    cy.get('.MuiDataGrid-row')
      .first()
      .find('button[aria-label="Delete Product"]')
      .should('be.visible')
      .click();

    // Verify the confirmation was shown
    cy.get('@confirm').should('have.been.called');
  });

  it('should delete product when confirmed', () => {
    // Stub the window.confirm to return true
    cy.window().then(win => {
      cy.stub(win, 'confirm').as('confirm').returns(true);
    });

    // Wait for the data grid to be loaded and get initial row count
    cy.get('.MuiDataGrid-root').should('be.visible');
    cy.get('.MuiDataGrid-row').then((rows) => {
      const initialRowCount = rows.length;

      // Find and click the delete button in the first row
      cy.get('.MuiDataGrid-row')
        .first()
        .find('button[aria-label="Delete Product"]')
        .should('be.visible')
        .click();

      // Verify the confirmation was shown
      cy.get('@confirm').should('have.been.called');

      // Verify that the product is deleted
      cy.get('.MuiDataGrid-row').should('have.length', initialRowCount - 1);
    });
  });

  it('should show confirmation when deleting product from edit page', () => {
    // Stub the window.confirm to always return false and verify it was called
    cy.window().then(win => {
      cy.stub(win, 'confirm').as('confirm').returns(false);
    });

    // Wait for the data grid to be loaded
    cy.get('.MuiDataGrid-root').should('be.visible');
    
    // Find and click the edit button in the first row
    cy.get('.MuiDataGrid-row')
      .first()
      .find('button[aria-label="Edit Product"]')
      .should('be.visible')
      .click();

    // Click the delete button
    cy.get('[data-testid="delete-btn"]').should('be.visible').click();

    // Verify the confirmation was shown
    cy.get('@confirm').should('have.been.called');

    // Verify we're still on the edit page since we cancelled
    cy.contains('h1', 'Edit Product').should('be.visible');
  });

  it('should delete product from edit page when confirmed', () => {
    // Stub the window.confirm to return true
    cy.window().then(win => {
      cy.stub(win, 'confirm').as('confirm').returns(true);
    });

    // Wait for the data grid to be loaded and get initial row count
    cy.get('.MuiDataGrid-root').should('be.visible');
    cy.get('.MuiDataGrid-row').then((rows) => {
      const initialRowCount = rows.length;

      // Find and click the edit button in the first row
      cy.get('.MuiDataGrid-row')
        .first()
        .find('button[aria-label="Edit Product"]')
        .should('be.visible')
        .click();

      // Click the delete button
      cy.get('[data-testid="delete-btn"]').should('be.visible').click();

      // Verify the confirmation was shown
      cy.get('@confirm').should('have.been.called');

      // Verify we're back on the list page
      cy.contains('h1', 'Products List - Inventory Management').should('be.visible');

      // Verify that the product is deleted
      cy.get('.MuiDataGrid-row').should('have.length', initialRowCount - 1);
    });
  });
});