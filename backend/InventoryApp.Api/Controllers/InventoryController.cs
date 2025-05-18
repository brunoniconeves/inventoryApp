using InventoryApp.Api.DTOs;
using InventoryApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    [HttpGet("products/{productId}")]
    public async Task<ActionResult<InventoryDto>> GetProductStock(int productId)
    {
        try
        {
            var inventory = await _inventoryService.GetProductStockAsync(productId);
            if (inventory == null)
            {
                return NotFound();
            }
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stock for product {ProductId}", productId);
            return StatusCode(500, "An error occurred while retrieving the stock information");
        }
    }

    [HttpPost("products/{productId}/stock")]
    public async Task<ActionResult<InventoryDto>> AddStock(int productId, [FromBody] UpdateStockDto stockDto)
    {
        try
        {
            var result = await _inventoryService.AddStockAsync(productId, stockDto);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding stock to product {ProductId}", productId);
            return StatusCode(500, "An error occurred while adding stock");
        }
    }

    [HttpDelete("products/{productId}/stock")]
    public async Task<ActionResult<InventoryDto>> RemoveStock(int productId, [FromBody] UpdateStockDto stockDto)
    {
        try
        {
            var result = await _inventoryService.RemoveStockAsync(productId, stockDto);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing stock from product {ProductId}", productId);
            return StatusCode(500, "An error occurred while removing stock");
        }
    }
} 