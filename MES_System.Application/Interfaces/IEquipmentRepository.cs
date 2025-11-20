using MES_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Application.Interfaces
{
    public interface IEquipmentRepository
    {
        // 定義兩個基本功能：取得所有機台、依照 ID 取得機台
        // 使用 Task 代表這是「非同步」操作 (Async)
        Task<IEnumerable<Equipment>> GetAllAsync();
        Task<Equipment?> GetByIdAsync(int id);
    }
}
