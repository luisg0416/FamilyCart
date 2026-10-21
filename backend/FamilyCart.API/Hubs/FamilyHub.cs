namespace FamilyCart.API.Hubs
{

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using FamilyCart.Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;

    [Authorize]
    public class FamilyHub : Hub<IFamilyClient>
    {
        private readonly AppDbContext _appDBContext;

         public FamilyHub(AppDbContext appDbContext)
        {
            _appDBContext = appDbContext;
        }

        public static string GroupName(int familyId)
        {
            return $"family-{familyId}";
        }

        public async Task JoinFamily(int familyId)
        {
            var userIdString = Context.UserIdentifier;

            if (!int.TryParse(userIdString, out int userId))
            {
                throw new HubException("Unauthorized");
            }

            bool isMember = await _appDBContext.FamilyMembers.AnyAsync(fm => fm.FamilyId == familyId && fm.UserId == userId);

            if (!isMember)
            {
                throw new HubException("Not a member of this family");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(familyId));

        }

    }
}