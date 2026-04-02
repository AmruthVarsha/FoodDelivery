using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogService.Infrastructure.Repositories
{
    public class ServiceAreaRepository : IServiceAreaRepository
    {
        private readonly CatalogDbContext _context;
        public ServiceAreaRepository(CatalogDbContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<ServiceArea>> GetByRestaurantIdAsync(Guid restaurantId)
        {
            return await _context.ServiceAreas.Where(sa => sa.RestaurantId == restaurantId).ToListAsync();
        }

        public async Task<ServiceArea?> GetByIdAsync(Guid id)
        {
            return await _context.ServiceAreas.FindAsync(id);
        }
        public async Task<bool> IsPincodeServiceableAsync(Guid restaurantId, string pincode)
        {
            return await _context.ServiceAreas.AnyAsync(sa => sa.RestaurantId == restaurantId && sa.Pincode == pincode);
        }
        public async Task<Guid> CreateAsync(ServiceArea serviceArea)
        {
            _context.ServiceAreas.Add(serviceArea);
            await _context.SaveChangesAsync();
            return serviceArea.Id;
        }
        public async Task DeleteAsync(Guid id)
        {
            var serviceArea = await _context.ServiceAreas.FindAsync(id);
            if (serviceArea is null) return;
            _context.ServiceAreas.Remove(serviceArea);
            await _context.SaveChangesAsync();
        }

    }
}
