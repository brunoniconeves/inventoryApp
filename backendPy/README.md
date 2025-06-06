# Python Backend for Inventory Management System

This is the Python (FastAPI) version of the backend for the Inventory Management System.

## Technology Stack

- Python 3.11
- FastAPI
- SQLAlchemy
- PostgreSQL
- pytest for testing

## Local Development Setup

1. Create a virtual environment:
```bash
python -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate
```

2. Install dependencies:
```bash
pip install -r requirements.txt
```

3. Run the application:
```bash
uvicorn app.main:app --reload --port 5000
```

The API will be available at http://localhost:5000

## Running Tests

To run the tests:
```bash
pytest
```

To run tests with coverage:
```bash
pytest --cov=app tests/
```

## API Documentation

When the application is running, you can access:
- Swagger UI: http://localhost:5000/docs
- ReDoc: http://localhost:5000/redoc

## Docker Setup

To run the application using Docker:

```bash
# From the root directory (where docker-compose-python.yml is located)
docker-compose -f docker-compose-python.yml up --build
```

## API Endpoints

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Inventory
- `GET /api/inventory/products/{productId}` - Get product stock information
- `POST /api/inventory/products/{productId}/stock` - Add stock to product
- `DELETE /api/inventory/products/{productId}/stock` - Remove stock from product 