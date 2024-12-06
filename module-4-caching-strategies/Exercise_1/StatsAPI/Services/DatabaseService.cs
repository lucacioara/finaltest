using Microsoft.Azure.Cosmos;
using StatsAPI.Models;
using Database = Microsoft.Azure.Cosmos.Database;

namespace StatsAPI.Services
{
    public class DatabaseService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IConfiguration _configuration;
        private Container _dataContainer;
        private Container _statsContainer;

        public DatabaseService(string endpointUri, IConfiguration configuration)
        {
            _cosmosClient = new CosmosClient(endpointUri, new CosmosClientOptions { ConnectionMode = ConnectionMode.Gateway, ConsistencyLevel = ConsistencyLevel.Eventual });
            _configuration = configuration;
            Task.Run(async () => await InitializeContainers());
        }
        public async Task InitializeContainers()
        {
            Database database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(id: "rockpaperscissors");
            _dataContainer = await database.CreateContainerIfNotExistsAsync(id: "data", partitionKeyPath: "/winner", throughput: 400);
            var TTL = _configuration.GetValue<int>("TTL");
            var ContainerProperties = new ContainerProperties
            {
                Id = "stats",
                PartitionKeyPath = "/period",
                DefaultTimeToLive = TTL
            };
            _statsContainer = await database.CreateContainerIfNotExistsAsync(ContainerProperties, throughput: 400);
        }
        public async Task<ItemResponse<Stats>> AddStats(Stats stats)
        {
            return await _dataContainer.CreateItemAsync(stats, new PartitionKey(stats.WinnerEmail));
        }
        public async Task<List<Stats>> GetStats(Period period)
        {
            var startDate = GetStartDate(period);

            var endDate = DateTime.UtcNow;

            var query = $"SELECT * FROM c WHERE c.Timestamp >= '{startDate:o}' AND c.Timestamp <= '{endDate:o}'";
            var queryDefinition = new QueryDefinition(query);

            var records = new List<Stats>();
            var iterator = _dataContainer.GetItemQueryIterator<Stats>(queryDefinition);
            {
                var feedResponse = await iterator.ReadNextAsync();
                foreach (var record in feedResponse)
                {
                    records.Add(record);
                }
            }
            return records;
        }
        public async Task<StatsResult> CalculateStats(Period period)
        {
            var result = new StatsResult();
            result.Id = period.ToString();
            result.Period = period.ToString();
            var records = await GetStats(period);
            var topPlayers = records
    .GroupBy(x => x.WinnerEmail)
    .Select(x => new WinnerResult { WinnerEmail = x.Key, Wins = x.Count() })
    .OrderByDescending(x => x.Wins)
    .Take(10)
    .ToList();
            int totalRockCount = records.Sum(r => r.RockCount);
            int totalPaperCount = records.Sum(r => r.PaperCount);
            int totalScissorsCount = records.Sum(r => r.ScissorsCount);

            var moveCounts = new List<MoveResult>
            {
                new MoveResult{ Move = "Rock", Count = totalRockCount },
                new MoveResult{ Move = "Paper", Count = totalPaperCount },
                new MoveResult { Move = "Scissors", Count = totalScissorsCount }
            };
            result.Moves = moveCounts;
            result.Winners = topPlayers;
            await WriteResult(result);
            return result;

        }
        public async Task<StatsResult> ReadResult(Period period)
        {
            try
            {
                StatsResult response = await _statsContainer.ReadItemAsync<StatsResult>(
                period.ToString(),
                    new Microsoft.Azure.Cosmos.PartitionKey(period.ToString())
                );
                return response;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return await CalculateStats(period);
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        public async Task WriteResult(StatsResult result)
        {
            await _statsContainer.CreateItemAsync(result, new PartitionKey(result.Period));
        }
        private DateTime GetStartDate(Period period)
        {
            return period switch
            {
                Period.Daily => DateTime.UtcNow.AddDays(-1),
                Period.Weekly => DateTime.UtcNow.AddDays(-7),
                Period.Monthly => DateTime.UtcNow.AddMonths(-1),
                _ => DateTime.UtcNow.AddDays(-1)
            };
        }

    }
}
