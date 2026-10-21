namespace FamilyCart.API.Hubs
{
    using FamilyCart.Core.DTOs;
    public interface IFamilyClient
    {
        Task ListItemAdded(ListItemDetailResponseDto listItemDetailResponseDto);
        Task ListItemUpdated(ListItemDetailResponseDto listItemDetailResponseDto);
        Task ListItemDeleted(int itemId, int shoppingListId);
    }
}