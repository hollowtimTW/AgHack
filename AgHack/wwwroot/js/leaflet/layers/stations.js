import { overlayConfig, styleConfig } from "../config.js";

export function createStationsLayer() {
    const layer = L.layerGroup();

    fetch(overlayConfig.Stations.api)
        .then(res => {
            if (!res.ok) {
                throw new Error(`HTTP error! status: ${res.status}`);
            }
            return res.json();
        })
        .then(data => {
            data.features.forEach(f => {
                const [lon, lat] = f.geometry.coordinates;
                const props = f.properties;
                
                // 建立自訂圖標
                const customIcon = L.icon({
                    iconUrl: styleConfig.station.iconUrl,
                    iconSize: styleConfig.station.iconSize,
                    iconAnchor: styleConfig.station.iconAnchor,
                    popupAnchor: styleConfig.station.popupAnchor
                });
                
                // 建立 popup 內容
                let popupContent = '<div class="station-popup">';
                popupContent += `<h6><i class="fas fa-broadcast-tower"></i> ${props.name || '監測站'}</h6>`;
                
                if (props.id) {
                    popupContent += `<p><strong>站點ID:</strong> ${props.id}</p>`;
                }
                
                if (props.type) {
                    popupContent += `<p><strong>類型:</strong> ${props.type}</p>`;
                }
                
                if (props.status) {
                    const statusClass = props.status === 'active' ? 'text-success' : 'text-danger';
                    popupContent += `<p><strong>狀態:</strong> <span class="${statusClass}">${props.status}</span></p>`;
                }
                
                if (props.lastUpdate) {
                    popupContent += `<p><strong>最後更新:</strong> ${props.lastUpdate}</p>`;
                }
                
                // 數據顯示
                if (props.data) {
                    popupContent += '<hr><h6>最新數據:</h6>';
                    for (const [key, value] of Object.entries(props.data)) {
                        popupContent += `<p><strong>${key}:</strong> ${value}</p>`;
                    }
                }
                
                popupContent += '<div class="mt-2">';
                popupContent += `<button class="btn btn-sm btn-primary" onclick="viewStationDetails('${props.id}')">詳細資料</button>`;
                popupContent += '</div>';
                popupContent += '</div>';
                
                const marker = L.marker([lat, lon], { icon: customIcon })
                    .bindPopup(popupContent, {
                        maxWidth: 300,
                        className: 'custom-popup'
                    });
                
                // 加入點擊事件
                marker.on('click', function(e) {
                    // 觸發數據更新到右側面板
                    if (window.updateStationData) {
                        window.updateStationData(props);
                    }
                });
                
                marker.addTo(layer);
            });
        })
        .catch(error => {
            console.error('載入監測站資料時發生錯誤:', error);
            const errorMarker = L.marker([23.5, 121])
                .bindPopup('<div class="alert alert-warning">監測站資料載入失敗</div>');
        });

    return layer;
}

// 全域函數：查看站點詳細資料
window.viewStationDetails = function(stationId) {
    console.log('查看站點詳細資料:', stationId);
    // 這裡可以打開模態視窗或導航到詳細頁面
    if (window.showStationModal) {
        window.showStationModal(stationId);
    }
};
