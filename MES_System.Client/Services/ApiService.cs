using MES_System.Client.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;

namespace MES_System.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private static string? _jwtToken; // 暫存 Token

        // 請確認您的 Port 是否為 5289 (Day 21 設定)
        private const string BASE_URL = "http://localhost:5289/api";

        public ApiService()
        {
            _http = new HttpClient();
        }

        // 登入功能
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var response = await _http.PostAsJsonAsync($"{BASE_URL}/Auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    _jwtToken = result?.Token;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"連線失敗: {ex.Message}");
                return false;
            }
        }

        // 取得設備列表
        public async Task<List<Equipment>> GetEquipmentsAsync()
        {
            if (string.IsNullOrEmpty(_jwtToken)) return new List<Equipment>();

            try
            {
                // 設定 JWT Header
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);

                var list = await _http.GetFromJsonAsync<List<Equipment>>($"{BASE_URL}/Equipment");
                return list ?? new List<Equipment>();
            }
            catch
            {
                return new List<Equipment>();
            }
        }
    }
}