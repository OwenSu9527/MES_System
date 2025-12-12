using Moq;
using Xunit;
using MES_System.Application.Services; // 稍後會建立
using MES_System.Application.Interfaces;
using MES_System.Application.DTOs;
using MES_System.Domain.Entities;
using MES_System.Domain.Enums;

namespace MES_System.Application.Tests
{
    public class MaintenanceServiceTests
    {
        // 定義我們要模擬 (Mock) 的依賴項目
        private readonly Mock<IMaintenanceRepository> _mockMaintenanceRepo;
        private readonly Mock<IEquipmentRepository> _mockEquipmentRepo;
        private readonly MaintenanceService _service; // 這是我們要測的主角

        public MaintenanceServiceTests()
        {
            // 初始化 Mock 物件
            _mockMaintenanceRepo = new Mock<IMaintenanceRepository>();
            _mockEquipmentRepo = new Mock<IEquipmentRepository>();

            // 初始化 Service，將 Mock 物件注入進去 (騙它說這些是真的 Repo)
            // 注意：MaintenanceService 此時還沒建立，會有紅底線，這是正常的 TDD 過程
            _service = new MaintenanceService(_mockMaintenanceRepo.Object, _mockEquipmentRepo.Object);
        }

        [Fact] // 標記這是一個測試案例
        public async Task CreateRequest_Should_Change_EquipmentStatus_To_Down()
        {
            // 1. Arrange (佈置情境)
            var equipmentId = 1;
            // 模擬：當 Repo 去找 ID=1 的機台時，回傳一個狀態為 "Running" 的機台
            var fakeEquipment = new Equipment { Id = equipmentId, Status = EquipmentStatus.Running };

            _mockEquipmentRepo.Setup(repo => repo.GetByIdAsync(equipmentId))
                .ReturnsAsync(fakeEquipment);

            var dto = new CreateMaintenanceDto
            {
                EquipmentId = equipmentId,
                ReasonCode = "Err-01",
                RequestUser = "Operator A"
            };

            // 2. Act (執行動作)
            await _service.CreateMaintenanceRequestAsync(dto);

            // 3. Assert (驗證結果)
            // 驗證機台狀態是否變成了 Down (故障)
            Assert.Equal(EquipmentStatus.Down, fakeEquipment.Status);

            // 驗證是否真的有呼叫 EquipmentRepository.UpdateAsync (也就是有存檔)
            // 注意：我們需要在 IEquipmentRepository 補上 UpdateAsync 方法才能驗證這步
            // 這裡我們先驗證狀態變更即可。
        }
    }
}