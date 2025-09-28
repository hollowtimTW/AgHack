# AgHack API 文件

## 簡介
AgHack 是一個農業水質監測系統，提供完整的 RESTful API 來存取水質、地下水和工業廢水的監測資料。

## API 基本資訊
- **Base URL**: `https://localhost:7xxx/api/`
- **版本**: v1
- **資料格式**: JSON
- **API 文件**: 開發環境下可透過 `/swagger` 存取 Swagger UI

## 主要 API 端點

### 1. 水質監測 API (`/api/WaterQualityApi`)
- `GET /stations` - 取得所有水質測站
- `GET /stations/{id}` - 取得特定測站詳細資料
- `GET /records` - 取得監測記錄 (支援篩選與分頁)
- `GET /items` - 取得監測項目清單
- `GET /statistics` - 取得統計資料

### 2. 地下水監測 API (`/api/GroundwaterApi`)
- `GET /stations` - 取得所有地下水測站
- `GET /stations/{id}` - 取得特定測站詳細資料
- `GET /records` - 取得監測記錄 (支援篩選與分頁)
- `GET /items` - 取得監測項目清單
- `GET /statistics` - 取得統計資料

### 3. 工業廢水監測 API (`/api/IndustrialWastewaterApi`)
- `GET /departments` - 取得部門清單
- `GET /stations` - 取得工業廢水測站
- `GET /stations/{id}` - 取得特定測站詳細資料
- `GET /monitoringpoints` - 取得監測點清單
- `GET /records` - 取得監測記錄
- `GET /statistics` - 取得統計資料

### 4. 參考資料 API (`/api/ReferenceApi`)
- `GET /counties` - 取得縣市清單
- `GET /towns` - 取得鄉鎮市區清單
- `GET /basins` - 取得流域清單
- `GET /rivers` - 取得河川清單
- `GET /water-quality-categories` - 取得水質監測項目分類
- `GET /groundwater-categories` - 取得地下水監測項目分類
- `GET /all-stations-summary` - 取得所有測站統計摘要

### 5. 搜尋查詢 API (`/api/SearchApi`)
- `GET /stations` - 搜尋測站 (關鍵字、地區篩選)
- `GET /nearby-stations` - 搜尋附近測站 (座標搜尋)
- `GET /recent-data` - 取得最近監測資料

## 使用範例

### 取得所有水質測站
```
GET /api/WaterQualityApi/stations
```

### 取得特定測站的監測記錄 (分頁)
```
GET /api/WaterQualityApi/records?stationId=1&page=1&pageSize=50
```

### 搜尋附近測站 (以座標為中心，5公里範圍)
```
GET /api/SearchApi/nearby-stations?latitude=25.047924&longitude=121.517081&radiusKm=5
```

### 取得最近 7 天的水質資料
```
GET /api/SearchApi/recent-data?dataType=WQ&days=7&limit=100
```

## 錯誤處理
所有 API 都包含適當的錯誤處理，會回傳標準的 HTTP 狀態碼：
- `200 OK` - 成功
- `404 Not Found` - 找不到資源
- `500 Internal Server Error` - 伺服器內部錯誤

錯誤回應格式：
```json
{
    "message": "錯誤描述",
    "error": "詳細錯誤訊息"
}
```

## 分頁支援
支援分頁的 API 會回傳以下格式：
```json
{
    "data": [...],
    "pagination": {
        "page": 1,
        "pageSize": 100,
        "totalCount": 1500,
        "totalPages": 15
    }
}
```

## 使用 Swagger UI
在開發環境中，可以透過瀏覽器存取 `https://localhost:7xxx/swagger` 來使用 Swagger UI 介面，提供：
- 完整的 API 文件
- 互動式 API 測試
- 請求/回應範例
- API 分組顯示