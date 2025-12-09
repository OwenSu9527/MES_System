using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES_System.Application.DTOs;
using MES_System.Domain.Entities;

namespace MES_System.Application.Interfaces
{
    public interface IWorkOrderService
    {
        // 取得所有工單
        Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync();
        // 建立工單
        Task<WorkOrder> CreateWorkOrderAsync(CreateWorkOrderDto dto);
        // 開始工單
        Task<bool> StartWorkOrderAsync(int id);

        // [Day 8] 生產回報
        // 這裡就是您錯誤訊息指出的 "缺少的定義"
        Task<bool> ReportProductionAsync(ProductionReportDto dto);
    }
}