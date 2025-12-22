using MES_System.Domain.Entities;
using MES_System.Infrastructure;
using MES_System.WebAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MES_System.WebAPI.Controllers
{
    /// <summary>
    /// 身份驗證控制器
    /// </summary>
    /// <remarks>
    /// 負責處理使用者登入驗證與 JWT Token 的簽發。
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MesDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger; // [Day 17]

        // 注入
        public AuthController(MesDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public class LoginDto
        {
            // 若前端傳來的 JSON 少了 Username，後端會直接擋掉並回傳 400
            [StringLength(50, MinimumLength = 3)]
            public required string Username { get; set; }

            [StringLength(100, MinimumLength = 6)] // 限制密碼長度，避免過短或過長的無效嘗試
            public required string Password { get; set; }
        }

        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <remarks>
        /// 驗證帳號密碼，成功後回傳含有 JWT Token 的物件。
        /// 
        /// 測試帳號: **admin**
        /// 測試密碼: **admin123**
        /// </remarks>
        /// <param name="login">登入資訊 (帳號/密碼)</param>
        /// <returns>JWT Token 與使用者資訊</returns>
        /// <response code="200">登入成功，回傳 Token</response>
        /// <response code="401">登入失敗 (帳號或密碼錯誤)</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            // [Day 17 優化] 1. 加入 Log: 記錄有人嘗試登入 (使用 Information 等級)
            // 注意：不要記錄 Password！
            _logger.LogInformation("接收到登入請求: Username={Username}", login.Username);

            // 1. 查詢使用者是否存在
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == login.Username);

            // [Day 16 優化] 安全性檢查：這裡我們不直接 return，而是先標記 user 是否存在
            // 這樣可以避免駭客透過「回應時間」的微小差異來猜測帳號是否存在 (Timing Attack)
            // 但為了教學簡單，我們合併下方的錯誤訊息即可。

            bool isValid = false;

            if (user != null)
            {
                // 2. 驗證密碼(將輸入的密碼 Hash 後與 DB 比對)
                if (PasswordHelper.VerifyPassword(login.Password, user.PasswordHash))
                {
                    isValid = true;
                }
            }

            if (!isValid)
            {
                // [Day 17 優化] 2. 加入 Log: 記錄失敗原因 (使用 Warning 等級)
                // 這裡可以記錄詳細一點供後端除錯，但回傳給前端要模糊
                _logger.LogWarning("登入失敗: 帳號 {Username} 驗證失敗 (帳號不存在或密碼錯誤)", login.Username);

                // [Day 16 優化] 3. 統一錯誤訊息，防止帳號列舉攻擊
                return Unauthorized(new { message = "帳號或密碼錯誤" });
            }

            // [Day 17 優化] 4. 記錄成功 (Information)
            _logger.LogInformation("登入成功: 使用者 {Username} (Role: {Role})", user!.Username, user.Role);

            // 3. 製作 JWT Token
            var tokenString = GenerateJwtToken(user);

            return Ok(new { token = tokenString, username = user.Username, role = user.Role });
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString()) // 可以多塞 User ID 方便後續使用
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1), // 1天過期
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}