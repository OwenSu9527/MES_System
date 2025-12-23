using MES_System.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MES_System.Client
{
    /// <summary>
    /// LoginWindow.xaml 的互動邏輯
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService;

        public LoginWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            // 預設密碼方便測試
            txtPass.Password = "admin123";
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.Content = "驗證中...";
            btnLogin.IsEnabled = false;

            bool success = await _apiService.LoginAsync(txtUser.Text, txtPass.Password);

            if (success)
            {
                // 登入成功，開啟主視窗，並把 token 傳遞過去 (或由 Service 持有)
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("帳號或密碼錯誤", "登入失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                btnLogin.Content = "登入系統";
                btnLogin.IsEnabled = true;
            }
        }
    }
}
