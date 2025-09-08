using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class CourierRepository : ICourierRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CourierRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext; throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(Courier courier)
        {
            if (courier == null)
            {
                throw new ArgumentNullException("Courier is Null");
            }
            await _dbContext.Couriers.AddAsync(courier);
        }

        public async Task<List<Courier>> GetAllFreeAsync()
        {
            var freeCouriers = await _dbContext.Couriers.Where(x => x.StoragePlaces.All(sp => sp.OrderId == null))
                                                        .ToListAsync();
            if (freeCouriers == null)
            {
                throw new ArgumentNullException("FreeCouriers Not Found");
            }
            return freeCouriers;
        }

        public async Task<Courier> GetByIdAsync(Guid courierId)
        {
            return await _dbContext.Couriers.FindAsync(courierId)
                         ?? throw new ArgumentNullException($"Courier c Id {courierId} Not Found"); ;
        }

        public void Update(Courier courier)
        {
            if (courier == null)
            {
                throw new ArgumentNullException("Courier is Null");
            }
            _dbContext.Couriers.Update(courier);
        }
    }
}