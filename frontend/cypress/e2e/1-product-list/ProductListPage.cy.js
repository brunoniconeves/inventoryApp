/// <reference types="cypress" />
describe('Product List Page', () => {
  beforeEach(() => {
    // Visit the page where ProductList is rendered
    cy.visit('http://localhost:3000'); // Adjust the URL to match your app's route
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

  it('should open the create new product modal', () => {    
    // Simulate a click on the Add button
    cy.get('[data-testid="add-product-button"]').click();

    // Wait for the modal to be visible
    cy.get('[data-testid="product-edit-modal-title"]')
      .should('be.visible')
      .and('contain', 'Create New Product');
  });

  it('should close the create new product modal', () => {
    // Simulate a click on the Add button
    cy.get('[data-testid="add-product-button"]').click();

    // Wait for the modal to be visible
    cy.get('[data-testid="product-edit-modal-title"]')
      .should('be.visible')
      .and('contain', 'Create New Product');

    // Click the cancel button
    cy.get('[data-testid="edit-modal-cancel-btn"]').click();

    // Verify the modal is closed
    cy.get('[data-testid="product-edit-modal-title"]').should('not.exist');
  });

  it('should save the new product', () => {
    // Simulate a click on the Add button
    cy.get('[data-testid="add-product-button"]').click();

    // Wait for the modal to be visible
    cy.get('[data-testid="product-edit-modal-title"]')
      .should('be.visible')
      .and('contain', 'Create New Product');

    // Fill in the form fields with new data
    cy.get('[data-testid="name-input"]').should('be.visible').clear().type('000 New Test Product Name');
    cy.get('[data-testid="sku-input"]').should('be.visible').clear().type('NewTestSKU123');
    cy.get('[data-testid="description-input"]').should('be.visible').clear().type('New Test Product Description');
    cy.get('[data-testid="price-input"]').should('be.visible').clear().type('55.55');
    cy.get('[data-testid="initial-stock-input"]').should('be.visible').clear().type('200');

    // Click the Save button
    cy.get('[data-testid="save-btn"]').click();

    // Wait for the modal to close
    cy.get('[data-testid="product-edit-modal-title"]').should('not.exist');

    // Verify that the new product is displayed in the datagrid
    cy.contains('000 New Test Product Name').should('be.visible');
    cy.contains('NewTestSKU123').should('be.visible');
    cy.contains('New Test Product Description').should('be.visible');
    cy.contains('55.55').should('be.visible');
    cy.contains('200').should('be.visible');    
  });

  it('should display the edit product modal', () => {
    // Wait for the data grid to be loaded
    cy.get('.MuiDataGrid-root').should('be.visible');
    
    // Find and click the edit button in the first row
    cy.get('.MuiDataGrid-row')
      .first()
      .find('button[aria-label="Edit Product"]')
      .should('be.visible')
      .click();

    // Verify the edit modal is visible
    cy.get('[data-testid="product-edit-modal-title"]')
      .should('be.visible')
      .and('contain', 'Edit Product');
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

    // Wait for the modal to be visible
    cy.get('[data-testid="product-edit-modal-title"]')
      .should('be.visible')
      .and('contain', 'Edit Product');

    // Fill in the form fields with new data
    cy.get('[data-testid="name-input"]').should('be.visible').clear().type('000 Updated Product Name');
    cy.get('[data-testid="sku-input"]').should('be.visible').clear().type('NewSKU123');
    cy.get('[data-testid="description-input"]').should('be.visible').clear().type('Updated Product Description');
    cy.get('[data-testid="price-input"]').should('be.visible').clear().type('99.99');
    cy.get('[data-testid="stock-quantity-input"]').should('be.visible').clear().type('10');

    // Click the Save button
    cy.get('[data-testid="save-btn"]').click();

    // Wait for the modal to close
    cy.get('[data-testid="product-edit-modal-title"]').should('not.exist');

    // Verify that the updated product is displayed in the datagrid
    cy.contains('000 Updated Product Name').should('be.visible');
    cy.contains('NewSKU123').should('be.visible');
    cy.contains('Updated Product Description').should('be.visible');
    cy.contains('99.99').should('be.visible');
    cy.contains('10').should('be.visible');    
  });

  it('On delete product, display confirmation question', () => {
    // Wait for the data grid to be loaded
    cy.get('.MuiDataGrid-root').should('be.visible');
    
    // Find and click the delete button in the first row
    cy.get('.MuiDataGrid-row')
      .first()
      .find('button[aria-label="Delete Product"]')
      .should('be.visible')
      .click();

    // Verify the delete confirmation dialog is visible
    cy.contains('Delete Product').should('be.visible');
    cy.get('[data-testid="confirm-delete-btn"]').should('be.visible');
  });

  it('On delete confirm, remove the product', () => {
    // Count the number of rows before deletion
    cy.get('.MuiDataGrid-row').then((rows) => {
      const initialRowCount = rows.length;

      // Find and click the delete button in the first row
      cy.get('.MuiDataGrid-row')
        .first()
        .find('button[aria-label="Delete Product"]')
        .should('be.visible')
        .click();

      // Click the confirm delete button
      cy.get('[data-testid="confirm-delete-btn"]').click();

      // Verify that the product is deleted
      cy.get('.MuiDataGrid-row').should('have.length', initialRowCount - 1);
    });
  });
});