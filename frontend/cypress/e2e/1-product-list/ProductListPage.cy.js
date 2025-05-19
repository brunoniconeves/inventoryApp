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

    // Verify the expected behavior after clicking (e.g., modal opens)
    cy.contains('Create New Product').should('be.visible'); // Adjust based on your app

  });

  it('should close the create new product modal', () => {
    // Simulate a click on the Add button
    cy.get('[data-testid="add-product-button"]').click();

    // Verify the expected behavior after clicking (e.g., modal opens)
    cy.contains('Create New Product').should('be.visible'); // Adjust based on your app

    // Verify that close modal function works
    cy.get('[data-testid="edit-modal-cancel-btn"]').click(); // Adjust based on your app

    // Verify the modal is closed
    cy.contains('Create New Product').should('not.exist'); // Adjust based on your app
  });

  it('should save the new product', () => {
    // Simulate a click on the Add button
    cy.get('[data-testid="add-product-button"]').click();

    // Verify the expected behavior after clicking (e.g., modal opens)
    cy.contains('Create New Product').should('be.visible'); // Adjust based on your app

    // Fill in the form fields with new data
    cy.get('[data-testid="name-input"]').clear().type('000 New Test Product Name');
    cy.get('[data-testid="sku-input"]').clear().type('NewTestSKU123');
    cy.get('[data-testid="description-input"]').clear().type('New Test Product Description');
    cy.get('[data-testid="price-input"]').clear().type('55.55');
    cy.get('[data-testid="initial-stock-input"]').clear().type('200');

    // Simulate a click on the Save button
    cy.get('[data-testid="save-btn"]').click();

    // Verify that the modal is closed and the product list is updated
    cy.contains('Create New Product').should('not.exist'); // Adjust based on your app

    // Verify that the updated product is displayed in the datagrid
    cy.contains('New Test Product Name').should('be.visible');
    cy.contains('NewTestSKU123').should('be.visible');
    cy.contains('New Test Product Description').should('be.visible');
    cy.contains('55.55').should('be.visible');
    cy.contains('200').should('be.visible');    
  });

  it('should display the edit product modal', () => {
    // Simulate a click on the first row of the datagrid
    cy.get('.MuiDataGrid-row').first().find('[aria-label="Edit Product"]').click();

    // Verify the expected behavior after clicking (e.g., modal opens)
    cy.contains('Edit Product').should('be.visible'); // Adjust based on your app
  });

  it('should save the edited product', () => {
    // Simulate a click on the first row of the datagrid
    cy.get('.MuiDataGrid-row').first().find('[aria-label="Edit Product"]').click();

    // Verify the expected behavior after clicking (e.g., modal opens)
    cy.contains('Edit Product').should('be.visible'); // Adjust based on your app

    // Fill in the form fields with new data
    cy.get('[data-testid="name-input"]').clear().type('000 Updated Product Name');
    cy.get('[data-testid="sku-input"]').clear().type('NewSKU123');
    cy.get('[data-testid="description-input"]').clear().type('Updated Product Description');
    cy.get('[data-testid="price-input"]').clear().type('99.99');
    cy.get('[data-testid="stock-quantity-input"]').clear().type('10');

    // Simulate a click on the Save button
    cy.get('[data-testid="save-btn"]').click();

    // Verify that the modal is closed and the product list is updated
    cy.contains('Edit Product').should('not.exist'); // Adjust based on your app

    // Verify that the updated product is displayed in the datagrid
    cy.contains('000 Updated Product Name').should('be.visible');
    cy.contains('NewSKU123').should('be.visible');
    cy.contains('Updated Product Description').should('be.visible');
    cy.contains('99.99').should('be.visible');
    cy.contains('10').should('be.visible');    
  });

  it('On delete product, display confirmation question', () => {
    // Simulate a click on the first row of the datagrid
    cy.get('.MuiDataGrid-row').first().find('[aria-label="Delete Product"]').click();

    cy.contains('Delete Product').should('exist');
    cy.get('[data-testid="confirm-delete-btn"]').should('exist');
  });

  it('On delete confirm, remove the product', () => {
    // Count the number of rows before deletion
    cy.get('.MuiDataGrid-row').then((rows) => {
      const initialRowCount = rows.length;

      // Simulate a click on the first row of the datagrid
      cy.get('.MuiDataGrid-row').first().find('[aria-label="Delete Product"]').click();

      // Simulate a click on the confirm delete button
      cy.get('[data-testid="confirm-delete-btn"]').click();

      // Verify that the product is deleted
      cy.get('.MuiDataGrid-row').should('have.length', initialRowCount - 1);
    });
  });
});