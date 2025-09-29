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
                                        <td style="border: 1px solid #ddd; padding: 8px;">${station.siteId}</td>
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
                        GetUGStById(station.stId)
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


function GetUGStById(stId) {
    fetch(`/api/UG/GetLatestRecordsById/${stId}`)
        .then(response => response.json())
        .then(data => {
            console.log(data);
            // 添加測站資訊
            let analysisContent = `
                <div class="border p-2 mb-2">
                    <h5 class="text-primary mb-2">測站資訊</h5>
                    <p><strong>測站名稱：</strong>${data.stInfo.siteName} (${data.stInfo.siteEngName})</p>
                    <p><strong>測站編號：</strong>${data.stInfo.siteId}</p>
                    <p><strong>地下水分區：</strong>${data.stInfo.ugwDistName}</p>
                    <p><strong>測站地址：</strong>${data.stInfo.siteAddress}</p>
                    <p><strong>採集日期：</strong>${data.sampleDate}</p>
                    <button id="historyBtn" data-stid="${data.stInfo.stId}" class="btn btn-sm btn-outline-primary">歷史數據</button>
                </div>
            `;

            // 添加小方框數據
            analysisContent += '<div class="d-flex flex-wrap gap-3">';
            data.records.forEach(record => {
                analysisContent += `
                    <div class="border p-3 text-center" style="width: 150px;">
                        <div><strong>${record.itemName}</strong></div>
                        <div>(${record.itemEngabbreviation})</div>
                        <div class="rounded p-1 text-white" style="background-color: gray;">${record.itemValue !== null ? record.itemValue : '-'}</div>
                        <div>${record.itemUnit}</div>
                    </div>
                `;
            });
            analysisContent += '</div>';

            // 更新數據分析區的內容
            const analysisSection = document.querySelector('#data-content');
            if (analysisSection) {
                analysisSection.parentElement.scrollTop = 0; // 滾動到頂部
                analysisSection.innerHTML = analysisContent;

                // 綁定按鈕事件
                const historyBtn = document.querySelector('#historyBtn');
                if (historyBtn) {
                    historyBtn.addEventListener('click', function () {
                        const stId = this.getAttribute('data-stid');

                        // 顯示 Modal
                        const historyModal = new bootstrap.Modal(document.getElementById('historyRecordModal'));
                        historyModal.show();

                        // 可以在這裡加入額外的邏輯，例如載入歷史數據
                        console.log(`Fetching history data for station ID: ${stId}`);
                    });
                }
            }
        })
        .catch(error => {
            console.error('Error fetching WQ station by ID:', error);
            throw error;
        });
}


function ShowUGHisModal(stId) {

}

