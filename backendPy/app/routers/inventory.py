from fastapi import APIRouter, Depends, HTTPException
from sqlalchemy.orm import Session
from ..database import get_db
from .. import models, schemas
from datetime import datetime

router = APIRouter()

@router.get("/products/{product_id}", response_model=schemas.Inventory)
def get_product_inventory(product_id: int, db: Session = Depends(get_db)):
    inventory = db.query(models.Inventory).filter(models.Inventory.product_id == product_id).first()
    if inventory is None:
        raise HTTPException(status_code=404, detail="Inventory not found for this product")
    return inventory

@router.post("/products/{product_id}/stock")
def add_stock(product_id: int, stock_update: schemas.InventoryUpdate, db: Session = Depends(get_db)):
    inventory = db.query(models.Inventory).filter(models.Inventory.product_id == product_id).first()
    if inventory is None:
        raise HTTPException(status_code=404, detail="Inventory not found for this product")
    
    inventory.quantity += stock_update.quantity
    inventory.updated_at = datetime.utcnow()
    db.commit()
    db.refresh(inventory)
    
    return {"message": f"Added {stock_update.quantity} units to stock", "current_stock": inventory.quantity}

@router.delete("/products/{product_id}/stock")
def remove_stock(product_id: int, stock_update: schemas.InventoryUpdate, db: Session = Depends(get_db)):
    inventory = db.query(models.Inventory).filter(models.Inventory.product_id == product_id).first()
    if inventory is None:
        raise HTTPException(status_code=404, detail="Inventory not found for this product")
    
    if inventory.quantity < stock_update.quantity:
        raise HTTPException(status_code=400, detail="Not enough stock available")
    
    inventory.quantity -= stock_update.quantity
    inventory.updated_at = datetime.utcnow()
    db.commit()
    db.refresh(inventory)
    
    return {"message": f"Removed {stock_update.quantity} units from stock", "current_stock": inventory.quantity} 