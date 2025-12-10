using MES_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Application.Interfaces
{
    public interface IWorkOrderRepository
    {
        Task<IEnumerable<WorkOrder>> GetAllAsync();
        Task<WorkOrder?> GetByIdAsync(int id);

        // [Day 9 新增] 依工單號查詢 (為了效能)
        Task<WorkOrder?> GetByOrderNoAsync(string orderNo);
        Task<WorkOrder> AddAsync(WorkOrder workOrder);
        Task UpdateAsync(WorkOrder workOrder);
    }
}
