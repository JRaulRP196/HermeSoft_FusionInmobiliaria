
document.addEventListener("DOMContentLoaded", function () {

    // ========= MAPA ============
    const w = 2000;
    const h = 1500;

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

    const filtroSelect = document.getElementById("filtro");
    const filtroMes = document.getElementById("filtroMes");
    const ventasMesGroup = document.getElementById("ventasMesGroup");
    const mensajeSinVentas = document.getElementById("mensajeSinVentas");
    const mapContainer = document.getElementById("mapContainer");
    const isReporte = !!filtroMes;
    const modalElement = document.getElementById("miModalCentro");
    const contenidoModal = document.getElementById("contenidoModal");
    const tieneModal = !!modalElement && !!contenidoModal;

    let markers = [];
    function crearPunto(lote, opciones = {}) {
        let estadoBase = lote.estado;

        switch (estadoBase) {
            case "Disponible":
                estadoBase = "verde";
                break;
            case "En Venta":
                estadoBase = "amarillo";
                break;
            case "Vendido":
                estadoBase = "rojo";
                break;
            case "Entregado":
                estadoBase = "azul";
                break;
        }

        const colorFinal = opciones.color || colores[estadoBase] || colores.verde;
        const estadoFinal = opciones.estado || estadoBase || "verde";

        const marker = L.circleMarker([lote.y, lote.x], {
            radius: 14,
            color: colorFinal,
            fillColor: colorFinal,
            fillOpacity: 0.9
        });

        marker.estado = estadoFinal;

        if (!opciones.disableModal && tieneModal) {
            marker.on("click", () => {
                contenidoModal.innerHTML = `
                <br>
                <p><b>Código:</b> ${lote.codigo}</p>
                <p><b>Área:</b> ${lote.area} m²</p>
                <p><b>Frente:</b> ${lote.frente} m</p>
                <p><b>Fondo:</b> ${lote.fondo} m</p>
                <p><b>Precio m²:</b> ₡${lote.precioM2.toLocaleString()}</p>
                <p><b>Precio Lista:</b> ₡${lote.precioLista.toLocaleString()}</p>
                <p><b>Precio Venta:</b> ₡${lote.precioVenta.toLocaleString()}</p>
                ${marker.estado === "verde" ? `
                    <a class="btn btn-primary waves-effect waves-light" href="/Ventas/StepperRegistro?lote=${encodeURIComponent(lote.codigo)}">
                        <i class="ri-wallet-3-line align-middle me-2"></i> Iniciar Proceso de Venta
                    </a>
                    <a class="btn btn-primary waves-effect waves-light" href="/Ventas/Prima?lote=${encodeURIComponent(lote.codigo)}">
                        <i class="ri-wallet-3-line align-middle me-2"></i> Calcular Prima
                    </a>
                ` : ``}
            `;

                const modal = new bootstrap.Modal(modalElement);
                modal.show();
            });
        }

        markers.push(marker);
        marker.addTo(map);

        return marker;
    }

    function aplicarFiltro(tipo) {
        markers.forEach(m => {
            map.removeLayer(m);
        });

        if (tipo === "Disponibilidad") {
            markers
                .filter(m => m.estado === "verde")
                .forEach(m => map.addLayer(m));
        }
        else {
            markers.forEach(m => map.addLayer(m));
        }
    }

    if (filtroSelect) {
        filtroSelect.addEventListener("change", function () {
            if (isReporte && this.value === "Ventas_Mes") {
                if (ventasMesGroup) ventasMesGroup.classList.remove("d-none");
                cargarVentasMes();
                return;
            }

            if (ventasMesGroup) ventasMesGroup.classList.add("d-none");
            aplicarFiltro(this.value);
            if (isReporte) {
                cargarLotesBase();
            }
        });
    }


    // ========= SLIDER / SELECTOR DE MAPAS ============

    let condominios = [];
    let indexMapa = 0;
    let mapas = {};
    let mapasBD = [];
    const nombreCondominio = document.getElementById("nombreCondominio");
    let condominiosCargados = false;
    let mapasCargados = false;

    $.get('/Condominio/Obtener', function (data) {
        condominios = data;
        condominiosCargados = true;
        intentarInicializar();
    });

    $.get('/Mapa/GetMapas', function (data) {
        mapasBD = data;
        data.forEach(m => {
            mapas[m.condominio] = {
                imagen: m.direccion,
                idMapa: m.idMapa
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
    let mapaActual = null;

    function mostrarMensajeSinVentas(mostrar) {
        if (!mensajeSinVentas || !mapContainer) return;
        if (mostrar) {
            mensajeSinVentas.classList.remove("d-none");
            mapContainer.classList.add("d-none");
        } else {
            mensajeSinVentas.classList.add("d-none");
            mapContainer.classList.remove("d-none");
            map.invalidateSize();
        }
    }

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

        if (overlayActual) map.removeLayer(overlayActual);

        markers.forEach(m => map.removeLayer(m));
        markers = [];

        mapaActual = data;
        overlayActual = L.imageOverlay(data.imagen, bounds).addTo(map);

        if (isReporte && filtroSelect && filtroSelect.value === "Ventas_Mes") {
            cargarVentasMes();
        } else {
            cargarLotesBase();
        }
    }

    function cargarLotesBase() {
        if (!mapaActual) return;
        mostrarMensajeSinVentas(false);
        $.get('/Lote/GetLotesMapa', { idMapa: mapaActual.idMapa }, function (lotes) {
            limpiarMapa();
            lotes.forEach(lote => {
                crearPunto(lote);
            });
            if (filtroSelect) aplicarFiltro(filtroSelect.value);
        });
    }

    function cargarVentasMes() {
        if (!mapaActual || !filtroMes) return;
        const mes = filtroMes.value;
        if (!mes) {
            mostrarMensajeSinVentas(false);
            limpiarMapa();
            return;
        }

        $.get('/Reporte/VentasMes', { idMapa: mapaActual.idMapa, mes: mes }, function (respuesta) {
            const lotes = (respuesta && respuesta.lotes) ? respuesta.lotes : [];
            limpiarMapa();

            if (!lotes.length) {
                mostrarMensajeSinVentas(true);
                return;
            }

            mostrarMensajeSinVentas(false);
            lotes.forEach(lote => {
                crearPunto(lote, { color: colores.verde, estado: "verde", disableModal: true });
            });
        }).fail(function () {
            mostrarMensajeSinVentas(false);
        });
    }

    //Condominios en select 
    $.get('/Condominio/Obtener', function (data) {
        for (let i = 0; i < data.length; i++) {
            $("#condominios").append("<option value='"+data[i].id+"'>"+data[i].nombre+" "+data[i].id +"</option>");
        }
    });

    if (isReporte && filtroMes) {
        const hoy = new Date();
        const mesActual = `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, "0")}`;
        if (!filtroMes.value) filtroMes.value = mesActual;
        filtroMes.addEventListener("change", function () {
            if (filtroSelect && filtroSelect.value === "Ventas_Mes") {
                cargarVentasMes();
            }
        });
    }
});
