import { overlayConfig, styleConfig } from "../config.js";

export function createSoilDataLayer() {
    const layer = L.layerGroup();

    fetch(overlayConfig.SoilData.api)
        .then(res => {
            if (!res.ok) {
                throw new Error(`HTTP error! status: ${res.status}`);
            }
            return res.json();
        })
        .then(data => {
            L.geoJSON(data, {
                style: function(feature) {
                    // 根據土壤類型或pH值設定不同顏色
                    const soilType = feature.properties.soilType;
                    let color = styleConfig.soilData.color;
                    
                    switch(soilType) {
                        case 'clay': color = '#8D6E63'; break;
                        case 'sand': color = '#FFC107'; break;
                        case 'loam': color = '#795548'; break;
                        case 'silt': color = '#9E9E9E'; break;
                        default: color = '#8D6E63';
                    }
                    
                    return {
                        fillColor: color,
                        fillOpacity: styleConfig.soilData.fillOpacity,
                        weight: styleConfig.soilData.weight,
                        color: color
                    };
                },
                onEachFeature: function(feature, layer) {
                    const props = feature.properties;
                    
                    let popupContent = '<div class="soil-popup">';
                    popupContent += `<h6><i class="fas fa-mountain"></i> 土壤資料</h6>`;
                    
                    if (props.soilType) {
                        popupContent += `<p><strong>土壤類型:</strong> ${props.soilType}</p>`;
                    }
                    
                    if (props.pH) {
                        popupContent += `<p><strong>pH值:</strong> ${props.pH}</p>`;
                    }
                    
                    if (props.organicMatter) {
                        popupContent += `<p><strong>有機物含量:</strong> ${props.organicMatter}%</p>`;
                    }
                    
                    if (props.nitrogen) {
                        popupContent += `<p><strong>氮含量:</strong> ${props.nitrogen} ppm</p>`;
                    }
                    
                    if (props.phosphorus) {
                        popupContent += `<p><strong>磷含量:</strong> ${props.phosphorus} ppm</p>`;
                    }
                    
                    if (props.potassium) {
                        popupContent += `<p><strong>鉀含量:</strong> ${props.potassium} ppm</p>`;
                    }
                    
                    popupContent += '</div>';
                    
                    layer.bindPopup(popupContent);
                    
                    // 滑鼠事件
                    layer.on('mouseover', function(e) {
                        e.target.setStyle({
                            weight: 3,
                            fillOpacity: 0.8
                        });
                    });
                    
                    layer.on('mouseout', function(e) {
                        e.target.setStyle({
                            weight: styleConfig.soilData.weight,
                            fillOpacity: styleConfig.soilData.fillOpacity
                        });
                    });
                }
            }).addTo(layer);
        })
        .catch(error => {
            console.error('載入土壤資料時發生錯誤:', error);
        });

    return layer;
}