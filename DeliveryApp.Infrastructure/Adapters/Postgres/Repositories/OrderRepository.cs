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
            _dbContext = dbContext; throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(Order order)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            await _dbContext.Orders.AddAsync(order);
        }

        public IQueryable<Order> GetAllAssignedAsync()
        {
            return _dbContext.Orders.Where(o => o.Status == OrderStatus.Assigned);
        }

        public async Task<Maybe<Order>> GetByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(orderId));
            }
            var order = await _dbContext.Orders.FindAsync(orderId);

            return order ?? Maybe<Order>.None;
        }

        public async Task<Maybe<Order>> GetCreated()
        {
            var order = await _dbContext.Orders.FirstAsync(o => o.Status == OrderStatus.Created);

            return order ?? Maybe<Order>.None;
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