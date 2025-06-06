from sqlalchemy.orm import Session
from . import models

def seed_database(db: Session):
    # Check if we already have products
    if db.query(models.Product).count() > 0:
        print("Database already has products, skipping seed")
        return

    # Initial products data
    products = [
        {
            "name": "Laptop",
            "description": "High-performance laptop for professional use",
            "price": 1299.99
        },
        {
            "name": "Smartphone",
            "description": "Latest model smartphone with advanced features",
            "price": 799.99
        },
        {
            "name": "Headphones",
            "description": "Wireless noise-canceling headphones",
            "price": 199.99
        },
        {
            "name": "Monitor",
            "description": "27-inch 4K LED Monitor",
            "price": 349.99
        },
        {
            "name": "Keyboard",
            "description": "Mechanical gaming keyboard with RGB lighting",
            "price": 129.99
        }
    ]

    # Add products and their inventory
    for product_data in products:
        # Create product
        product = models.Product(**product_data)
        db.add(product)
        db.commit()
        db.refresh(product)

        # Create inventory with 0 initial quantity
        inventory = models.Inventory(product_id=product.id, quantity=10)
        db.add(inventory)
        db.commit()

    print("Database seeded successfully with initial products") 