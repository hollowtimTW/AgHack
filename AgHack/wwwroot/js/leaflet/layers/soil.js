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
                    // �ھڤg�[������pH�ȳ]�w���P�C��
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
                    popupContent += `<h6><i class="fas fa-mountain"></i> �g�[���</h6>`;
                    
                    if (props.soilType) {
                        popupContent += `<p><strong>�g�[����:</strong> ${props.soilType}</p>`;
                    }
                    
                    if (props.pH) {
                        popupContent += `<p><strong>pH��:</strong> ${props.pH}</p>`;
                    }
                    
                    if (props.organicMatter) {
                        popupContent += `<p><strong>�������t�q:</strong> ${props.organicMatter}%</p>`;
                    }
                    
                    if (props.nitrogen) {
                        popupContent += `<p><strong>��t�q:</strong> ${props.nitrogen} ppm</p>`;
                    }
                    
                    if (props.phosphorus) {
                        popupContent += `<p><strong>�C�t�q:</strong> ${props.phosphorus} ppm</p>`;
                    }
                    
                    if (props.potassium) {
                        popupContent += `<p><strong>�[�t�q:</strong> ${props.potassium} ppm</p>`;
                    }
                    
                    popupContent += '</div>';
                    
                    layer.bindPopup(popupContent);
                    
                    // �ƹ��ƥ�
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
            console.error('���J�g�[��Ʈɵo�Ϳ��~:', error);
        });

    return layer;
}