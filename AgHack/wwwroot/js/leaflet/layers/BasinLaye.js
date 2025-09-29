// 載入流域圖層
function LoadBasinLayer() {
    shp("/data/main_basin.zip").then(function (geojson) {
        var basinOverlay = L.geoJSON(geojson, {
            pane: 'basinPane', // 指定使用的 pane
            style: { color: "green", weight: 1, fill: true, fillOpacity: 0 },
            interactive: false, 
            onEachFeature: function (feature, layer) {
                if (feature.properties && feature.properties.BASIN_NAME) {
                    layer.bindTooltip(`<strong>流域名稱:</strong> ${feature.properties.BASIN_NAME}`, { sticky: true });
                }
            }
        });

        basinOverlay.addTo(map);
        layerControl.addOverlay(basinOverlay, "Basin");
    });
}