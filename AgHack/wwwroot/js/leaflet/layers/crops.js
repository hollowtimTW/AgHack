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
                    // �ھڧ@�������]�w���P�C��
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
                    popupContent += `<h6><i class="fas fa-seedling"></i> ${props.name || '�A��'}</h6>`;
                    
                    if (props.cropType) {
                        popupContent += `<p><strong>�@������:</strong> ${props.cropType}</p>`;
                    }
                    
                    if (props.area) {
                        popupContent += `<p><strong>���n:</strong> ${props.area} ����</p>`;
                    }
                    
                    if (props.plantingDate) {
                        popupContent += `<p><strong>�شӤ��:</strong> ${props.plantingDate}</p>`;
                    }
                    
                    if (props.expectedHarvest) {
                        popupContent += `<p><strong>�w�p��ì:</strong> ${props.expectedHarvest}</p>`;
                    }
                    
                    if (props.owner) {
                        popupContent += `<p><strong>�A���D:</strong> ${props.owner}</p>`;
                    }
                    
                    if (props.irrigationSystem) {
                        popupContent += `<p><strong>��@�t��:</strong> ${props.irrigationSystem}</p>`;
                    }
                    
                    // �ͪ����A���ܾ�
                    if (props.growthStage) {
                        const stageClass = getGrowthStageClass(props.growthStage);
                        popupContent += `<p><strong>�ͪ����q:</strong> <span class="${stageClass}">${props.growthStage}</span></p>`;
                    }
                    
                    popupContent += '<div class="mt-2">';
                    popupContent += `<button class="btn btn-sm btn-success" onclick="viewFieldDetails('${props.id}')">�d�ݸԱ�</button>`;
                    popupContent += '</div>';
                    popupContent += '</div>';
                    
                    layer.bindPopup(popupContent, {
                        maxWidth: 300,
                        className: 'custom-popup'
                    });
                    
                    // �ƹ��ƥ�
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
            console.error('���J�A�и�Ʈɵo�Ϳ��~:', error);
        });

    return layer;
}

// ���U��ơG�ھڥͪ����q��^CSS���O
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

// �����ơG�d�ݹA�иԲӸ��
window.viewFieldDetails = function(fieldId) {
    console.log('�d�ݹA�иԲӸ��:', fieldId);
    // �o�̥i�H���}�ҺA�����ξɯ��Բӭ���
    if (window.showFieldModal) {
        window.showFieldModal(fieldId);
    }
};