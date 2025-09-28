import { appConfig, baseLayersConfig, overlayConfig, controlsConfig } from "./config.js";
import { createBaseLayers } from "./baselayers.js";
import { createRiversLayer } from "./layers/rivers.js";
import { createStationsLayer } from "./layers/stations.js";
import { createWeatherStationsLayer } from "./layers/weather.js";
import { createSoilDataLayer } from "./layers/soil.js";
import { createCropFieldsLayer } from "./layers/crops.js";
import { mapControls, dataUtils } from "./utils.js";

// 1. 初始化地圖
const map = L.map(appConfig.mapId, {
    center: appConfig.center,
    zoom: appConfig.zoom,
    minZoom: appConfig.minZoom,
    maxZoom: appConfig.maxZoom,
    zoomControl: appConfig.zoomControl,
    attributionControl: appConfig.attributionControl
});

// 2. 建立 baselayers
const baseLayers = createBaseLayers(baseLayersConfig);
baseLayers["OpenStreetMap"].addTo(map); // 預設載入 OSM

// 3. 建立 overlays
const overlays = {};
if (overlayConfig.Rivers.enabled) overlays["河流"] = createRiversLayer();
if (overlayConfig.Stations.enabled) overlays["監測站"] = createStationsLayer();
if (overlayConfig.Boundaries.enabled) overlays["行政邊界"] = createBoundariesLayer();
if (overlayConfig.WeatherStations.enabled) overlays["氣象站"] = createWeatherStationsLayer();
if (overlayConfig.SoilData.enabled) overlays["土壤資料"] = createSoilDataLayer();
if (overlayConfig.CropFields.enabled) overlays["農田"] = createCropFieldsLayer();

// 4. 加到 map (只載入啟用的圖層)
Object.entries(overlays).forEach(([name, layer]) => {
    const configKey = getConfigKey(name);
    if (overlayConfig[configKey] && overlayConfig[configKey].enabled) {
        layer.addTo(map);
    }
});

// 5. 建立 Layer Control
const layerControl = L.control.layers(baseLayers, overlays, {
    position: 'topright',
    collapsed: window.innerWidth < 768 // 手機版自動收合
}).addTo(map);

// 6. 新增控制項
// 比例尺
if (appConfig.scaleControl) {
    L.control.scale(controlsConfig.scale).addTo(map);
}

// 自訂控制項
mapControls.createLocationControl().addTo(map);
mapControls.createMeasureTool().addTo(map);

// 7. 地圖事件處理
map.on('click', function(e) {
    console.log('點擊座標:', e.latlng);
});

map.on('zoomend', function() {
    console.log('目前縮放等級:', map.getZoom());
    // 根據縮放等級調整圖層顯示
    adjustLayersForZoom(map.getZoom());
});

// 8. 工具函數
window.mapUtils = {
    // 飛到指定位置
    flyTo: function(lat, lng, zoom = 13) {
        map.flyTo([lat, lng], zoom);
    },
    
    // 取得目前視窗範圍
    getBounds: function() {
        return map.getBounds();
    },
    
    // 重設視圖到台灣
    resetView: function() {
        map.setView(appConfig.center, appConfig.zoom);
    },
    
    // 切換圖層
    toggleLayer: function(layerName) {
        const layer = overlays[layerName];
        if (layer) {
            if (map.hasLayer(layer)) {
                map.removeLayer(layer);
            } else {
                map.addLayer(layer);
            }
        }
    },
    
    // 取得所有圖層資料統計
    getLayersStats: function() {
        const stats = {};
        Object.keys(overlays).forEach(layerName => {
            // 這裡可以加入各圖層的統計計算
            stats[layerName] = { count: 0, visible: map.hasLayer(overlays[layerName]) };
        });
        return stats;
    }
};

// 9. 更新右側數據面板的函數
window.updateStationData = function(stationData) {
    console.log('更新監測站數據:', stationData);
    // 更新右側面板的數據顯示
    if (stationData.data) {
        updateDataPanel(stationData.data);
    }
};

// 10. 輔助函數
function getConfigKey(displayName) {
    const mapping = {
        "河流": "Rivers",
        "監測站": "Stations", 
        "行政邊界": "Boundaries",
        "氣象站": "WeatherStations",
        "土壤資料": "SoilData",
        "農田": "CropFields"
    };
    return mapping[displayName] || displayName;
}

function adjustLayersForZoom(zoomLevel) {
    // 根據縮放等級調整圖層顯示
    if (zoomLevel < 10) {
        // 縮放等級低時隱藏細節圖層
        if (overlays["監測站"] && map.hasLayer(overlays["監測站"])) {
            map.removeLayer(overlays["監測站"]);
        }
    } else {
        // 縮放等級高時顯示細節圖層
        if (overlays["監測站"] && !map.hasLayer(overlays["監測站"])) {
            map.addLayer(overlays["監測站"]);
        }
    }
}

function updateDataPanel(data) {
    // 更新右側數據面板
    const dataCards = document.querySelectorAll('.card-body h3');
    if (dataCards.length >= 4) {
        dataCards[0].textContent = data.total || '--';
        dataCards[1].textContent = data.average || '--';
        dataCards[2].textContent = data.max || '--';
        dataCards[3].textContent = data.min || '--';
    }
}

// 11. 載入預設圖層的佔位函數（待實作）
function createBoundariesLayer() {
    const layer = L.layerGroup();
    // 這裡可以載入行政邊界資料
    return layer;
}

// 12. 響應式設計調整
window.addEventListener('resize', function() {
    setTimeout(() => {
        map.invalidateSize();
    }, 100);
});
