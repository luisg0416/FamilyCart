namespace FamilyCart.API.Controllers
{
    using FamilyCart.Core.DTOs;
    using FamilyCart.Core.Models;
    using FamilyCart.Infrastructure.Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using System.Security.Claims;
    using Microsoft.EntityFrameworkCore;
   
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class ShoppingListController : ControllerBase
    {
        private readonly AppDbContext _appDBContext;
        private readonly UserManager<User> _userManager;

        public ShoppingListController(AppDbContext appDbContext, UserManager<User> userManager)
        {
            _appDBContext = appDbContext;
            _userManager = userManager;
        }

        [HttpGet("family/{familyId}")]
        public async Task<IActionResult> GetListsForFamily(int familyId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Id showing as null");
            }

            var user = await _userManager.FindByIdAsync(userIdString);

              if (user == null)
            {
                return Unauthorized("User not found.");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == familyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            var family = await _appDBContext.Families.FindAsync(familyId);

            if (family == null)
            {
                return NotFound("Family not found");
            }

            var shoppingLists = await _appDBContext.ShoppingLists.Where(sl => sl.FamilyId == familyId).ToListAsync();

            var shoppingListsDtos = shoppingLists.Select(sl => new ShoppingListResponseDto
            {
                Id = sl.Id,
                ListName = sl.ListName,
                FamilyId = familyId,
                StoreId = sl.StoreId,
                CreatedById = sl.CreatedById,
                CreatedAt = sl.CreatedAt
            }).ToList();

            return Ok(shoppingListsDtos);
        }

        // Work in progress
        [HttpPost]
        public async Task<IActionResult> CreateShoppingList(CreateShoppingListDto createShoppingListDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Id showing as null");
            }

            var user = await _userManager.FindByIdAsync(userIdString);

              if (user == null)
            {
                return Unauthorized("User not found.");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == createShoppingListDto.FamilyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            var family = await _appDBContext.Families.FindAsync(createShoppingListDto.FamilyId);

            if (family == null)
            {
                return NotFound("Family not found");
            }

            var store = await _appDBContext.Stores.FindAsync(createShoppingListDto.StoreId);

            if (store == null)
            {
                return NotFound("Store not found.");
            }

            if (store.FamilyId != null && store.FamilyId != createShoppingListDto.FamilyId)
            {
                return StatusCode(403, "This store does not belong to your family.");
            }

            var shoppingList = new ShoppingList
            {
                ListName = createShoppingListDto.ListName,
                Family = family,
                Store = store,
                CreatedByUser = user,
                FamilyId = createShoppingListDto.FamilyId,
                StoreId = createShoppingListDto.StoreId,
                CreatedById = userId
            };

            _appDBContext.ShoppingLists.Add(shoppingList);
            await _appDBContext.SaveChangesAsync();

            var shoppingListResponseDto = new ShoppingListResponseDto
            {
                Id = shoppingList.Id,
                ListName = createShoppingListDto.ListName,
                FamilyId = createShoppingListDto.FamilyId,
                StoreId = createShoppingListDto.StoreId,
                CreatedById = shoppingList.CreatedById,
                CreatedAt = shoppingList.CreatedAt
            };

            return Ok(shoppingListResponseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShoppingListName(int id, UpdateShoppingListDto updateShoppingListDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Id showing as null");
            }

            var user = await _userManager.FindByIdAsync(userIdString);

              if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var shoppingList = await _appDBContext.ShoppingLists.FindAsync(id);
            if(shoppingList == null)
            {
                return NotFound("Shopping List not found.");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == shoppingList.FamilyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            shoppingList.ListName = updateShoppingListDto.ListName;
            await _appDBContext.SaveChangesAsync();

            var shoppingListResponseDto = new ShoppingListResponseDto
            {
                Id = shoppingList.Id,
                ListName = shoppingList.ListName,
                FamilyId = shoppingList.FamilyId,
                StoreId = shoppingList.StoreId,
                CreatedById = shoppingList.CreatedById,
                CreatedAt = shoppingList.CreatedAt
            };

            return Ok(shoppingListResponseDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingList(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Id showing as null");
            }

            var user = await _userManager.FindByIdAsync(userIdString);

              if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var shoppingList = await _appDBContext.ShoppingLists.FindAsync(id);

            if(shoppingList == null)
            {
                return NotFound("Shopping List not found.");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == shoppingList.FamilyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            _appDBContext.ShoppingLists.Remove(shoppingList);
            await _appDBContext.SaveChangesAsync();

            return NoContent();
        }
    }
}