using InventoryApp.Api.DTOs;
using InventoryApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, "An error occurred while retrieving products");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Product ID must be greater than zero");
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, "An error occurred while retrieving the product");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto productDto)
    {
        try
        {
            if (productDto == null)
            {
                return BadRequest("Product data is required");
            }

            if (string.IsNullOrWhiteSpace(productDto.Name))
            {
                return BadRequest("Product name is required");
            }

            if (string.IsNullOrWhiteSpace(productDto.SKU))
            {
                return BadRequest("Product SKU is required");
            }

            if (productDto.Price <= 0)
            {
                return BadRequest("Product price must be greater than zero");
            }

            if (productDto.StockQuantity < 0)
            {
                return BadRequest("Product stock quantity cannot be negative");
            }

            var product = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, "An error occurred while creating the product");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto productDto)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Product ID must be greater than zero");
            }

            if (productDto == null)
            {
                return BadRequest("Product data is required");
            }

            if (string.IsNullOrWhiteSpace(productDto.Name))
            {
                return BadRequest("Product name is required");
            }

            if (string.IsNullOrWhiteSpace(productDto.SKU))
            {
                return BadRequest("Product SKU is required");
            }

            if (productDto.Price <= 0)
            {
                return BadRequest("Product price must be greater than zero");
            }

            var product = await _productService.UpdateProductAsync(id, productDto);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, "An error occurred while updating the product");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest("Product ID must be greater than zero");
            }

            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user"))
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, "An error occurred while deleting the product");
        }
    }
} 