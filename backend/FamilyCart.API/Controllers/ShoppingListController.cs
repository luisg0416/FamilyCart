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
    using System.Runtime.Intrinsics.X86;

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

            return Ok();
        }
    }
}