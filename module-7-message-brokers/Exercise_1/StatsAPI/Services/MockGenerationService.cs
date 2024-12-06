using Bogus;
using StatsAPI.Models;

namespace StatsAPI.Services
{
    public interface IMockGenerationService
    {
        IReadOnlyCollection<Stats> CreateMockData(int count);
    }

    public class MockGenerationService : IMockGenerationService
    {
        private List<string> emailAdresses = new List<string>();
        public IReadOnlyCollection<Stats> CreateMockData(int count)
        {
            if (emailAdresses.Count == 0)
                GenerateEmailAddresses(count / 10);
            var time = DateTime.UtcNow;
            return new Bogus.Faker<Stats>()
                .RuleFor(x => x.Id, f => Guid.NewGuid().ToString())
                .RuleFor(x => x.WinnerEmail, f => f.PickRandom(emailAdresses))
                .RuleFor(x => x.RockCount, f => f.Random.Number(0, 8))
                .RuleFor(x => x.PaperCount, f => f.Random.Number(0, 8))
                .RuleFor(x => x.ScissorsCount, f => f.Random.Number(0, 8))
                .RuleFor(x => x.Timestamp, f => f.Date.Between(time.AddDays(-40), time))
                .Generate(count);
        }
        private void GenerateEmailAddresses(int count)
        {
            var faker = new Faker();
            while (emailAdresses.Count <= count)
            {
                emailAdresses.Add(faker.Internet.ExampleEmail());
            }
        }
    }
}
