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
        // [Day 12 新增] 實作更新邏輯
        public async Task UpdateAsync(Equipment equipment)
        {
            // 步驟 1: 告訴 EF Core 這個物件被修改了
            // 注意：Update 方法本身不是非同步的，所以不需要 await
            _context.Equipments.Update(equipment);

            // 步驟 2: 真正的「存檔」動作
            // 這一步才是真正連線到資料庫執行 SQL UPDATE 指令的地方，所以需要 await
            await _context.SaveChangesAsync();
        }
    }
}
