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

    public class StoreController : ControllerBase
    {

        private readonly AppDbContext _appDBContext;
        private readonly UserManager<User> _userManager;

        public StoreController(AppDbContext appDbContext, UserManager<User> userManager)
        {
            _appDBContext = appDbContext;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStoreDto createStoreDto)
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

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == createStoreDto.FamilyId && fm.UserId == userId);

            if (!isMember)
            {
                return StatusCode(403, "User is not a member of this Family");
            }

            var family = await _appDBContext.Families.FindAsync(createStoreDto.FamilyId);

            if (family == null)
            {
                return NotFound("Family not found");
            }

            var store = new Store
            {
                Name = createStoreDto.Name,
                Family = family,
                FamilyId = createStoreDto.FamilyId,
                InstacartStoreId = createStoreDto.InstacartStoreId
            };

            _appDBContext.Stores.Add(store);
            await _appDBContext.SaveChangesAsync();

            var storeResponseDto = new StoreResponseDto
            {
                Id = store.Id,
                Name = store.Name,
                FamilyId = store.FamilyId,
                InstacartStoreId = store.InstacartStoreId
            };

            return Ok(storeResponseDto);
        }

        [HttpGet("{familyId}")]
        public async Task<IActionResult> GetStoresForFamily(int familyId)
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

            var stores = await _appDBContext.Stores.Where(s => s.FamilyId == null || s.FamilyId == familyId).ToListAsync();

            var storeDtos = stores.Select(s => new StoreResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    InstacartStoreId = s.InstacartStoreId,
                    FamilyId = s.FamilyId
                }).ToList();

            return Ok(storeDtos);
        }
    }
}