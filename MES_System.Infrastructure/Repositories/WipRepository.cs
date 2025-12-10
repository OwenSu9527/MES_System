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
    /// <summary>
    /// 在製品庫存 (WIP) 資料存取實作
    /// </summary>
    public class WipRepository : IWipRepository
    {
        private readonly MesDbContext _context;

        public WipRepository(MesDbContext context)
        {
            _context = context;
        }

        public async Task<WipSnapshot?> GetByWorkOrderAndStationAsync(int workOrderId, int stationId)
        {
            // 使用 FirstOrDefaultAsync 找第一筆符合的資料，找不到回傳 null
            return await _context.WipSnapshots
                .FirstOrDefaultAsync(w => w.WorkOrderId == workOrderId && w.StationId == stationId);
        }

        // [Day10] 取得所有在製品快照資料
        public async Task<IEnumerable<WipSnapshot>> GetAllAsync()
        {
            return await _context.WipSnapshots.ToListAsync();
        }
        public async Task AddAsync(WipSnapshot wip)
        {
            await _context.WipSnapshots.AddAsync(wip);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WipSnapshot wip)
        {
            _context.WipSnapshots.Update(wip);
            await _context.SaveChangesAsync();
        }
    }
}