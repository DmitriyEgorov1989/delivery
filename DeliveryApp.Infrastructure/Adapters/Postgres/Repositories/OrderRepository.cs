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

        public async Task<List<Order>> GetAllAssignedAsync()
        {
            var assignedOrders = await _dbContext.Orders.Where(o => o.Status == OrderStatus.Assigned)
                                                        .ToListAsync();

            return assignedOrders ?? throw new ArgumentNullException(nameof(assignedOrders));
        }

        public async Task<Order> GetByIdAsync(Guid orderId)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);

            return order ?? throw new ArgumentNullException(nameof(order));
        }

        public async Task<Order> GetCreated()
        {
            var order = await _dbContext.Orders.FirstAsync(o => o.Status == OrderStatus.Created);

            return order ?? throw new ArgumentNullException(nameof(order));
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