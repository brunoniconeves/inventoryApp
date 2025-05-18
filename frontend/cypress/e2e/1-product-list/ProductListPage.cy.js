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

  it('should open the create new product modal', () => {    
    // Simulate a click on the Add button
    cy.get('[data-testid="add-product-button"]').click();

    // Verify the expected behavior after clicking (e.g., modal opens)
    cy.contains('Create New Product').should('be.visible'); // Adjust based on your app
  });
});