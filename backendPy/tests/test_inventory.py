import pytest
from app.main import app
from app.models import Product, Inventory
from app.database import Base, init_db

@pytest.fixture(scope="function")
def test_product(client, db_session):
    # Initialize database
    engine = init_db("sqlite:///:memory:")
    Base.metadata.create_all(bind=engine)
    
    # Create product
    response = client.post(
        "/api/products/",
        json={"name": "Test Product", "description": "Test Description", "price": 99.99}
    )
    product_data = response.json()
    
    # Create inventory record
    inventory = Inventory(product_id=product_data["id"], quantity=0)
    db_session.add(inventory)
    db_session.commit()
    
    return product_data

def test_get_product_inventory(client, test_product):
    response = client.get(f"/api/inventory/products/{test_product['id']}")
    assert response.status_code == 200
    data = response.json()
    assert data["product_id"] == test_product["id"]
    assert data["quantity"] == 0  # Initial quantity should be 0

def test_add_stock(client, test_product):
    response = client.post(
        f"/api/inventory/products/{test_product['id']}/stock",
        json={"quantity": 10}
    )
    assert response.status_code == 200
    data = response.json()
    assert data["current_stock"] == 10
    assert "Added 10 units to stock" in data["message"]

def test_remove_stock(client, test_product):
    # First add stock
    client.post(
        f"/api/inventory/products/{test_product['id']}/stock",
        json={"quantity": 10}
    )
    
    # Then remove some
    response = client.delete(
        f"/api/inventory/products/{test_product['id']}/stock",
        json={"quantity": 5}
    )
    assert response.status_code == 200
    data = response.json()
    assert data["current_stock"] == 5
    assert "Removed 5 units from stock" in data["message"]

def test_remove_too_much_stock(client, test_product):
    # First add stock
    client.post(
        f"/api/inventory/products/{test_product['id']}/stock",
        json={"quantity": 10}
    )
    
    # Try to remove more than available
    response = client.delete(
        f"/api/inventory/products/{test_product['id']}/stock",
        json={"quantity": 15}
    )
    assert response.status_code == 400
    assert "Not enough stock available" in response.json()["detail"]