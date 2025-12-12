using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES_System.Domain.Entities;

namespace MES_System.Application.Interfaces
{
    public interface IMaintenanceRepository
    {
        Task AddAsync(MaintenanceRequest request);
        // Day 12 只需要新增功能，查詢之後再補
    }
}