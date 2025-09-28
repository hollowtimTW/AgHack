export function createBaseLayers(baseLayersConfig) {
    const layers = {};
    for (const [name, cfg] of Object.entries(baseLayersConfig)) {
        layers[name] = L.tileLayer(cfg.url, cfg.options);
    }
    return layers;
}
