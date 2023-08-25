using Restaurant.Api.Models;

namespace Restaurant.Api.Domain
{
    public class RestManager
    {
        static readonly SemaphoreSlim Semaphore = new(1, 1);

        private readonly Dictionary<Table, List<ClientsGroup>> _tables = new();

        private readonly List<ClientsGroup> _waitingGroups = new();

        public void Arrive(int clientsGroupSize)
        {
            Semaphore.Wait();

            try
            {
                var clientsGroup = new ClientsGroup(Guid.NewGuid(), clientsGroupSize);
                var table = FindTableForClientsGroup(clientsGroup);

                if (table is null)
                {
                    _waitingGroups.Add(clientsGroup);
                }
                else
                {
                    _tables[table].Add(clientsGroup);
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public void Leave(ClientsGroup clientsGroup)
        {
            Semaphore.Wait();

            try
            {
                if (_waitingGroups.Contains(clientsGroup))
                {
                    _waitingGroups.Remove(clientsGroup);
                    return;
                }

                var tableWithClientsGroup = FindTableWithClientsGroup(clientsGroup.Id);
                if (tableWithClientsGroup is not null)
                {
                    _tables[tableWithClientsGroup].Remove(clientsGroup);
                    TryToFindTableForWaitingClientsGroups();
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public bool HasNoTableForClientsGroupSize(int size)
            => !_tables.Any(x => x.Key.Size >= size);

        public Table? Lookup(Guid id)
            => FindTableWithClientsGroup(id);

        public RestaurantState GetRestaurantState()
            => new(_waitingGroups, _tables.Select(x =>
                new TableState(
                    x.Key.Id,
                    x.Key.Size,
                    x.Key.Size - x.Value.Sum(g => g.Size),
                    x.Value.Count == 0,
                    x.Value.Sum(x => x.Size) == x.Key.Size,
                    x.Value
                    ))
                );

        public void InitTables(IEnumerable<int> tableSizes)
        {
            foreach (var size in tableSizes)
            {
                _tables.Add(new Table(Guid.NewGuid(), size), new List<ClientsGroup>());
            }
        }

        private Table? FindTableForClientsGroup(ClientsGroup clientsGroup)
            => _tables
                .Select(x => new
                {
                    Table = x.Key,
                    IsEmpty = x.Value.Count == 0,
                    FreeSeets = x.Key.Size - x.Value.Sum(g => g.Size),
                })
                .Where(x => x.FreeSeets >= clientsGroup.Size)
                .OrderByDescending(x => x.IsEmpty)
                .ThenBy(x => x.FreeSeets)
                .Select(x => x.Table)
                .FirstOrDefault();

        private Table? FindTableWithClientsGroup(Guid id)
            => _tables.Where(x => x.Value.Any(g => g.Id == id)).Select(x => x.Key).FirstOrDefault();

        private void TryToFindTableForWaitingClientsGroups()
        {
            if (_waitingGroups.Count == 0)
            {
                return;
            }

            var setletClientsGroups = new List<ClientsGroup>();

            _waitingGroups.ForEach(clientsGroup =>
            {
                var table = FindTableForClientsGroup(clientsGroup);
                if (table is not null)
                {
                    _tables[table].Add(clientsGroup);
                    setletClientsGroups.Add(clientsGroup);
                }
            });

            if (setletClientsGroups.Count > 0)
            {
                _waitingGroups.RemoveAll(setletClientsGroups.Contains);
            }
        }
    }
}

