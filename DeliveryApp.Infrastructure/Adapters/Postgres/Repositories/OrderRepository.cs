using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(Order order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            await _dbContext.Orders.AddAsync(order);
        }

        public IEnumerable<Order> GetAllAssigned()
        {
            return _dbContext.Orders.Where(o => o.Status.Name == OrderStatus.Assigned.Name);
        }

        public async Task<Maybe<Order>> GetByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(orderId));
            }
            var order = await _dbContext.Orders.FindAsync(orderId);

            return Maybe.From(order);
        }

        public async Task<Maybe<Order>> GetCreatedAsync()
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Status.Name == OrderStatus.Created.Name);

            return Maybe.From(order);
        }

        public void Update(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            _dbContext.Orders.Update(order);
        }
    }
}