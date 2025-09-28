import { overlayConfig, styleConfig } from "../config.js";

export function createWeatherStationsLayer() {
    const layer = L.layerGroup();

    fetch(overlayConfig.WeatherStations.api)
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
                
                const weatherIcon = L.icon({
                    iconUrl: styleConfig.weatherStation.iconUrl,
                    iconSize: styleConfig.weatherStation.iconSize,
                    iconAnchor: styleConfig.weatherStation.iconAnchor
                });
                
                let popupContent = '<div class="weather-popup">';
                popupContent += `<h6><i class="fas fa-cloud-sun"></i> ${props.name || '��H��'}</h6>`;
                
                if (props.temperature) {
                    popupContent += `<p><i class="fas fa-thermometer-half"></i> <strong>�ū�:</strong> ${props.temperature}�XC</p>`;
                }
                
                if (props.humidity) {
                    popupContent += `<p><i class="fas fa-tint"></i> <strong>���:</strong> ${props.humidity}%</p>`;
                }
                
                if (props.windSpeed) {
                    popupContent += `<p><i class="fas fa-wind"></i> <strong>���t:</strong> ${props.windSpeed} m/s</p>`;
                }
                
                if (props.rainfall) {
                    popupContent += `<p><i class="fas fa-cloud-rain"></i> <strong>���B�q:</strong> ${props.rainfall} mm</p>`;
                }
                
                popupContent += '</div>';
                
                const marker = L.marker([lat, lon], { icon: weatherIcon })
                    .bindPopup(popupContent);
                
                marker.addTo(layer);
            });
        })
        .catch(error => {
            console.error('���J��H����Ʈɵo�Ϳ��~:', error);
        });

    return layer;
}