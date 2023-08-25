namespace Restaurant.Api.Models
{
    public record TableState(Guid Id, int Size, int FreeSeets, bool IsEmpty, bool IsFull, IEnumerable<ClientsGroup> Groups);

    public record RestaurantState(IEnumerable<ClientsGroup> WaitingGroups, IEnumerable<TableState> Tables);
}
