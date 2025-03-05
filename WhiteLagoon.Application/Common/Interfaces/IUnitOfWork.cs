using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IVillaRepository villa { get; }
        IVillaNumberRepository villaNumber { get; }
        IAmenityRepository amenity { get; }
        IApplicationUserRepository user { get; }
        IBookingRepository booking { get; }

        void Save();
    }
}
