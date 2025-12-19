using System;
using System.Collections.Generic;
using MES_System.Domain.Enums;

namespace MES_System.Domain.Entities; // 原本是 MES_System.Infrastructure.GeneratedEntities;

public partial class Equipment
{
    /// <summary>
    /// 主鍵 (Primary Key)
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 機台名稱 (e.g., SMT-01)
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// 位置 (e.g., Line-A)
    /// </summary>
    public string Location { get; set; } = null!;
    /// <summary>
    /// 設備狀態
    /// </summary>
    public EquipmentStatus Status { get; set; }
    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTime LastUpdated { get; set; }
    /// <summary>
    /// 轉速 (RPM - Revolutions Per Minute)
    /// </summary>
    public int RPM { get; set; }
    /// <summary>
    /// 運行溫度 (攝氏度)
    /// </summary>
    public double Temperature { get; set; }
}
