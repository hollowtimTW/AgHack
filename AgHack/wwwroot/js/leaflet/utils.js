// utils.js - �a�Ϥu����

// �y���ഫ�u��
export const coordinateUtils = {
    // WGS84 �� TWD97 (�x�W�y�Шt��)
    wgs84ToTwd97: function(lat, lng) {
        // ²�ƪ��y���ഫ�A������Ϋ�ĳ�ϥαM�~���y���ഫ�w
        // �o�̪�^��y�Ч@���ܨ�
        return { lat, lng };
    },
    
    // �p����I���Z�� (����)
    calculateDistance: function(lat1, lng1, lat2, lng2) {
        const R = 6371; // �a�y�b�| (����)
        const dLat = this.deg2rad(lat2 - lat1);
        const dLng = this.deg2rad(lng2 - lng1);
        const a = Math.sin(dLat/2) * Math.sin(dLat/2) +
                  Math.cos(this.deg2rad(lat1)) * Math.cos(this.deg2rad(lat2)) *
                  Math.sin(dLng/2) * Math.sin(dLng/2);
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
        return R * c;
    },
    
    deg2rad: function(deg) {
        return deg * (Math.PI/180);
    }
};

// �a�ϱ���u��
export const mapControls = {
    // �إߦۭq���
    createCustomControl: function(className, content, clickHandler) {
        const CustomControl = L.Control.extend({
            onAdd: function(map) {
                const div = L.DomUtil.create('div', className);
                div.innerHTML = content;
                div.onclick = clickHandler;
                return div;
            }
        });
        return new CustomControl();
    },
    
    // �إߴ��q�u��
    createMeasureTool: function() {
        return this.createCustomControl('measure-control', 
            '<i class="fas fa-ruler"></i>', 
            function() {
                console.log('�Ұʴ��q�u��');
            }
        );
    },
    
    // �إߩw�챱�
    createLocationControl: function() {
        return this.createCustomControl('location-control', 
            '<i class="fas fa-crosshairs"></i>', 
            function() {
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(function(position) {
                        const lat = position.coords.latitude;
                        const lng = position.coords.longitude;
                        window.mapUtils.flyTo(lat, lng, 15);
                    });
                }
            }
        );
    }
};

// ��ƳB�z�u��
export const dataUtils = {
    // �榡�Ƽƭ�
    formatNumber: function(value, decimals = 2) {
        return parseFloat(value).toFixed(decimals);
    },
    
    // �榡�Ƥ��
    formatDate: function(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('zh-TW');
    },
    
    // �����C��̾ڼƭ�
    getColorByValue: function(value, min, max, colorScale = ['#green', '#yellow', '#red']) {
        const normalized = (value - min) / (max - min);
        const index = Math.floor(normalized * (colorScale.length - 1));
        return colorScale[Math.max(0, Math.min(index, colorScale.length - 1))];
    },
    
    // �έp�p��
    calculateStats: function(dataArray) {
        const sum = dataArray.reduce((a, b) => a + b, 0);
        const mean = sum / dataArray.length;
        const sortedArray = [...dataArray].sort((a, b) => a - b);
        const median = sortedArray[Math.floor(sortedArray.length / 2)];
        const min = Math.min(...dataArray);
        const max = Math.max(...dataArray);
        
        return { sum, mean, median, min, max, count: dataArray.length };
    }
};

// �ʵe�ĪG
export const animationUtils = {
    // �ϼh�H�J�ĪG
    fadeInLayer: function(layer, duration = 1000) {
        layer.setStyle({ fillOpacity: 0, opacity: 0 });
        let start = null;
        
        function animate(timestamp) {
            if (!start) start = timestamp;
            const progress = (timestamp - start) / duration;
            
            if (progress < 1) {
                layer.setStyle({ 
                    fillOpacity: progress * 0.6, 
                    opacity: progress 
                });
                requestAnimationFrame(animate);
            } else {
                layer.setStyle({ fillOpacity: 0.6, opacity: 1 });
            }
        }
        
        requestAnimationFrame(animate);
    },
    
    // �аO�u���ʵe
    bounceMarker: function(marker) {
        const icon = marker.getIcon();
        const originalSize = icon.options.iconSize;
        
        marker.setIcon(L.icon({
            ...icon.options,
            iconSize: [originalSize[0] * 1.5, originalSize[1] * 1.5]
        }));
        
        setTimeout(() => {
            marker.setIcon(icon);
        }, 300);
    }
};

// �ץX�Ҧ��u��
export default {
    coordinateUtils,
    mapControls,
    dataUtils,
    animationUtils
};