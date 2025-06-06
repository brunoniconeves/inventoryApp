from contextlib import asynccontextmanager
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.routers import products, inventory
from app.database import engine, Base, SessionLocal
from app.seed import seed_database
from sqlalchemy import text
import time
from sqlalchemy.exc import OperationalError

@asynccontextmanager
async def lifespan(app: FastAPI):
    # Setup - wait for database
    max_retries = 5
    retry_delay = 5  # seconds

    for retry in range(max_retries):
        try:
            # Test the connection
            with engine.connect() as conn:
                conn.execute(text("SELECT 1"))
                conn.commit()
            print("Successfully connected to the database")
            break
        except OperationalError as e:
            if retry < max_retries - 1:
                print(f"Database connection attempt {retry + 1} failed. Retrying in {retry_delay} seconds...")
                time.sleep(retry_delay)
            else:
                print("Failed to connect to the database after multiple attempts")
                raise e

    # Create database tables
    Base.metadata.create_all(bind=engine)
    
    # Seed the database
    db = SessionLocal()
    try:
        seed_database(db)
    finally:
        db.close()

    yield
    # Cleanup (if needed)
    pass

app = FastAPI(
    title="Inventory Management System API",
    description="API for managing inventory and products",
    version="1.0.0",
    lifespan=lifespan
)

# Configure CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000"],  # Frontend URL
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Include routers
app.include_router(products.router, prefix="/api", tags=["products"])
app.include_router(inventory.router, prefix="/api/inventory", tags=["inventory"])

@app.get("/")
async def root():
    return {"message": "Welcome to Inventory Management System API"} 