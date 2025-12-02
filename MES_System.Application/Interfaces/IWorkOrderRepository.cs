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
        Task<WorkOrder> AddAsync(WorkOrder workOrder);
        Task UpdateAsync(WorkOrder workOrder);
    }
}
