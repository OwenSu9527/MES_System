using System;
using System.Collections.Generic;

namespace MES_System.Domain.Entities; // 原本是 MES_System.Infrastructure.GeneratedEntities;

public partial class WorkOrder
{
    public int Id { get; set; }

    public string OrderNo { get; set; } = null!;

    public string Product { get; set; } = null!;

    public int TargetQty { get; set; }

    public int CurrentQty { get; set; }

    public string Status { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
