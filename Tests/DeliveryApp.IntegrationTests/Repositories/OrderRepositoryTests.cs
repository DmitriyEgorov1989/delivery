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
    public class OrderRepositoryTests : IAsyncLifetime
    {
        private ApplicationDbContext _dbContext;

        private OrderRepository _orderRepository;

        private UnitOfWork _unitOfWork;

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

        /// <summary>
        ///     Уничтожаем окружение
        /// </summary>
        /// <remarks>Вызывается после каждого теста</remarks>
        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync().AsTask();
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
                .Options;

            _dbContext = new ApplicationDbContext(options);

            await _dbContext.Database.MigrateAsync();

            _orderRepository = new OrderRepository(_dbContext);
            _unitOfWork = new UnitOfWork(_dbContext);
        }

        [Fact]
        public async Task CanAddOrder()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = Order.Create(orderId, Location.CreateRandom(), 2);
            order.IsSuccess.Should().BeTrue();

            //Act
            await _orderRepository.AddAsync(order.Value);
            await _unitOfWork.SaveChangesAsync();

            //Assert
            var orderFromDb = await _orderRepository.GetByIdAsync(orderId);
            orderFromDb.Should().NotBeNull();
            order.Value.Should().BeEquivalentTo(orderFromDb.Value);
        }

        [Fact]
        public async Task CanUpdateOrder()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = Order.Create(orderId, Location.CreateRandom(), 2);
            order.IsSuccess.Should().BeTrue();

            await _orderRepository.AddAsync(order.Value);
            await _unitOfWork.SaveChangesAsync();

            //Act
            var orderExchange = order.Value.Assign(orderId);
            orderExchange.IsSuccess.Should().BeTrue();

            _orderRepository.Update(order.Value);
            await _unitOfWork.SaveChangesAsync();

            //Aseert
            var orderFromDb = await _orderRepository.GetByIdAsync(orderId);
            orderFromDb.HasValue.Should().BeTrue();
            orderFromDb.Value.Status.Should().Be(OrderStatus.Assigned);
        }

        [Fact]
        public async Task CanGetAllAssignedOrders()
        {
            //Arrange
            var orderIdAssign = Guid.NewGuid();
            var orderAssign = Order.Create(orderIdAssign, Location.CreateRandom(), 2);
            var orderExAssign = orderAssign.Value.Assign(orderIdAssign);
            orderExAssign.IsSuccess.Should().BeTrue();

            var orderIdNoAssign = Guid.NewGuid();
            var orderNoAssign = Order.Create(orderIdNoAssign, Location.CreateRandom(), 2);
            orderNoAssign.IsSuccess.Should().BeTrue();

            await _orderRepository.AddAsync(orderAssign.Value);
            await _orderRepository.AddAsync(orderNoAssign.Value);
            await _unitOfWork.SaveChangesAsync();

            //Act
            var resultFromDb = await _orderRepository.GetAllAssigned()
                                                     .ToListAsync();

            //Aseert
            resultFromDb.Count.Should().Be(1);
            resultFromDb[0].Status.Should().Be(OrderStatus.Assigned);
        }

        [Fact]
        public async Task CanGetRandomCreatedOrders()
        {
            //Arrange
            var orderIdAssign = Guid.NewGuid();
            var orderAssign = Order.Create(orderIdAssign, Location.CreateRandom(), 2);
            var orderExAssign = orderAssign.Value.Assign(orderIdAssign);
            orderExAssign.IsSuccess.Should().BeTrue();

            var orderIdCreated = Guid.NewGuid();
            var orderCreated = Order.Create(orderIdCreated, Location.CreateRandom(), 2);
            orderCreated.IsSuccess.Should().BeTrue();

            await _orderRepository.AddAsync(orderAssign.Value);
            await _orderRepository.AddAsync(orderCreated.Value);
            await _unitOfWork.SaveChangesAsync();

            //Act
            var resultFromDb = await _orderRepository.GetCreated();

            //Aseert
            resultFromDb.HasValue.Should().BeTrue();
            resultFromDb.Value.Status.Should().BeEquivalentTo(OrderStatus.Created);
        }
    }
}