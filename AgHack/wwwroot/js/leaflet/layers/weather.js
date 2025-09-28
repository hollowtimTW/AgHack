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
                popupContent += `<h6><i class="fas fa-cloud-sun"></i> ${props.name || '氣象站'}</h6>`;
                
                if (props.temperature) {
                    popupContent += `<p><i class="fas fa-thermometer-half"></i> <strong>溫度:</strong> ${props.temperature}°C</p>`;
                }
                
                if (props.humidity) {
                    popupContent += `<p><i class="fas fa-tint"></i> <strong>濕度:</strong> ${props.humidity}%</p>`;
                }
                
                if (props.windSpeed) {
                    popupContent += `<p><i class="fas fa-wind"></i> <strong>風速:</strong> ${props.windSpeed} m/s</p>`;
                }
                
                if (props.rainfall) {
                    popupContent += `<p><i class="fas fa-cloud-rain"></i> <strong>降雨量:</strong> ${props.rainfall} mm</p>`;
                }
                
                popupContent += '</div>';
                
                const marker = L.marker([lat, lon], { icon: weatherIcon })
                    .bindPopup(popupContent);
                
                marker.addTo(layer);
            });
        })
        .catch(error => {
            console.error('載入氣象站資料時發生錯誤:', error);
        });

    return layer;
}