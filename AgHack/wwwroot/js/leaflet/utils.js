// utils.js - 地圖工具函數

// 座標轉換工具
export const coordinateUtils = {
    // WGS84 轉 TWD97 (台灣座標系統)
    wgs84ToTwd97: function(lat, lng) {
        // 簡化的座標轉換，實際應用建議使用專業的座標轉換庫
        // 這裡返回原座標作為示例
        return { lat, lng };
    },
    
    // 計算兩點間距離 (公里)
    calculateDistance: function(lat1, lng1, lat2, lng2) {
        const R = 6371; // 地球半徑 (公里)
        const dLat = this.deg2rad(lat2 - lat1);
        const dLng = this.deg2rad(lng2 - lng1);
        const a = Math.sin(dLat/2) * Math.sin(dLat/2) +
                  Math.cos(this.deg2rad(lat1)) * Math.cos(this.deg2rad(lat2)) *
                  Math.sin(dLng/2) * Math.sin(dLng/2);
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
        return R * c;
    },
    
    deg2rad: function(deg) {
        return deg * (Math.PI/180);
    }
};

// 地圖控制工具
export const mapControls = {
    // 建立自訂控制項
    createCustomControl: function(className, content, clickHandler) {
        const CustomControl = L.Control.extend({
            onAdd: function(map) {
                const div = L.DomUtil.create('div', className);
                div.innerHTML = content;
                div.onclick = clickHandler;
                return div;
            }
        });
        return new CustomControl();
    },
    
    // 建立測量工具
    createMeasureTool: function() {
        return this.createCustomControl('measure-control', 
            '<i class="fas fa-ruler"></i>', 
            function() {
                console.log('啟動測量工具');
            }
        );
    },
    
    // 建立定位控制項
    createLocationControl: function() {
        return this.createCustomControl('location-control', 
            '<i class="fas fa-crosshairs"></i>', 
            function() {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(function(position) {
                        const lat = position.coords.latitude;
                        const lng = position.coords.longitude;
                        window.mapUtils.flyTo(lat, lng, 15);
                    });
                }
            }
        );
    }
};

// 資料處理工具
export const dataUtils = {
    // 格式化數值
    formatNumber: function(value, decimals = 2) {
        return parseFloat(value).toFixed(decimals);
    },
    
    // 格式化日期
    formatDate: function(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('zh-TW');
    },
    
    // 產生顏色依據數值
    getColorByValue: function(value, min, max, colorScale = ['#green', '#yellow', '#red']) {
        const normalized = (value - min) / (max - min);
        const index = Math.floor(normalized * (colorScale.length - 1));
        return colorScale[Math.max(0, Math.min(index, colorScale.length - 1))];
    },
    
    // 統計計算
    calculateStats: function(dataArray) {
        const sum = dataArray.reduce((a, b) => a + b, 0);
        const mean = sum / dataArray.length;
        const sortedArray = [...dataArray].sort((a, b) => a - b);
        const median = sortedArray[Math.floor(sortedArray.length / 2)];
        const min = Math.min(...dataArray);
        const max = Math.max(...dataArray);
        
        return { sum, mean, median, min, max, count: dataArray.length };
    }
};

// 動畫效果
export const animationUtils = {
    // 圖層淡入效果
    fadeInLayer: function(layer, duration = 1000) {
        layer.setStyle({ fillOpacity: 0, opacity: 0 });
        let start = null;
        
        function animate(timestamp) {
            if (!start) start = timestamp;
            const progress = (timestamp - start) / duration;
            
            if (progress < 1) {
                layer.setStyle({ 
                    fillOpacity: progress * 0.6, 
                    opacity: progress 
                });
                requestAnimationFrame(animate);
            } else {
                layer.setStyle({ fillOpacity: 0.6, opacity: 1 });
            }
        }
        
        requestAnimationFrame(animate);
    },
    
    // 標記彈跳動畫
    bounceMarker: function(marker) {
        const icon = marker.getIcon();
        const originalSize = icon.options.iconSize;
        
        marker.setIcon(L.icon({
            ...icon.options,
            iconSize: [originalSize[0] * 1.5, originalSize[1] * 1.5]
        }));
        
        setTimeout(() => {
            marker.setIcon(icon);
        }, 300);
    }
};

// 匯出所有工具
export default {
    coordinateUtils,
    mapControls,
    dataUtils,
    animationUtils
};