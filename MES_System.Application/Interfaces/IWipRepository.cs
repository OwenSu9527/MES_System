using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES_System.Domain.Entities;

namespace MES_System.Application.Interfaces
{
    /// <summary>
    /// 在製品庫存 (WIP) 資料存取介面
    /// </summary>
    public interface IWipRepository
    {
        // 根據工單ID和站點ID，找出目前的快照 (看之前有沒有堆貨)
        Task<WipSnapshot?> GetByWorkOrderAndStationAsync(int workOrderId, int stationId);

        // 新增一筆快照
        Task AddAsync(WipSnapshot wip);

        // 更新快照 (主要用來更新數量)
        Task UpdateAsync(WipSnapshot wip);
    }
}