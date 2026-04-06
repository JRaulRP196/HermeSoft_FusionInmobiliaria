document.addEventListener("DOMContentLoaded", function () {
    // ========= MAPA ============
    const w = 1471;
    const h = 1357;

    const map = L.map("map", {
        crs: L.CRS.Simple,
        preferCanvas: true,
        minZoom: -2,
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        crossOrigin: true
    }).addTo(map);

    const bounds = [
        [0, 0],
        [h, w],
    ];
    map.fitBounds(bounds);

    const colores = {
        verde: "#22c55e",
        amarillo: "#FFFF00",
        rojo: "#ef4444",
        azul: "#3b82f6",
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
            fillOpacity: 0.9,
        });

        marker.estado = lote.estado;
        marker.fechaVenta = lote.fechaVenta;

        marker.on("click", () => {
            document.getElementById("contenidoModal").innerHTML = `
            <br>
            <p><b>Código:</b> ${lote.codigo}</p>
            <p><b>Área:</b> ${lote.area} m²</p>
            <p><b>Frente:</b> ${lote.frente} m</p>
            <p><b>Fondo:</b> ${lote.fondo} m</p>
            <p><b>Precio m²:</b> ₡${lote.precioM2.toLocaleString()}</p>
            <p><b>Precio Lista:</b> ₡${lote.precioLista.toLocaleString()}</p>
            <p><b>Precio Venta:</b> ₡${lote.precioVenta.toLocaleString()}</p>
            ${lote.estado === "rojo" || lote.estado === "azul" || lote.estado === "amarillo"
                    ? `<p><b>Fecha Venta:</b> ${new Date(lote.fechaVenta).toLocaleDateString()}</p>` : ``}
            ${lote.estado === "verde"
                    ? `
                <a class="btn btn-primary waves-effect waves-light" href="/Ventas/StepperRegistro?lote=${encodeURIComponent(lote.codigo)}">
                    <i class="ri-wallet-3-line align-middle me-2"></i> Iniciar Proceso de Venta
                </a>
                <a class="btn btn-primary waves-effect waves-light" href="/Ventas/Prima?lote=${encodeURIComponent(lote.codigo)}">
                    <i class="ri-wallet-3-line align-middle me-2"></i> Calcular Prima
                </a>
            `
                    : ``
                }
        `;

            const modal = new bootstrap.Modal(
                document.getElementById("miModalCentro"),
            );
            modal.show();
        });

        markers.push(marker);
        marker.addTo(map);

        return marker;
    }

    // ========= SLIDER / SELECTOR DE MAPAS ============

    let condominios = [];
    let indexMapa = 0;
    let mapas = {};
    let mapasBD = [];
    const nombreCondominio = document.getElementById("nombreCondominio");
    let condominiosCargados = false;
    let mapasCargados = false;

    $.get("/Condominio/Obtener", function (data) {
        condominios = data;
        condominiosCargados = true;
        intentarInicializar();
    });

    $.get("/Mapa/GetMapas", function (data) {
        mapasBD = data;
        data.forEach((m) => {
            mapas[m.condominio] = {
                imagen: m.direccion,
                idMapa: m.idMapa,
            };
        });
        mapasCargados = true;
        intentarInicializar();
        if (condominios.length > 0) {
            actualizarSelector();
        }
    });

    function intentarInicializar() {
        if (condominiosCargados && mapasCargados && condominios.length > 0) {
            actualizarSelector();
        }
    }

    document.getElementById("btnPrevCondominio").addEventListener("click", () => {
        indexMapa = (indexMapa - 1 + condominios.length) % condominios.length;
        actualizarSelector();
    });

    document.getElementById("btnNextCondominio").addEventListener("click", () => {
        indexMapa = (indexMapa + 1) % condominios.length;
        actualizarSelector();
    });

    function actualizarSelector() {
        const nombreMapa = condominios[indexMapa].id;
        nombreCondominio.textContent = nombreMapa;
        $("#listadoLotes").attr("data-condominio", condominios[indexMapa].id);
        cargarMapa(nombreMapa);
    }

    $("#listadoLotes").on("click", function (e) {
        e.preventDefault();
        const condominio = $("#listadoLotes").data("condominio");
        window.location.href = `/Lote/ListadoLotes?condominio=${encodeURIComponent(condominio)}`;
    });

    let overlayActual = null;

    function limpiarMapa() {
        markers.forEach((m) => map.removeLayer(m));
        markers = [];
    }

    function cargarMapa(nombreMapa) {
        const data = mapas[nombreMapa];

        limpiarMapa();

        if (!data) {
            map.removeLayer(overlayActual);
            $("#error").removeClass("d-none");
            $("#listadoLotes").addClass("d-none");
            return;
        }

        $("#error").addClass("d-none");
        $("#listadoLotes").removeClass("d-none");

        if (overlayActual) map.removeLayer(overlayActual);

        markers.forEach((m) => map.removeLayer(m));
        markers = [];

        overlayActual = L.imageOverlay(data.imagen, bounds, {
            crossOrigin: true
        }).addTo(map);

        $.get("/Lote/GetLotesMapa", { idMapa: data.idMapa }, function (lotes) {
            lotes.forEach((lote) => {
                crearPunto(lote);
            });
        });
    }

    //Condominios en select
    $.get("/Condominio/Obtener", function (data) {
        for (let i = 0; i < data.length; i++) {
            $("#condominios").append(
                "<option value='" +
                data[i].id +
                "'>" +
                data[i].nombre +
                " " +
                data[i].id +
                "</option>",
            );
        }
    });

});
