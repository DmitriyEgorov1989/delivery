using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

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
                .Options;

            _dbContext = new ApplicationDbContext(options);

            await _dbContext.Database.MigrateAsync();
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

            //Act
            var courierRepository = new CourierRepository(_dbContext);
            var unitOfWork = new UnitOfWork(_dbContext);
            await courierRepository.AddAsync(courier);
            await unitOfWork.SaveChangesAsync();

            //Assert
            var getCourierResult = await courierRepository.GetByIdAsync(courier.Id);
            getCourierResult.HasValue.Should().BeTrue();
            var courierFromDb = getCourierResult.Value;
            courier.Should().BeEquivalentTo(courierFromDb);
        }

        [Fact]
        public async Task CanGetAllFreeCourier()
        {
            //Arrange
            var location1 = Location.CreateRandom();
            var location2 = Location.CreateRandom();
            var courier = Courier.Create("Занятый_курьер", 1, location2);
            courier.IsSuccess.Should().BeTrue();

            var order = Order.Create(Guid.NewGuid(), Location.CreateRandom(), 3);
            order.IsSuccess.Should().BeTrue();
            var orderRepository = new OrderRepository(_dbContext);
            await orderRepository.AddAsync(order.Value);

            courier.Value.TakeOrder(order.Value);

            var courierFree = Courier.Create("Свободный_курьер", 1, location1);
            courier.IsSuccess.Should().BeTrue();

            //Act
            var courierRepository = new CourierRepository(_dbContext);
            var unitOfWork = new UnitOfWork(_dbContext); ;
            await courierRepository.AddAsync(courier.Value);
            await courierRepository.AddAsync(courierFree.Value);

            await unitOfWork.SaveChangesAsync();
            var couriersFromDb = await courierRepository.GetAllFree()
                                                .ToListAsync();

            //Assert);
            couriersFromDb.Count.Should().Be(1);
            couriersFromDb[0].Name.Should().Be("Свободный_курьер");
        }

        [Fact]
        public async Task CanUpdateCourier()
        {
            //Arrange
            var location = Location.CreateRandom();
            var courier = Courier.Create("тест", 1, location);
            courier.IsSuccess.Should().BeTrue();

            var courierRepository = new CourierRepository(_dbContext);
            var unitOfWork = new UnitOfWork(_dbContext);
            await courierRepository.AddAsync(courier.Value);
            await unitOfWork.SaveChangesAsync();

            //Act
            courier.Value.AddStoragePlace("Тестовый_апдейт", 5);
            courierRepository.Update(courier.Value);
            await unitOfWork.SaveChangesAsync();

            //Assert
            var courierFromDb = await courierRepository.GetByIdAsync(courier.Value.Id);
            courierFromDb.HasValue.Should().BeTrue();
            courierFromDb.Value.StoragePlaces.Count.Should().Be(2);
            courierFromDb.Value.StoragePlaces[1].Name.Should().Be("Тестовый_апдейт");
        }
    }
}