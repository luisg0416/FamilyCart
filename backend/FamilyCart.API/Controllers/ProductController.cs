namespace FamilyCart.API.Controllers
{
    using FamilyCart.Core.DTOs;
    using FamilyCart.Core.Models;
    using FamilyCart.Infrastructure.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Claims;
    using System.Security.Cryptography;

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _appDBContext;
        private readonly UserManager<User> _userManager;

        public ProductController(AppDbContext appDbContext, UserManager<User> userManager)
        {
            _appDBContext = appDbContext;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Id showing as null");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == createProductDto.FamilyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            var family = await _appDBContext.Families.FindAsync(createProductDto.FamilyId);

            if (family == null)
            {
                return NotFound("Family not found");
            }

            var store = await _appDBContext.Stores.FindAsync(createProductDto.StoreId);

            if (store == null)
            {
                return NotFound("Store not found");
            }

            if (store.FamilyId != null && store.FamilyId != createProductDto.FamilyId)
            {
                return StatusCode(403, "This store does not belong to your family.");
            }

            var product = new Product
            {
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Type = ProductType.Family,
                Family = family,
                FamilyId = createProductDto.FamilyId,
                Store = store,
                StoreId = createProductDto.StoreId
            };

            _appDBContext.Products.Add(product);
            await _appDBContext.SaveChangesAsync();

            var productResponseDto = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Type = product.Type,
                FamilyId = product.FamilyId,
                StoreId = product.StoreId,
                CreatedAt = product.CreatedAt
            };

            return Ok(productResponseDto);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Id showing as null");
            }

            var product = await _appDBContext.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            if(product.FamilyId == null)
            {
                return StatusCode(403, "Trying to edit Instacart Product");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == product.FamilyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            if (updateProductDto.Name != null)
            {
                product.Name = updateProductDto.Name;
            }

            if (updateProductDto.Price != null)
            {
                product.Price = updateProductDto.Price.Value; // .Value unwraps the nullable decimal
            }

            if (updateProductDto.ImageUrl != null)
            {
                product.ImageUrl = updateProductDto.ImageUrl;
            }

            await _appDBContext.SaveChangesAsync();

            var productResponseDto = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Type = product.Type,
                FamilyId = product.FamilyId,
                StoreId = product.StoreId,
                CreatedAt = product.CreatedAt
            };

            return Ok(productResponseDto);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Id showing as null");
            }

            var product = await _appDBContext.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            if(product.FamilyId == null)
            {
                return StatusCode(403, "Cannot delete an Instacart Product");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == product.FamilyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            _appDBContext.Products.Remove(product);
            await _appDBContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetProducts(int storeId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Id showing as null");
            }

            var store = await _appDBContext.Stores.FindAsync(storeId);

            if (store == null)
            {
                return NotFound("Store not found");
            }

            if (store.FamilyId != null)
            {
                bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == store.FamilyId && fm.UserId == userId);

                if (!isMember)
                {
                    return StatusCode(403, "User is not a member of this Family");
                }
            }

            var products = await _appDBContext.Products.Where(p => p.StoreId == storeId).ToListAsync();

            var productsResponseDto = products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Type = p.Type,
                FamilyId = p.FamilyId,
                StoreId = p.StoreId,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(productsResponseDto);
        }

    }
}