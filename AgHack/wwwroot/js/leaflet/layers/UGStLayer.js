// 載入地下水測站圖層
function LoadUGStLayer() {
    fetch('/api/UG/GetAllUGSt')
        .then(response => response.json())
        .then(data => {
            const ugLayerGroup = L.layerGroup();

            data.forEach(station => {
                if (station.twD97Lat && station.twD97Lon) {
                    const marker = L.circleMarker([station.twD97Lat, station.twD97Lon], {
                        pane: 'ugPane', // 指定使用的 pane
                        radius: 8,
                        fillColor: "Green",
                        color: "Red",
                        weight: 1,
                        opacity: 1,
                        fillOpacity: 0.8
                    });

                    // 定義 Popup 的內容
                    const popupContent = `
                                <table style="width: 100%; border-collapse: collapse;">
                                    <tr>
                                        <td colspan="2" style="border: 1px solid #ddd; padding: 8px; text-align: center; font-weight: bold; color: Green;">地下水測站資訊</td>
                                    </tr>
                                    <tr>
                                        <td style="border: 1px solid #ddd; padding: 8px;">測站名稱</td>
                                        <td style="border: 1px solid #ddd; padding: 8px;">${station.siteName}</td>
                                    </tr>
                                    <tr>
                                        <td style="border: 1px solid #ddd; padding: 8px;">測站編號</td>
                                        <td style="border: 1px solid #ddd; padding: 8px;">${station.stId}</td>
                                    </tr>
                                </table>
                            `;

                    marker.on('mouseover', function () {
                        marker.bindPopup(popupContent, {
                            offset: [0, -10],
                            closeButton: false,
                            autoPan: true
                        }).openPopup();
                    });

                    marker.on('mouseout', function () {
                        marker.closePopup();
                    });

                    // 將 Marker 加入到 Layer Group
                    ugLayerGroup.addLayer(marker);
                }
            });

            ugLayerGroup.addTo(map);
            layerControl.addOverlay(ugLayerGroup, "Underground Water Stations");
        })
        .catch(error => console.error('Error loading UG data:', error));
}