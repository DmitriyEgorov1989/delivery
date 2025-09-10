using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;
using Xunit.Abstractions;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class CourierRepositoryTests : IAsyncLifetime
    {
        /// <summary>
        ///     Настройка Postgres из библиотеки TestContainers
        /// </summary>
        /// <remarks>По сути это Docker контейнер с Postgres</remarks>

        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("testDb")
            .WithUsername("username")
            .WithPassword("password")
            .WithCleanUp(true)
            .Build();

        private ApplicationDbContext _dbContext;

        private readonly ITestOutputHelper _output;

        public CourierRepositoryTests(ITestOutputHelper output)
        {

            _output = output;
        }
        /// <summary>
        ///     Инициализируем окружение
        /// </summary>
        /// <remarks>Вызывается перед каждым тестом</remarks>
        public async Task InitializeAsync()
        {

            await _postgreSqlContainer.StartAsync();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString(),
                    o => o.MigrationsAssembly("DeliveryApp.Infrastructure"))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .Options;

            _dbContext = new ApplicationDbContext(options);
            var pending = await _dbContext.Database.GetPendingMigrationsAsync();
            var applied = await _dbContext.Database.GetAppliedMigrationsAsync();
            _output.WriteLine("Pending: " + string.Join(", ", pending));
            _output.WriteLine("Applied: " + string.Join(", ", applied));
            _dbContext.Database.Migrate();
        }

        /// <summary>
        ///     Уничтожаем окружение
        /// </summary>
        /// <remarks>Вызывается после каждого теста</remarks>
        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public async Task CanAddCourier()
        {
            //Arrange
            var location = Location.CreateRandom();
            var courier = Courier.Create("тест", 1, location).Value;


            var _repository = new CourierRepository(_dbContext);
            var _unitOfWork = new UnitOfWork(_dbContext);
            await _repository.AddAsync(courier);


            //Assert
            var getCourierResult = await _repository.GetByIdAsync(courier.Id);
            getCourierResult.HasValue.Should().BeTrue();
            courier.Should().BeEquivalentTo(getCourierResult.Value);


            //Assert

        }
    }
}
