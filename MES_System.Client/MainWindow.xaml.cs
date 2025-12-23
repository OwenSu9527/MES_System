using MES_System.Client.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MES_System.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();

            // 設定計時器：每 2 秒去後端撈一次資料 (Polling)
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // 立即執行一次
            // RefreshData(); [Day 22 修正] 移除這裡的 RefreshData();
            // 因為建構函式不能 await，移到下方的 Window_Loaded 處理
        }

        // [Day 22 修正] 新增此方法處理載入事件
        // 注意：只有事件處理器可以使用 async void
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await RefreshData(); // 這裡就可以安心使用 await 了
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            await RefreshData();
        }

        private async Task RefreshData()
        {
            try
            {
                var data = await _apiService.GetEquipmentsAsync();
                gridEquipments.ItemsSource = data;
                txtStatus.Text = $"最後更新時間: {DateTime.Now:HH:mm:ss} | 設備數量: {data.Count}";
            }
            catch (Exception ex)
            {
                txtStatus.Text = "連線異常，嘗試重連中...";
            }
        }
    }
}