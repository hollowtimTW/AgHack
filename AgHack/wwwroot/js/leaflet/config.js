
// 地圖初始化設定
const mapConfig = {
    center: [23.5, 121], // 台灣中心點
    zoom: 8, // 初始縮放級別
    minZoom: 8, // 最小縮放級別
    maxZoom: 15, // 最大縮放級別
    maxBounds: [[21, 120], [26, 122]], // 限制地圖範圍
    attributionControl: true // 顯示版權資訊

}

// 定義底圖陣列
const baseLayerConfigs = [
    {
        name: "OpenStreetMap",
        layer: L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        })
    },
    {
        name: "CartoDB Light",
        layer: L.tileLayer('https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://carto.com/attributions">CARTO</a>'
        })
    },
    {
        name: "CartoDB Dark",
        layer: L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://carto.com/attributions">CARTO</a>'
        })
    },
    {
        name: "Google Streets",
        layer: L.tileLayer('http://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}', {
            maxZoom: 20,
            subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
            attribution: '&copy; Google'
        })
    },
    {
        name: "Google Satellite",
        layer: L.tileLayer('http://{s}.google.com/vt/lyrs=s&x={x}&y={y}&z={z}', {
            maxZoom: 20,
            subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
            attribution: '&copy; Google'
        })
    },
    {
        name: "Google Hybrid",
        layer: L.tileLayer('http://{s}.google.com/vt/lyrs=y&x={x}&y={y}&z={z}', {
            maxZoom: 20,
            subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
            attribution: '&copy; Google'
        })
    }
];