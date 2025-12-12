using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES_System.Application.Interfaces;
using MES_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES_System.Infrastructure.Repositories
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly MesDbContext _context;

        public MaintenanceRepository(MesDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MaintenanceRequest request)
        {
            await _context.MaintenanceRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }
    }
}