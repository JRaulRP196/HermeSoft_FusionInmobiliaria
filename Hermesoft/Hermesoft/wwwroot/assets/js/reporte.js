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
            radius: 2,
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

    function aplicarFiltro(tipo) {
        markers.forEach((m) => {
            map.removeLayer(m);
        });

        if (tipo === "Disponibilidad") {
            markers
                .filter((m) => m.estado === "verde")
                .forEach((m) => map.addLayer(m));
        } else if (tipo === "Ventas_Mes") {
            markers
                .filter((m) => esDelMesActual(m.fechaVenta))
                .forEach((m) => map.addLayer(m));
        } else {
            markers.forEach((m) => map.addLayer(m));
        }
    }

    function cargarFiltro() {
        const filtro = document.getElementById("filtro");
        if (filtro) {
            aplicarFiltro(filtro.value);
        }
    }

    document.getElementById("filtro").addEventListener("change", function () {
        aplicarFiltro(this.value);
    });

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
            $("#opciones").addClass("d-none");
            return;
        }

        $("#error").addClass("d-none");
        $("#opciones").removeClass("d-none");

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
            cargarFiltro();
        });
    }

    function esDelMesActual(fecha) {
        const hoy = new Date();
        const fechaVenta = new Date(fecha);

        return (
            fechaVenta.getMonth() === hoy.getMonth() &&
            fechaVenta.getFullYear() === hoy.getFullYear()
        );
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

    function recortarCanvas(canvas) {
        const ctx = canvas.getContext("2d");
        const { width, height } = canvas;

        const imageData = ctx.getImageData(0, 0, width, height).data;

        let top = null, left = null, right = null, bottom = null;

        for (let y = 0; y < height; y++) {
            for (let x = 0; x < width; x++) {
                const index = (y * width + x) * 4;
                const alpha = imageData[index + 3];

                if (alpha !== 0) {
                    if (top === null) top = y;
                    if (left === null || x < left) left = x;
                    if (right === null || x > right) right = x;
                    bottom = y;
                }
            }
        }

        const croppedWidth = right - left;
        const croppedHeight = bottom - top;

        const newCanvas = document.createElement("canvas");
        newCanvas.width = croppedWidth;
        newCanvas.height = croppedHeight;

        const newCtx = newCanvas.getContext("2d");
        newCtx.drawImage(canvas, left, top, croppedWidth, croppedHeight, 0, 0, croppedWidth, croppedHeight);

        return newCanvas;
    }

    function hayPuntosVisibles() {
        let visibles = false;

        map.eachLayer(layer => {
            if (layer instanceof L.CircleMarker && map.hasLayer(layer)) {
                visibles = true;
            }
        });

        return visibles;
    }

    function convertirBlancoNegro(canvas) {
        const ctx = canvas.getContext("2d");
        const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
        const data = imageData.data;

        for (let i = 0; i < data.length; i += 4) {
            const r = data[i];
            const g = data[i + 1];
            const b = data[i + 2];

            // fórmula luminancia
            const gris = 0.3 * r + 0.59 * g + 0.11 * b;

            data[i] = gris;
            data[i + 1] = gris;
            data[i + 2] = gris;
        }

        ctx.putImageData(imageData, 0, 0);
    }

    function capturarMapa() {

        setTimeout(() => {

            leafletImage(map, function (err, canvas) {

                if (err) {
                    console.error(err);
                    return;
                }

                const ctx = canvas.getContext("2d");


                const puntosCanvas = document.createElement("canvas");
                puntosCanvas.width = canvas.width;
                puntosCanvas.height = canvas.height;
                const puntosCtx = puntosCanvas.getContext("2d");
                puntosCtx.drawImage(canvas, 0, 0);

                ctx.clearRect(0, 0, canvas.width, canvas.height);


                const img = document.querySelector(".leaflet-image-layer");

                const rect = img.getBoundingClientRect();
                const mapRect = document.getElementById("map").getBoundingClientRect();

                const x = rect.left - mapRect.left;
                const y = rect.top - mapRect.top;
                const width = rect.width;
                const height = rect.height;

                const imageObj = new Image();
                imageObj.crossOrigin = "anonymous";
                imageObj.src = img.src;

                imageObj.onload = function () {


                    ctx.drawImage(imageObj, x, y, width, height);

                    ctx.drawImage(puntosCanvas, 0, 0);

                    const croppedCanvas = recortarCanvas(canvas);

                    if (!hayPuntosVisibles()) {
                        convertirBlancoNegro(croppedCanvas);
                    }

                    const finalImg = croppedCanvas.toDataURL("image/png");
                    const select = document.getElementById("filtro");
                    const tipoReporte = select.options[select.selectedIndex].text;
                    const condominio = document.getElementById("nombreCondominio").textContent;

                    fetch("/Reporte/GenerarPdf", {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify({
                            imagenBase64: finalImg,
                            condominio: condominio,
                            tipoReporte: tipoReporte
                        })
                    })
                        .then(res => res.blob())
                        .then(blob => {
                            const url = window.URL.createObjectURL(blob);
                            const a = document.createElement("a");
                            a.href = url;
                            a.download = "reporte.pdf";
                            a.click();
                        });
                };

            });

        }, 1200);
    }

    $("#btnDescargar").on("click", function () {
        capturarMapa();
    });

});

