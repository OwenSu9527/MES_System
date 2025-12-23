using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_System.Client.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Status { get; set; } // 0, 1, 2
        public string Location { get; set; } = string.Empty;
        public int RPM { get; set; }
        public double Temperature { get; set; }

        // 為了 UI 顯示方便，加一個唯讀屬性把數字轉文字
        public string StatusText => Status switch
        {
            0 => "閒置 (Idle)",
            1 => "運轉中 (Running)",
            2 => "異常 (Down)",
            3 => "維修中 (Repair)",
            _ => "未知"
        };

        // 為了 UI 顯示顏色 (WPF 可以直接綁定顏色字串)
        public string StatusColor => Status switch
        {
            0 => "Orange",
            1 => "Green",
            2 => "Red",
            3 => "Blue",
            _ => "Gray"
        };
    }

    // 用來接收登入結果
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
    }
}