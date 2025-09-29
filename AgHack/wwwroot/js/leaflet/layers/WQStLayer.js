// 載入河川測站圖層
function LoadWQStLayer() {
    fetch('/api/WQ/GetAllWQSt')
        .then(response => response.json())
        .then(data => {
            const wqLayerGroup = L.layerGroup();

            data.forEach(station => {
                if (station.twD97Lat && station.twD97Lon) {
                    const marker = L.circleMarker([station.twD97Lat, station.twD97Lon], {
                        pane: 'wqPane', // 指定使用的 pane
                        radius: 8,
                        fillColor: "blue",
                        color: "white",
                        weight: 1,
                        opacity: 1,
                        fillOpacity: 0.8
                    });

                    // 定義 Popup 的內容
                    const popupContent = `
                                <table style="width: 100%; border-collapse: collapse;">
                                    <tr>
                                        <td colspan="2" style="border: 1px solid #ddd; padding: 8px; text-align: center; font-weight: bold; color: blue;">河川測站資訊</td>
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

                    marker.on('click', function () {

                    });

                    // 將 Marker 加入到 Layer Group
                    wqLayerGroup.addLayer(marker);
                }
            });

            wqLayerGroup.addTo(map);
            layerControl.addOverlay(wqLayerGroup, "Water Quality Stations");
        })
        .catch(error => console.error('Error loading WQ data:', error));
}



function GetWQStById(stId) {
    fetch(`/api/WQ/GetWQStById?stId=${stId}`)
        .then(response => response.json())
        .then(data => {

        })
        .catch(error => {
            console.error('Error fetching WQ station by ID:', error);
            throw error;
        });
}