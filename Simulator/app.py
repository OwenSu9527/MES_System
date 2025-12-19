import requests
import time
import random
import json
import urllib3

# 關閉 SSL 警告 (如果是 localhost https)
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# =================設定區=================
# API 基礎網址
BASE_URL = "http://localhost:5289/api" 

# 管理員帳號 (用來自動登入)
USERNAME = "admin"
PASSWORD = "admin123"

# =======================================

# 定義 5 台機台的行為模式
# Status: 1=Running, 0=Idle, 2=Down, 3=Repair
machines = [
    {"id": 1, "name": "SMT-01", "status": 1, "base_rpm": 1200, "base_temp": 45},
    {"id": 2, "name": "AOI-01", "status": 1, "base_rpm": 800,  "base_temp": 38},
    {"id": 3, "name": "Reflow", "status": 1, "base_rpm": 100,  "base_temp": 240},
    {"id": 4, "name": "DieBond", "status": 0, "base_rpm": 0,    "base_temp": 25}, # 閒置中
    {"id": 5, "name": "WireBond", "status": 2, "base_rpm": 0,    "base_temp": 60}  # 故障過熱
]

def login():
    """自動登入取得 Token"""
    print(f"🔑 嘗試登入使用者: {USERNAME} ...")
    try:
        url = f"{BASE_URL}/Auth/login"
        payload = {"username": USERNAME, "password": PASSWORD}
        # 這裡不帶 Header，因為是登入
        response = requests.post(url, json=payload, verify=False)

        if response.status_code == 200:
            token = response.json().get("token")
            print("✅ 登入成功！已取得 Token")
            return token
        else:
            print(f"❌ 登入失敗: {response.status_code} - {response.text}")
            return None
    except Exception as e:
        print(f"❌ 連線錯誤: {e}")
        return None

def simulate_factory():
    # 1. 先執行自動登入
    token = login()
    if not token:
        print("無法取得 Token，模擬器終止。")
        return

    # 2. 設定 Header (帶入剛剛拿到的 Token)
    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {token}"
    }

    print("🏭 MES 工廠模擬器啟動中...")
    print("按 Ctrl+C 停止模擬")

    while True:
        try:
            for m in machines:
                # 模擬數據波動
                current_rpm = 0
                current_temp = 25.0

                if m["status"] == 1: # 運轉中
                    current_rpm = int(m["base_rpm"] * random.uniform(0.95, 1.05))
                    current_temp = round(m["base_temp"] + random.uniform(-2, 2), 1)
                elif m["status"] == 2: # 故障
                    current_rpm = 0
                    current_temp = round(m["base_temp"] - random.uniform(0, 0.5), 1)
                
                # 發送數據給 .NET API
                payload = {
                    "rpm": current_rpm,
                    "temperature": current_temp
                }
                
                url = f"{BASE_URL}/Equipment/{m['id']}/telemetry"
                
                # 使用剛剛建立的 headers (裡面是乾淨的 Token)
                response = requests.patch(url, json=payload, headers=headers, verify=False)

                if response.status_code == 204:
                    print(f"[{m['name']}] 更新成功: RPM={current_rpm}, Temp={current_temp}")
                elif response.status_code == 401:
                    print(f"[{m['name']}] ❌ Token 過期，重新登入中...")
                    token = login() # 重新登入刷新 Token
                    if token:
                        headers["Authorization"] = f"Bearer {token}"
                else:
                    print(f"[{m['name']}] 更新失敗: {response.status_code}")

            print("-" * 30)
            time.sleep(2)

        except KeyboardInterrupt:
            print("🛑 模擬器停止")
            break
        except Exception as e:
            print(f"發生錯誤: {e}")
            time.sleep(5)

if __name__ == "__main__":
    simulate_factory()