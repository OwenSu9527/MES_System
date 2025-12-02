using System;
using System.Collections.Generic;

namespace MES_System.Domain.Entities; // 原本是 MES_System.Infrastructure.GeneratedEntities;

public partial class Station
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Sequence { get; set; }
}
