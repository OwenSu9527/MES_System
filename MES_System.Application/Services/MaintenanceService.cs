using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES_System.Application.DTOs;
using MES_System.Application.Interfaces;
using MES_System.Domain.Entities;
using MES_System.Domain.Enums;

namespace MES_System.Application.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _maintenanceRepo;
        private readonly IEquipmentRepository _equipmentRepo;

        // 建構子注入
        public MaintenanceService(IMaintenanceRepository maintenanceRepo, IEquipmentRepository equipmentRepo)
        {
            _maintenanceRepo = maintenanceRepo;
            _equipmentRepo = equipmentRepo;
        }

        public async Task CreateMaintenanceRequestAsync(CreateMaintenanceDto dto)
        {
            // 1. 找出機台
            var equipment = await _equipmentRepo.GetByIdAsync(dto.EquipmentId);
            if (equipment == null) throw new Exception("機台不存在");

            // 2. 建立維修單 (Entity)
            var request = new MaintenanceRequest
            {
                EquipmentId = dto.EquipmentId,
                RequestUser = dto.RequestUser,
                ReasonCode = dto.ReasonCode,
                Description = dto.Description,
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            // 3. [關鍵邏輯] 變更機台狀態為 Down
            equipment.Status = EquipmentStatus.Down;
            equipment.LastUpdated = DateTime.Now;

            // 4. 存檔 (交易一致性)
            // 理想情況應包在 Transaction，這裡分開存
            await _maintenanceRepo.AddAsync(request);
            await _equipmentRepo.UpdateAsync(equipment);
        }
    }
}