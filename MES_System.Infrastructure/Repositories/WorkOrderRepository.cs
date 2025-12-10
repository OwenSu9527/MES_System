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
    // 實作 IWorkOrderRepository 介面
    public class WorkOrderRepository : IWorkOrderRepository
    {
        private readonly MesDbContext _context;

        // 注入 DbContext，這是 EF Core 用來操作資料庫的核心物件
        public WorkOrderRepository(MesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkOrder>> GetAllAsync()
        {
            // 相當於 SQL: SELECT * FROM WorkOrders
            return await _context.WorkOrders.ToListAsync();
        }

        public async Task<WorkOrder?> GetByIdAsync(int id)
        {
            // 相當於 SQL: SELECT TOP 1 * FROM WorkOrders WHERE Id = @id
            return await _context.WorkOrders.FindAsync(id);
        }

        public async Task<WorkOrder> AddAsync(WorkOrder workOrder)
        {
            // 1. 將實體加入到 EF Core 的追蹤清單 (此時還沒寫入 DB)
            await _context.WorkOrders.AddAsync(workOrder);
            // 2. 真正執行 SQL INSERT 指令 (相當於 Commit)
            await _context.SaveChangesAsync();
            // 3. 回傳包含新產生 ID 的物件
            return workOrder;
        }

        public async Task UpdateAsync(WorkOrder workOrder)
        {
            // 1. 標記該物件為 "已修改" 狀態
            _context.WorkOrders.Update(workOrder);
            // 2. 執行 SQL UPDATE 指令
            await _context.SaveChangesAsync();
        }

        // [Day 9 新增] 依工單號查詢 (為了效能)
        public async Task<WorkOrder?> GetByOrderNoAsync(string orderNo)
        {
            // 使用 EF Core 的 FirstOrDefaultAsync
            // 這會轉譯成 SQL: SELECT TOP 1 * FROM WorkOrders WHERE OrderNo = @orderNo
            return await _context.WorkOrders.FirstOrDefaultAsync(w => w.OrderNo == orderNo);
        }
    }
}