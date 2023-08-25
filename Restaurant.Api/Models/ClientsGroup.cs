namespace Restaurant.Api.Models
{
    public record ClientsGroupArriveRequest(int Size);

    public record ClientsGroup(Guid Id, int Size);
}
