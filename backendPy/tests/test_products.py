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

def test_create_product(client):
    response = client.post(
        "/api/products/",
        json={"name": "Test Product", "description": "Test Description", "price": 99.99}
    )
    assert response.status_code == 200
    data = response.json()
    assert data["name"] == "Test Product"
    assert data["description"] == "Test Description"
    assert data["price"] == 99.99
    assert "id" in data

def test_get_product(client):
    # First create a product
    response = client.post(
        "/api/products/",
        json={"name": "Test Product", "description": "Test Description", "price": 99.99}
    )
    product_id = response.json()["id"]
    
    # Then get it
    response = client.get(f"/api/products/{product_id}")
    assert response.status_code == 200
    data = response.json()
    assert data["name"] == "Test Product"
    assert data["id"] == product_id

def test_update_product(client):
    # First create a product
    response = client.post(
        "/api/products/",
        json={"name": "Test Product", "description": "Test Description", "price": 99.99}
    )
    product_id = response.json()["id"]
    
    # Then update it
    response = client.put(
        f"/api/products/{product_id}",
        json={"name": "Updated Product", "description": "Updated Description", "price": 199.99}
    )
    assert response.status_code == 200
    data = response.json()
    assert data["name"] == "Updated Product"
    assert data["description"] == "Updated Description"
    assert data["price"] == 199.99

def test_delete_product(client):
    # First create a product
    response = client.post(
        "/api/products/",
        json={"name": "Test Product", "description": "Test Description", "price": 99.99}
    )
    product_id = response.json()["id"]
    
    # Then delete it
    response = client.delete(f"/api/products/{product_id}")
    assert response.status_code == 200
    
    # Verify it's gone
    response = client.get(f"/api/products/{product_id}")
    assert response.status_code == 404 