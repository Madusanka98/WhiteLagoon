using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly ApplicationDbContext _context;
        public IVillaRepository villa { get; private set; }
        public IVillaNumberRepository villaNumber { get; private set; }
        public IApplicationUserRepository user { get; private set; }

        public IAmenityRepository amenity { get; private set; }
        public IBookingRepository booking { get; private set; }

        public UnitOfWork (ApplicationDbContext context)
        {
            _context = context;
            villa = new VillaRepository(_context);
            villaNumber = new VillaNumberRepository(_context);
            user = new ApplicationUserRepository(_context);
            amenity = new AmenityRepository(_context);
            booking = new BookingRepository(_context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
