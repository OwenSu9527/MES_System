using System;
using System.Collections.Generic;

namespace MES_System.Domain.Entities; // 原本是 MES_System.Infrastructure.GeneratedEntities;

public partial class WipSnapshot
{
    public int Id { get; set; }

    public int WorkOrderId { get; set; }

    public string WorkOrderNo { get; set; } = null!;

    public int StationId { get; set; }

    public string StationName { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime LastUpdated { get; set; }
}
