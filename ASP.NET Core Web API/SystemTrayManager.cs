using System.Diagnostics;
using System.Drawing; // 需引用 System.Drawing
using System.Windows.Forms; // 需引用 System.Windows.Forms

namespace MES_System.WebAPI
{
    public class SystemTrayManager : IDisposable
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;
        private readonly IHostApplicationLifetime _lifetime;

        public SystemTrayManager(IHostApplicationLifetime lifetime)
        {
            _lifetime = lifetime;
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            // 1. 建立右鍵選單
            _contextMenu = new ContextMenuStrip();
            var exitItem = new ToolStripMenuItem("關閉伺服器 (Exit)", null, OnExitClicked);
            _contextMenu.Items.Add(exitItem);

            // 2. 設定托盤圖示
            _notifyIcon = new NotifyIcon
            {
                // 使用系統內建圖示，實務上可換成公司 Logo (.ico)
                Icon = SystemIcons.Shield,
                Text = "MES Backend Server (Running)",
                Visible = true,
                ContextMenuStrip = _contextMenu
            };

            // 3. 綁定雙擊事件 (可選：雙擊開啟瀏覽器 Swagger)
            _notifyIcon.DoubleClick += (s, e) =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "http://localhost:5289/swagger",
                    UseShellExecute = true
                });
            };
        }

        private void OnExitClicked(object? sender, EventArgs e)
        {
            // [Day 21] 密碼驗證邏輯
            if (CheckPassword())
            {
                _notifyIcon.Visible = false;
                // 通知 ASP.NET Core 停止 WebHost
                _lifetime.StopApplication();
                // 退出 Windows Forms 訊息迴圈, 指定使用 System.Windows.Forms 的 Application
                System.Windows.Forms.Application.Exit();
            }
        }

        private bool CheckPassword()
        {
            // 建立一個簡單的輸入視窗
            Form prompt = new Form()
            {
                Width = 300,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "安全驗證",
                StartPosition = FormStartPosition.CenterScreen,
                TopMost = true,
                MinimizeBox = false,
                MaximizeBox = false
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = "請輸入管理員密碼:", AutoSize = true };
            TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 240, PasswordChar = '*' };
            Button confirmation = new Button() { Text = "確定", Left = 160, Width = 100, Top = 90, DialogResult = DialogResult.OK };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            // 顯示視窗並等待結果
            if (prompt.ShowDialog() == DialogResult.OK)
            {
                // 這裡驗證密碼，簡單起見寫死，實務上可讀 appsettings
                if (inputBox.Text == "admin123")
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("密碼錯誤！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
            _contextMenu?.Dispose();
        }
    }
}