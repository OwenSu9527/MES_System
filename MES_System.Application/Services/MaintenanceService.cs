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
        // [Day 14] 開始維修
        public async Task StartMaintenanceAsync(int requestId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(requestId);
            if (request == null) throw new Exception("維修單不存在");

            // 狀態流轉: Open -> InProgress
            request.Status = "InProgress";
            request.StartTime = DateTime.Now;

            // 同步更新機台狀態: Down -> Repair (選用，有些工廠這兩者分開，這裡假設連動)
            var equipment = await _equipmentRepo.GetByIdAsync(request.EquipmentId);
            if (equipment != null)
            {
                equipment.Status = EquipmentStatus.Repair; // 需確認 Enum 有 Repair (值為3)
                await _equipmentRepo.UpdateAsync(equipment);
            }

            await _maintenanceRepo.UpdateAsync(request);
        }

        // [Day 14] 完成維修
        public async Task CompleteMaintenanceAsync(CompleteMaintenanceDto dto)
        {
            // 1. 撈出維修單
            var request = await _maintenanceRepo.GetByIdAsync(dto.RequestId);
            if (request == null) throw new Exception("維修單不存在");

            // 2. 更新維修單資訊
            request.Status = "Closed";
            request.EndTime = DateTime.Now;
            request.Description += $" | 對策: {dto.Solution}"; // 簡單串接字串，實務上可開新欄位

            // 3. [關鍵] 機台狀態復歸 -> Idle
            var equipment = await _equipmentRepo.GetByIdAsync(request.EquipmentId);
            if (equipment != null)
            {
                equipment.Status = EquipmentStatus.Idle; // 恢復成閒置 (黃燈)
                equipment.LastUpdated = DateTime.Now;
                await _equipmentRepo.UpdateAsync(equipment);
            }

            // 4. 存檔
            await _maintenanceRepo.UpdateAsync(request);
        }
    }
}