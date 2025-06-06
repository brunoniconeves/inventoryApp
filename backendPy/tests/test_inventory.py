from fastapi.testclient import TestClient
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from sqlalchemy.pool import StaticPool
import pytest
from app.main import app
from app.database import Base, get_db

# Create in-memory SQLite database for testing
SQLALCHEMY_DATABASE_URL = "sqlite://"
engine = create_engine(
    SQLALCHEMY_DATABASE_URL,
    connect_args={"check_same_thread": False},
    poolclass=StaticPool,
)
TestingSessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

def override_get_db():
    db = TestingSessionLocal()
    try:
        yield db
    finally:
        db.close()

app.dependency_overrides[get_db] = override_get_db

@pytest.fixture
def client():
    Base.metadata.create_all(bind=engine)
    yield TestClient(app)
    Base.metadata.drop_all(bind=engine)

@pytest.fixture
def test_product(client):
    response = client.post(
        "/api/products/",
        json={"name": "Test Product", "description": "Test Description", "price": 99.99}
    )
    return response.json()

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