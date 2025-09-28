// config.js

export const appConfig = {
    mapId: "Map",               // map container ID
    center: [23.5, 121],        // 預設中心 (台灣)
    zoom: 8,                    // 預設縮放
    minZoom: 5,
    maxZoom: 18,
    attributionControl: true,
    zoomControl: true,
    scaleControl: true          // 顯示比例尺
};

export const baseLayersConfig = {
    "OpenStreetMap": {
        url: "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
        options: { attribution: "© OpenStreetMap contributors" }
    },
    "Google 衛星": {
        url: "https://mt1.google.com/vt/lyrs=s&x={x}&y={y}&z={z}",
        options: { attribution: "© Google" }
    },
    "Google 地形": {
        url: "https://mt1.google.com/vt/lyrs=p&x={x}&y={y}&z={z}",
        options: { attribution: "© Google" }
    },
    "Esri 衛星": {
        url: "https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}",
        options: { attribution: "© Esri" }
    },
    "地形圖": {
        url: "https://{s}.tile.opentopomap.org/{z}/{x}/{y}.png",
        options: { attribution: "© OpenTopoMap contributors" }
    }
};

export const overlayConfig = {
    Rivers: { enabled: true, api: "/data/rivers.geojson" },
    Stations: { enabled: false, api: "/api/stations" },
    Boundaries: { enabled: false, api: "/data/boundaries.geojson" },
    WeatherStations: { enabled: false, api: "/api/weather-stations" },
    SoilData: { enabled: false, api: "/api/soil-data" },
    CropFields: { enabled: false, api: "/api/crop-fields" }
};

export const styleConfig = {
    river: { 
        color: "#1e88e5", 
        weight: 3, 
        opacity: 0.8,
        dashArray: null
    },
    station: { 
        iconUrl: "/assets/icons/station.png", 
        iconSize: [25, 25],
        iconAnchor: [12, 25],
        popupAnchor: [0, -25]
    },
    boundary: { 
        color: "#666", 
        dashArray: "5,5", 
        weight: 2,
        fillOpacity: 0.1
    },
    weatherStation: {
        iconUrl: "/assets/icons/weather.png",
        iconSize: [20, 20],
        iconAnchor: [10, 20]
    },
    soilData: {
        fillOpacity: 0.6,
        weight: 1,
        color: "#8D6E63"
    },
    cropField: {
        fillOpacity: 0.4,
        weight: 2,
        color: "#4CAF50"
    }
};

// 新增：圖層群組配置
export const layerGroupsConfig = {
    "環境數據": ["Rivers", "WeatherStations", "SoilData"],
    "農業數據": ["CropFields", "Stations"],
    "行政區域": ["Boundaries"]
};

// 新增：地圖控制項配置
export const controlsConfig = {
    scale: {
        position: 'bottomleft',
        imperial: false,
        metric: true
    },
    fullscreen: {
        position: 'topleft'
    },
    search: {
        position: 'topright',
        placeholder: '搜尋地點...'
    }
};
