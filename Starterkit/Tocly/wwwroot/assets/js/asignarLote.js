document.addEventListener("DOMContentLoaded", function () {

    const w = 1471;
    const h = 1357;

    const map = L.map('map', {
        crs: L.CRS.Simple,
        minZoom: -2
    });

    const bounds = [[0, 0], [h, w]];
    map.fitBounds(bounds);

    const colores = {
        verde: "#22c55e",
        amarillo: "#FFFF00",
        rojo: "#ef4444",
        azul: "#3b82f6"
    };

    let markers = [];
    function crearPunto(lote) {

        switch (lote.estado) {
            case "Disponible":
                lote.estado = "verde";
                break;
            case "En Venta":
                lote.estado = "amarillo";
                break;
            case "Vendido":
                lote.estado = "rojo";
                break;
            case "Entregado":
                lote.estado = "azul";
                break;
        }

        const marker = L.circleMarker([lote.y, lote.x], {
            radius: 14,
            color: colores[lote.estado],
            fillColor: colores[lote.estado],
            fillOpacity: 0.9
        });

        marker.estado = lote.estado;


        markers.push(marker);
        marker.addTo(map);

        return marker;
    }

    let mapasBD = [];
    let mapas = {};
    $.get('/Mapa/GetMapas', function (data) {
        mapasBD = data;
        data.forEach(m => {
            mapas[m.condominio] = {
                imagen: m.direccion,
                idMapa : m.idMapa,
                lotes: []
            };
        });
        actualizarSelector();
    });

    function actualizarSelector() {
        const nombreMapa = $("#nombreCondominio").text().trim();
        cargarMapa(nombreMapa);
    }

    let overlayActual = null;

    function limpiarMapa() {
        markers.forEach(m => map.removeLayer(m));
        markers = [];
    }

    function cargarMapa(nombreMapa) {

        const data = mapas[nombreMapa];

        limpiarMapa();

        if (!data) {
            alert(`No hay mapa configurado para ${nombreMapa}`);
            return;
        }

        $("#idMapa").val(data.idMapa);

        if (overlayActual) map.removeLayer(overlayActual);

        markers.forEach(m => map.removeLayer(m));
        markers = [];

        overlayActual = L.imageOverlay(data.imagen, bounds).addTo(map);

        $.get('/Lote/GetLotesMapa', { idMapa: data.idMapa }, function (lotes) {

            lotes.forEach(lote => {
                crearPunto(lote);
            });

        });
    }

    // Para obtener coordenadas
    map.on("click", (e) => {
        $("#X").val(e.latlng.lng.toFixed(6).replace(",", "."));
        $("#Y").val(e.latlng.lat.toFixed(6).replace(",", "."));
        console.log("x:", e.latlng.lng, "y:", e.latlng.lat);
    });

});