import { overlayConfig, styleConfig } from "../config.js";

export function createRiversLayer() {
    const layer = L.layerGroup();

    fetch(overlayConfig.Rivers.api)
        .then(res => {
            if (!res.ok) {
                throw new Error(`HTTP error! status: ${res.status}`);
            }
            return res.json();
        })
        .then(data => {
            L.geoJSON(data, {
                style: styleConfig.river,
                onEachFeature: function(feature, layer) {
                    // 加入 popup
                    if (feature.properties) {
                        let popupContent = '<div class="river-popup">';
                        popupContent += `<h6><i class="fas fa-water"></i> ${feature.properties.name || '河流'}</h6>`;
                        
                        if (feature.properties.length) {
                            popupContent += `<p><strong>長度:</strong> ${feature.properties.length} 公里</p>`;
                        }
                        
                        if (feature.properties.flow_rate) {
                            popupContent += `<p><strong>流量:</strong> ${feature.properties.flow_rate} m³/s</p>`;
                        }
                        
                        if (feature.properties.quality) {
                            popupContent += `<p><strong>水質:</strong> ${feature.properties.quality}</p>`;
                        }
                        
                        popupContent += '</div>';
                        layer.bindPopup(popupContent);
                    }
                    
                    // 滑鼠事件
                    layer.on('mouseover', function(e) {
                        e.target.setStyle({
                            weight: 5,
                            opacity: 1
                        });
                    });
                    
                    layer.on('mouseout', function(e) {
                        e.target.setStyle(styleConfig.river);
                    });
                }
            }).addTo(layer);
        })
        .catch(error => {
            console.error('載入河流資料時發生錯誤:', error);
            // 可選：顯示錯誤訊息給使用者
            const errorPopup = L.popup()
                .setLatLng([23.5, 121])
                .setContent('<div class="alert alert-warning">河流資料載入失敗</div>');
        });

    return layer;
}
