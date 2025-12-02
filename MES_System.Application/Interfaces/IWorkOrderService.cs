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
        Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync();
        Task<WorkOrder> CreateWorkOrderAsync(CreateWorkOrderDto dto);
        Task<bool> StartWorkOrderAsync(int id);
    }
}