using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Application.DTOs
{
    // 定義傳輸物件 (Data Transfer Object)
    public class UpdateTelemetryDto
    {
        public int Rpm { get; set; }
        public double Temperature { get; set; }
    }
}
