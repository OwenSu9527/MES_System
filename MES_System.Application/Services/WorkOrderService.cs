using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES_System.Application.DTOs;
using MES_System.Application.Interfaces;
using MES_System.Domain.Entities;

namespace MES_System.Application.Services
{
    // 實作 IWorkOrderService 介面
    public class WorkOrderService : IWorkOrderService
    {
        // Service 不直接依賴資料庫，而是依賴 Repository 介面
        private readonly IWorkOrderRepository _repository;

        public WorkOrderService(IWorkOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync()
        {
            // 簡單轉發給 Repository
            return await _repository.GetAllAsync();
        }

        public async Task<WorkOrder> CreateWorkOrderAsync(CreateWorkOrderDto dto)
        {
            // --- 核心業務邏輯 ---

            // 1. 轉換 (Mapping): 將前端傳來的簡單資料 (DTO) 轉為完整的資料庫實體 (Entity)
            var workOrder = new WorkOrder
            {
                OrderNo = dto.OrderNo,
                Product = dto.Product,
                TargetQty = dto.TargetQty,
                // 2. 設定預設值 (這些是前端不能決定的，由系統決定)
                CurrentQty = 0,         // 一開始產量必為 0
                Status = "New",         // 狀態預設為新建
                StartTime = DateTime.Now // 建立時間為當下
            };

            // 3. 呼叫 Repository 存入資料庫
            return await _repository.AddAsync(workOrder);
        }

        public async Task<bool> StartWorkOrderAsync(int id)
        {
            // 1. 先去 DB 查這張單存不存在
            var workOrder = await _repository.GetByIdAsync(id);
            if (workOrder == null) return false;

            // 2. 變更狀態邏輯
            workOrder.Status = "Running";
            workOrder.StartTime = DateTime.Now; // 更新實際開工時間

            // 3. 更新回資料庫
            await _repository.UpdateAsync(workOrder);
            return true;
        }
    }
}