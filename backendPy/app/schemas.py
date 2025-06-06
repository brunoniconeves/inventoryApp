from pydantic import BaseModel
from datetime import datetime
from typing import Optional

class ProductBase(BaseModel):
    name: str
    description: str
    price: float

class ProductCreate(ProductBase):
    pass

class ProductUpdate(ProductBase):
    name: Optional[str] = None
    description: Optional[str] = None
    price: Optional[float] = None

class InventoryBase(BaseModel):
    quantity: int

class InventoryCreate(InventoryBase):
    product_id: int

class InventoryUpdate(InventoryBase):
    pass

class Inventory(InventoryBase):
    id: int
    product_id: int
    created_at: datetime
    updated_at: datetime

    class Config:
        from_attributes = True

class Product(ProductBase):
    id: int
    created_at: datetime
    updated_at: datetime
    inventory: Optional[Inventory] = None

    class Config:
        from_attributes = True 