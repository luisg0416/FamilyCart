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
    using FamilyCart.API.Hubs;
    using Microsoft.AspNetCore.SignalR;

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ListItemController : ControllerBase
    {
        private readonly AppDbContext _appDBContext;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<FamilyHub, IFamilyClient> _hubContext;

        public ListItemController(AppDbContext appDbContext, UserManager<User> userManager, IHubContext<FamilyHub, IFamilyClient> hubContext)
        {
            _appDBContext = appDbContext;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateListItem(CreateListItemDto createListItemDto)
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

            var shoppingList = await _appDBContext.ShoppingLists.FindAsync(createListItemDto.ShoppingListId);

            if (shoppingList == null)
            {
                return NotFound("Shopping List not found");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == shoppingList.FamilyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            var product = await _appDBContext.Products.FindAsync(createListItemDto.ProductId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            if (product.StoreId != shoppingList.StoreId)
            {
                return BadRequest("This product does not belong to the same store as the shopping list.");
            }

            if(product.ArchivedAt != null)
            {
                return NotFound("Product not found");
            }

            var listItem = new ListItem
            {
                ShoppingList = shoppingList,
                Product = product,
                AddedByUser = user,
                ShoppingListId = createListItemDto.ShoppingListId,
                ProductId = createListItemDto.ProductId,
                Quantity = createListItemDto.Quantity,
                IsChecked = false,
                AddedById = userId
            };

            _appDBContext.ListItems.Add(listItem);
            await _appDBContext.SaveChangesAsync();

            var listItemDetailResponseDto = new ListItemDetailResponseDto
            {
                Id = listItem.Id,
                ProductId = listItem.ProductId,
                ShoppingListId = listItem.ShoppingListId,
                Quantity = listItem.Quantity,
                IsChecked = listItem.IsChecked,
                AddedById = listItem.AddedById,
                AddedAt = listItem.AddedAt,
                ProductName = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsArchived = false
            };

            await _hubContext.Clients.Group(FamilyHub.GroupName(shoppingList.FamilyId)).ListItemAdded(listItemDetailResponseDto);

            var listItemResponseDto = new ListItemResponseDto
            {
                Id = listItem.Id,
                ShoppingListId = listItem.ShoppingListId,
                ProductId = listItem.ProductId,
                Quantity = listItem.Quantity,
                IsChecked = listItem.IsChecked,
                AddedById = listItem.AddedById,
                AddedAt = listItem.AddedAt
            };

            return Ok(listItemResponseDto);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateListItem(int id, UpdateListItemDto updateListItemDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Id showing as null");
            }

            var listItem = await _appDBContext.ListItems.FindAsync(id);

            if (listItem == null)
            {
                return NotFound("List item not found");
            }

            var shoppingList = await _appDBContext.ShoppingLists.FindAsync(listItem.ShoppingListId);

            if (shoppingList == null)
            {
                return NotFound("Shopping list not found");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == shoppingList.FamilyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            var product = await _appDBContext.Products.FindAsync(listItem.ProductId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            if (updateListItemDto.Quantity != null)
            {
                listItem.Quantity = updateListItemDto.Quantity.Value;
            }

            if (updateListItemDto.IsChecked != null)
            {
                listItem.IsChecked = updateListItemDto.IsChecked.Value;
            }


            await _appDBContext.SaveChangesAsync();

            var listItemDetailResponseDto = new ListItemDetailResponseDto
            {
                Id = listItem.Id,
                ProductId = listItem.ProductId,
                ShoppingListId = listItem.ShoppingListId,
                Quantity = listItem.Quantity,
                IsChecked = listItem.IsChecked,
                AddedById = listItem.AddedById,
                AddedAt = listItem.AddedAt,
                ProductName = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsArchived = product.ArchivedAt != null
            };

            await _hubContext.Clients.Group(FamilyHub.GroupName(shoppingList.FamilyId)).ListItemUpdated(listItemDetailResponseDto);

            var listItemResponseDto = new ListItemResponseDto
            {
                Id = listItem.Id,
                ShoppingListId = listItem.ShoppingListId,
                ProductId = listItem.ProductId,
                Quantity = listItem.Quantity,
                IsChecked = listItem.IsChecked,
                AddedById = listItem.AddedById,
                AddedAt = listItem.AddedAt
            };

            return Ok(listItemResponseDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteListItem(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Id showing as null");
            }

            var listItem = await _appDBContext.ListItems.FindAsync(id);

            if (listItem == null)
            {
                return NotFound("List item not found");
            }

            var shoppingList = await _appDBContext.ShoppingLists.FindAsync(listItem.ShoppingListId);

            if (shoppingList == null)
            {
                return NotFound("Shopping list not found");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == shoppingList.FamilyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            _appDBContext.ListItems.Remove(listItem);
            await _appDBContext.SaveChangesAsync();
            await _hubContext.Clients.Group(FamilyHub.GroupName(shoppingList.FamilyId)).ListItemDeleted(listItem.Id, listItem.ShoppingListId);
            return NoContent();
        }
    }
}