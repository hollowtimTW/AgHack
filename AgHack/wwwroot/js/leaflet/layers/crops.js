import { overlayConfig, styleConfig } from "../config.js";

export function createCropFieldsLayer() {
    const layer = L.layerGroup();

    fetch(overlayConfig.CropFields.api)
        .then(res => {
            if (!res.ok) {
                throw new Error(`HTTP error! status: ${res.status}`);
            }
            return res.json();
        })
        .then(data => {
            L.geoJSON(data, {
                style: function(feature) {
                    // 根據作物類型設定不同顏色
                    const cropType = feature.properties.cropType;
                    let color = styleConfig.cropField.color;
                    
                    switch(cropType) {
                        case 'rice': color = '#4CAF50'; break;
                        case 'corn': color = '#FF9800'; break;
                        case 'wheat': color = '#FFC107'; break;
                        case 'vegetables': color = '#8BC34A'; break;
                        case 'fruits': color = '#E91E63'; break;
                        default: color = '#4CAF50';
                    }
                    
                    return {
                        fillColor: color,
                        fillOpacity: styleConfig.cropField.fillOpacity,
                        weight: styleConfig.cropField.weight,
                        color: color
                    };
                },
                onEachFeature: function(feature, layer) {
                    const props = feature.properties;
                    
                    let popupContent = '<div class="crop-popup">';
                    popupContent += `<h6><i class="fas fa-seedling"></i> ${props.name || '農田'}</h6>`;
                    
                    if (props.cropType) {
                        popupContent += `<p><strong>作物類型:</strong> ${props.cropType}</p>`;
                    }
                    
                    if (props.area) {
                        popupContent += `<p><strong>面積:</strong> ${props.area} 公頃</p>`;
                    }
                    
                    if (props.plantingDate) {
                        popupContent += `<p><strong>種植日期:</strong> ${props.plantingDate}</p>`;
                    }
                    
                    if (props.expectedHarvest) {
                        popupContent += `<p><strong>預計收穫:</strong> ${props.expectedHarvest}</p>`;
                    }
                    
                    if (props.owner) {
                        popupContent += `<p><strong>農場主:</strong> ${props.owner}</p>`;
                    }
                    
                    if (props.irrigationSystem) {
                        popupContent += `<p><strong>灌溉系統:</strong> ${props.irrigationSystem}</p>`;
                    }
                    
                    // 生長狀態指示器
                    if (props.growthStage) {
                        const stageClass = getGrowthStageClass(props.growthStage);
                        popupContent += `<p><strong>生長階段:</strong> <span class="${stageClass}">${props.growthStage}</span></p>`;
                    }
                    
                    popupContent += '<div class="mt-2">';
                    popupContent += `<button class="btn btn-sm btn-success" onclick="viewFieldDetails('${props.id}')">查看詳情</button>`;
                    popupContent += '</div>';
                    popupContent += '</div>';
                    
                    layer.bindPopup(popupContent, {
                        maxWidth: 300,
                        className: 'custom-popup'
                    });
                    
                    // 滑鼠事件
                    layer.on('mouseover', function(e) {
                        e.target.setStyle({
                            weight: 4,
                            fillOpacity: 0.7
                        });
                    });
                    
                    layer.on('mouseout', function(e) {
                        e.target.setStyle({
                            weight: styleConfig.cropField.weight,
                            fillOpacity: styleConfig.cropField.fillOpacity
                        });
                    });
                }
            }).addTo(layer);
        })
        .catch(error => {
            console.error('載入農田資料時發生錯誤:', error);
        });

    return layer;
}

// 輔助函數：根據生長階段返回CSS類別
function getGrowthStageClass(stage) {
    switch(stage) {
        case 'germination': return 'badge bg-info';
        case 'vegetative': return 'badge bg-success';
        case 'flowering': return 'badge bg-warning';
        case 'fruiting': return 'badge bg-primary';
        case 'harvest': return 'badge bg-danger';
        default: return 'badge bg-secondary';
    }
}

// 全域函數：查看農田詳細資料
window.viewFieldDetails = function(fieldId) {
    console.log('查看農田詳細資料:', fieldId);
    // 這裡可以打開模態視窗或導航到詳細頁面
    if (window.showFieldModal) {
        window.showFieldModal(fieldId);
    }
};