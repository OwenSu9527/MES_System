using System;
using System.Collections.Generic;

namespace MES_System.Domain.Entities; // 原本是 MES_System.Infrastructure.GeneratedEntities;

public partial class Station
{
    public int Id { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// 站點順序编號
    /// </summary>
    public int Sequence { get; set; }
}
