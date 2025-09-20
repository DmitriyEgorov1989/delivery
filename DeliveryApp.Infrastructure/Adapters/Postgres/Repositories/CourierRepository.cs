using CSharpFunctionalExtensions;
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
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(Courier courier)
        {
            if (courier == null)
            {
                throw new ArgumentNullException("Courier is Null");
            }
            await _dbContext.Couriers.AddAsync(courier);
        }

        public IEnumerable<Courier> GetAllFree()
        {
            return _dbContext.Couriers.Include(c => c.StoragePlaces)
                                      .Where(x => x.StoragePlaces.All(sp => sp.OrderId == null))
                                      .AsNoTracking();
        }

        public async Task<Maybe<Courier>> GetByIdAsync(Guid courierId)
        {
            var courier = await _dbContext.Couriers.Where(x => x.Id == courierId)
                                                   .Include(c => c.StoragePlaces)
                                                   .FirstOrDefaultAsync();
            return Maybe.From(courier);
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