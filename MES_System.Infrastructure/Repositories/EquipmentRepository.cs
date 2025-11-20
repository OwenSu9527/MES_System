using MES_System.Application.Interfaces;
using MES_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Infrastructure.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly MesDbContext _context;

        // 透過建構子注入 DbContext
        public EquipmentRepository(MesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Equipment>> GetAllAsync()
        {
            // 使用 EF Core 的 ToListAsync 非同步取得資料
            return await _context.Equipments.ToListAsync();
        }

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            return await _context.Equipments.FindAsync(id);
        }
    }
}
