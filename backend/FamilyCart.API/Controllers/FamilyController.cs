namespace FamilyCart.API.Controllers
{
    using FamilyCart.Core.DTOs;
    using FamilyCart.Core.Interfaces;
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

    public class FamilyController: ControllerBase
    {
        private readonly AppDbContext _appDBContext;
        private readonly UserManager<User> _userManager;

        public FamilyController(AppDbContext appDbContext, UserManager<User> userManager)
        {
            _appDBContext = appDbContext;
            _userManager = userManager;
        }

        private async Task<string> GenerateUniqueInviteCodeAsync()
        {
            string code;
            
            const string chars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";

            do 
            {   
                int length = 8;
                
                char[] result = new char[length];
                
                for (int i = 0; i < length; i++)
                {
                    result[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
                    
                }
                code = new string(result);
            }
            while (await _appDBContext.Families.AnyAsync(f => f.InviteCode == code));

            return code;
        }

        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFamilyDto createFamilyDto)
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

            string inviteCode = await GenerateUniqueInviteCodeAsync();

            var family = new Family
            {
                CreatedByUser = user,
                FamilyName = createFamilyDto.FamilyName,
                InviteCode = inviteCode,
                CreatedById = userId
            };

            var familyMember = new FamilyMember
            { 
                Family = family, 
                User = user, 
                FamilyId = family.Id, 
                UserId = user.Id 
            };

            _appDBContext.Families.Add(family);
            _appDBContext.FamilyMembers.Add(familyMember);

            await _appDBContext.SaveChangesAsync();

            var familyResponseDto = new FamilyResponseDto
            {
                Id = family.Id,
                FamilyName = family.FamilyName,
                InviteCode = family.InviteCode,
                CreatedAt = family.CreatedAt
            };

            return Ok(familyResponseDto);
        }

        [HttpPost("join")]
        public async Task<IActionResult> Join(JoinFamilyDto joinFamilyDto)
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

            var family = await _appDBContext.Families.FirstOrDefaultAsync(f => f.InviteCode == joinFamilyDto.InviteCode);

            if (family == null)
            {
                return NotFound("Family not found");
            }

            bool alreadyMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == family.Id && fm.UserId == userId);

            if (alreadyMember)
            {
                return BadRequest("You are already a member of this family.");
            }

            var familyMember = new FamilyMember
            { 
                Family = family, 
                User = user, 
                FamilyId = family.Id, 
                UserId = user.Id 
            };

            _appDBContext.FamilyMembers.Add(familyMember);
            await _appDBContext.SaveChangesAsync();

            var familyResponseDto = new FamilyResponseDto
            {
                Id = family.Id,
                FamilyName = family.FamilyName,
                InviteCode = family.InviteCode,
                CreatedAt = family.CreatedAt
            };

            return Ok(familyResponseDto);
        }
    }
}