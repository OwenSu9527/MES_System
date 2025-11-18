using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MES_System.Domain.Entities;
using MES_System.Domain.Enums;

namespace MES_System.Infrastructure
{
    public class MesDbContext : DbContext
    {
        // 建構子：接收設定選項 (如連線字串) 傳給基底類別
        public MesDbContext(DbContextOptions<MesDbContext> options) : base(options)
        {
        }

        // 定義資料表：這行代表資料庫裡會有一張 Equipment 表
        public DbSet<Equipment> Equipments { get; set; }

        // 種子資料 (Seed Data)：程式啟動時預先塞入的假資料
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Equipment>().HasData(
                new Equipment { Id = 1, Name = "SMT-01", Location = "Line-A", Status = EquipmentStatus.Running, LastUpdated = DateTime.Now },
                new Equipment { Id = 2, Name = "AOI-01", Location = "Line-A", Status = EquipmentStatus.Idle, LastUpdated = DateTime.Now },
                new Equipment { Id = 3, Name = "Reflow-01", Location = "Line-A", Status = EquipmentStatus.Down, LastUpdated = DateTime.Now }
            );
        }
    }
}