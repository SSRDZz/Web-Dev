function parseLatLng(value) {
    if (!value) return null;
    const matched = value.trim().match(/^(-?\d+(?:\.\d+)?)\s*,\s*(-?\d+(?:\.\d+)?)$/);
    if (!matched) return null;

    const lat = parseFloat(matched[1]);
    const lng = parseFloat(matched[2]);
    if (Number.isNaN(lat) || Number.isNaN(lng)) return null;

    return { lat, lng };
}

(function initCreateMap() {
    const mapContainer = document.getElementById("create-activity-map");
    const mapUrlInput = document.getElementById("map-url-input");

    if (!mapContainer || !mapUrlInput || typeof L === "undefined") return;

    const defaultLat = 13.7563;
    const defaultLng = 100.5018;
    const defaultZoom = 10;

    const existing = parseLatLng(mapUrlInput.value);
    const startLat = existing ? existing.lat : defaultLat;
    const startLng = existing ? existing.lng : defaultLng;
    const startZoom = existing ? 15 : defaultZoom;

    const map = L.map("create-activity-map").setView([startLat, startLng], startZoom);

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors'
    }).addTo(map);

    let marker = null;

    function setPoint(lat, lng) {
        if (marker) {
            marker.setLatLng([lat, lng]);
        } else {
            marker = L.marker([lat, lng]).addTo(map);
        }

        mapUrlInput.value = `${lat.toFixed(6)},${lng.toFixed(6)}`;
    }

    if (existing) {
        setPoint(existing.lat, existing.lng);
    }

    map.on("click", function (event) {
        const { lat, lng } = event.latlng;
        setPoint(lat, lng);
    });
})();
