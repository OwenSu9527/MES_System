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
        // (舊有)
        // Service 不直接依賴資料庫，而是依賴 Repository 介面
        // private readonly IWorkOrderRepository _repository;

        // [Day8]
        private readonly IWorkOrderRepository _orderRepo; // 改名讓語意更清楚
        private readonly IWipRepository _wipRepo;         // 注入 WIP 倉庫

        // (舊有)
        //public WorkOrderService(IWorkOrderRepository repository)
        //{
        //    _repository = repository;
        //}

        // [Day8] 建構子注入：兩個 Repository
        public WorkOrderService(IWorkOrderRepository orderRepo, IWipRepository wipRepo)
        {
            _orderRepo = orderRepo;
            _wipRepo = wipRepo;
        }

        // (舊有)
        //public async Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync()
        //{
        //    // 簡單轉發給 Repository
        //    return await _repository.GetAllAsync();
        //}

        // [Day8] 取得所有工單
        public async Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync()
        {
            return await _orderRepo.GetAllAsync();
        }

        // 建立工單
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
            // (舊有)
            // 3. 呼叫 Repository 存入資料庫
            // return await _repository.AddAsync(workOrder);

            //
            return await _orderRepo.AddAsync(workOrder);
        }

        // 開始工單
        public async Task<bool> StartWorkOrderAsync(int id)
        {
            // 1. 先去 DB 查這張單存不存在
            var workOrder = await _orderRepo.GetByIdAsync(id);
            if (workOrder == null) return false;

            // 2. 變更狀態邏輯
            workOrder.Status = "Running";
            workOrder.StartTime = DateTime.Now; // 更新實際開工時間

            // 3. 更新回資料庫
            await _orderRepo.UpdateAsync(workOrder);
            return true;
        }

        // [Day 8] 生產回報核心邏輯
        public async Task<bool> ReportProductionAsync(ProductionReportDto dto)
        {
            // --- 步驟 1: 撈出工單 ---
            // 實務上建議在 Repository 加一個 GetByOrderNoAsync 方法效能較好
            // 為了不更動 Repository 介面，先用 GetAll + LINQ 篩選 (資料量少時沒問題)
            var allOrders = await _orderRepo.GetAllAsync();
            var workOrder = allOrders.FirstOrDefault(o => o.OrderNo == dto.OrderNo);

            // --- 步驟 2: 防呆驗證 (Validation) ---

            // 狀況 A: 工單號碼打錯，找不到單
            if (workOrder == null) return false;

            // 狀況 B: 工單還沒按「開始」，或者是已經「結案」的單，不准回報生產
            if (workOrder.Status != "Running") return false;

            // --- 步驟 3: 更新工單總進度 ---
            workOrder.CurrentQty += dto.Quantity;
            await _orderRepo.UpdateAsync(workOrder);

            // --- 步驟 4: 更新 WIP (站點堆貨量) ---

            // 4-1. 檢查這個站點，針對這張工單，之前有沒有紀錄？
            var wip = await _wipRepo.GetByWorkOrderAndStationAsync(workOrder.Id, dto.StationId);

            if (wip == null)
            {
                // [情況 A: 新增] 如果之前沒紀錄，代表這是這張單第一次流到這個站
                wip = new WipSnapshot
                {
                    WorkOrderId = workOrder.Id,
                    WorkOrderNo = workOrder.OrderNo,
                    StationId = dto.StationId,
                    StationName = "Station-" + dto.StationId, // 暫時簡化名稱
                    Quantity = dto.Quantity, // 數量就是這次回報的量
                    LastUpdated = DateTime.Now
                };
                await _wipRepo.AddAsync(wip);
            }
            else
            {
                // [情況 B: 更新] 如果之前有紀錄，就在原有的基礎上累加
                wip.Quantity += dto.Quantity;
                wip.LastUpdated = DateTime.Now;
                await _wipRepo.UpdateAsync(wip);
            }

            return true; // 回報成功
        }
    }
}