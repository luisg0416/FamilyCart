namespace FamilyCart.API.Controllers
{
    using FamilyCart.Core.DTOs;
    using FamilyCart.Core.Interfaces;
    using FamilyCart.Core.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [ApiController]
    [Route("api/[whoami]")]

    public class FamilyController: ControllerBase
    {
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            
        }
    }
}