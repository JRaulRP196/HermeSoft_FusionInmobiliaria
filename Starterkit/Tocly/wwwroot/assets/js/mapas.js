document.addEventListener("DOMContentLoaded", function () {


    // ========= MAPA ============
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
        amarillo: "#eab308",
        rojo: "#ef4444",
        azul: "#3b82f6"
    };

    let markers = [];
    function crearPunto(lote) {

        const marker = L.circleMarker([lote.y, lote.x], {
            radius: 14,
            color: colores[lote.estado],
            fillColor: colores[lote.estado],
            fillOpacity: 0.9
        });

        marker.estado = lote.estado; 

        marker.on("click", () => {
            document.getElementById("contenidoModal").innerHTML = `
            <h4>${lote.nombre}</h4>
            <br>
            <p><b>Código:</b> ${lote.codigo}</p>
            <p><b>Área:</b> ${lote.area} m²</p>
            <p><b>Frente:</b> ${lote.frente} m</p>
            <p><b>Fondo:</b> ${lote.fondo} m</p>
            <p><b>Precio m²:</b> ₡${lote.precioM2.toLocaleString()}</p>
            <p><b>Precio Lista:</b> ₡${lote.precioLista.toLocaleString()}</p>
            <p><b>Precio Venta:</b> ₡${lote.precioVenta.toLocaleString()}</p>
            ${lote.estado === "verde" ? `
                <a class="btn btn-primary waves-effect waves-light" href="/Ventas/Registro">
                    <i class="ri-wallet-3-line align-middle me-2"></i> Iniciar Proceso de Venta
                </a>
            ` : ``}
        `;

            const modal = new bootstrap.Modal(document.getElementById("miModalCentro"));
            modal.show();
        });

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
        else if (tipo === "Ventas_Mes") {
            markers
                .filter(m => m.estado === "amarillo")
                .forEach(m => map.addLayer(m));
        }
        else {
            markers.forEach(m => map.addLayer(m));
        }
    }

    document.getElementById("filtro").addEventListener("change", function () {
        aplicarFiltro(this.value);
    });


    // ========= SLIDER / SELECTOR DE MAPAS ============
    const listaMapas = ["mapa1", "mapa2", "mapa3"];
    let indexMapa = 0;
    const nombreCondominio = document.getElementById("nombreCondominio");

    document.getElementById("btnPrevCondominio").addEventListener("click", () => {
        indexMapa = (indexMapa - 1 + listaMapas.length) % listaMapas.length;
        actualizarSelector();
    });

    document.getElementById("btnNextCondominio").addEventListener("click", () => {
        indexMapa = (indexMapa + 1) % listaMapas.length;
        actualizarSelector();
    });

    function actualizarSelector() {
        const nombreMapa = listaMapas[indexMapa];
        nombreCondominio.textContent = nombreMapa;
        cargarMapa(nombreMapa);
    }

    // ========= DATOS DE MAPAS ============
    const mapas = {
        mapa1: {
            imagen: "/assets/images/Mapas/Mapa_Condominio.jpg",
            lotes: [
                {
                    x: 359.53, y: 803.91, estado: "verde", nombre: "Lote 12",
                    area: 342, frente: 13, fondo: 28, precioM2: 72000, precioLista: 24624000, precioVenta: 23392800, codigo: "LT-001"
                },
                {
                    x: 374.53, y: 785.91, estado: "amarillo", nombre: "Lote 14",
                    area: 290, frente: 10, fondo: 27, precioM2: 68000, precioLista: 19720000, precioVenta: 18734000, codigo: "LT-002"
                },
                {
                    x: 402.53, y: 770.91, estado: "rojo", nombre: "Lote 15",
                    area: 410, frente: 15, fondo: 30, precioM2: 75000, precioLista: 30750000, precioVenta: 29212500, codigo: "LT-003"
                },
                {
                    x: 379.03, y: 740.91, estado: "azul", nombre: "Lote 20",
                    area: 265, frente: 12, fondo: 22, precioM2: 69000, precioLista: 18285000, precioVenta: 17370750, codigo: "LT-004"
                },
                {
                    x: 364.53, y: 719.41, estado: "verde", nombre: "Lote 21",
                    area: 320, frente: 14, fondo: 25, precioM2: 71000, precioLista: 22720000, precioVenta: 21584000, codigo: "LT-005"
                },
                {
                    x: 354.53, y: 699.91, estado: "amarillo", nombre: "Lote 22",
                    area: 300, frente: 11, fondo: 26, precioM2: 67000, precioLista: 20100000, precioVenta: 19095000, codigo: "LT-006"
                },
                {
                    x: 343.02, y: 677.91, estado: "rojo", nombre: "Lote 23",
                    area: 385, frente: 16, fondo: 29, precioM2: 80000, precioLista: 30800000, precioVenta: 29260000, codigo: "LT-007"
                },
                {
                    x: 378.5, y: 614.353120803833, estado: "rojo", nombre: "Lote 24",
                    area: 280, frente: 10, fondo: 24, precioM2: 62000, precioLista: 17360000, precioVenta: 16492000, codigo: "LT-008"
                },
                {
                    x: 398.75, y: 607.603120803833, estado: "rojo", nombre: "Lote 25",
                    area: 315, frente: 11, fondo: 27, precioM2: 76000, precioLista: 23940000, precioVenta: 22743000, codigo: "LT-009"
                },
                {
                    x: 420, y: 599.853120803833, estado: "rojo", nombre: "Lote 26",
                    area: 450, frente: 17, fondo: 32, precioM2: 90000, precioLista: 40500000, precioVenta: 38475000, codigo: "LT-010"
                },
                {
                    x: 442, y: 592.8531246185303, estado: "rojo", nombre: "Lote 27",
                    area: 260, frente: 12, fondo: 23, precioM2: 65000, precioLista: 16900000, precioVenta: 16055000, codigo: "LT-011"
                },
                {
                    x: 462, y: 581.8531246185303, estado: "rojo", nombre: "Lote 28",
                    area: 390, frente: 15, fondo: 30, precioM2: 78000, precioLista: 30420000, precioVenta: 28899000, codigo: "LT-012"
                },
                {
                    x: 486.75, y: 567.8531246185303, estado: "rojo", nombre: "Lote 29",
                    area: 340, frente: 13, fondo: 28, precioM2: 74000, precioLista: 25160000, precioVenta: 23902000, codigo: "LT-013"
                },
                {
                    x: 499.25, y: 550.6031169891357, estado: "rojo", nombre: "Lote 30",
                    area: 305, frente: 12, fondo: 25, precioM2: 69000, precioLista: 21045000, precioVenta: 19992750, codigo: "LT-014"
                },
                {
                    x: 511.5, y: 537.3531169891357, estado: "rojo", nombre: "Lote 31",
                    area: 355, frente: 14, fondo: 29, precioM2: 81000, precioLista: 28755000, precioVenta: 27317250, codigo: "LT-015"
                },
                {
                    x: 524.7500152587891, y: 521.1031227111816, estado: "rojo", nombre: "Lote 32",
                    area: 330, frente: 13, fondo: 28, precioM2: 73000, precioLista: 24090000, precioVenta: 22885500, codigo: "LT-016"
                },
                {
                    x: 537.0000152587891, y: 503.85312271118164, estado: "rojo", nombre: "Lote 33",
                    area: 375, frente: 15, fondo: 30, precioM2: 77000, precioLista: 28875000, precioVenta: 27431250, codigo: "LT-017"
                },
                {
                    x: 546.2500152587891, y: 485.60311126708984, estado: "rojo", nombre: "Lote 34",
                    area: 290, frente: 12, fondo: 22, precioM2: 68000, precioLista: 19720000, precioVenta: 18734000, codigo: "LT-018"
                },
                {
                    x: 560.0000152587891, y: 469.35311126708984, estado: "rojo", nombre: "Lote 35",
                    area: 420, frente: 16, fondo: 31, precioM2: 85000, precioLista: 35700000, precioVenta: 33915000, codigo: "LT-019"
                },
                {
                    x: 571.5000305175781, y: 453.85311126708984, estado: "rojo", nombre: "Lote 36",
                    area: 265, frente: 11, fondo: 23, precioM2: 66000, precioLista: 17490000, precioVenta: 16615500, codigo: "LT-020"
                },
                {
                    x: 581.2500305175781, y: 436.35311126708984, estado: "rojo", nombre: "Lote 37",
                    area: 305, frente: 12, fondo: 25, precioM2: 72000, precioLista: 21960000, precioVenta: 20862000, codigo: "LT-021"
                },
                {
                    x: 596.5000305175781, y: 419.35311126708984, estado: "rojo", nombre: "Lote 38",
                    area: 385, frente: 14, fondo: 28, precioM2: 80000, precioLista: 30800000, precioVenta: 29260000, codigo: "LT-022"
                },
                {
                    x: 606.5000305175781, y: 403.85311126708984, estado: "rojo", nombre: "Lote 39",
                    area: 310, frente: 12, fondo: 26, precioM2: 70000, precioLista: 21700000, precioVenta: 20615000, codigo: "LT-023"
                },
                {
                    x: 617.2500152587891, y: 385.6031093597412, estado: "rojo", nombre: "Lote 40",
                    area: 360, frente: 14, fondo: 29, precioM2: 83000, precioLista: 29880000, precioVenta: 28386000, codigo: "LT-024"
                },
                {
                    x: 628.7500152587891, y: 369.8531093597412, estado: "rojo", nombre: "Lote 41",
                    area: 275, frente: 11, fondo: 22, precioM2: 65000, precioLista: 17875000, precioVenta: 16981250, codigo: "LT-025"
                },
                {
                    x: 640, y: 351.85309982299805, estado: "rojo", nombre: "Lote 42",
                    area: 420, frente: 16, fondo: 30, precioM2: 88000, precioLista: 36960000, precioVenta: 35112000, codigo: "LT-026"
                },
                {
                    x: 651.75, y: 335.10309982299805, estado: "rojo", nombre: "Lote 43",
                    area: 260, frente: 11, fondo: 21, precioM2: 60000, precioLista: 15600000, precioVenta: 14820000, codigo: "LT-027"
                },
                {
                    x: 423.75, y: 662.7062492370605, estado: "verde", nombre: "Lote 44",
                    area: 315, frente: 14, fondo: 25, precioM2: 74000, precioLista: 23310000, precioVenta: 22144500, codigo: "LT-028"
                },
                {
                    x: 436.75, y: 683.2062492370605, estado: "verde", nombre: "Lote 45",
                    area: 340, frente: 13, fondo: 29, precioM2: 78000, precioLista: 26520000, precioVenta: 25194000, codigo: "LT-029"
                },
                {
                    x: 445.25, y: 699.2062492370605, estado: "verde", nombre: "Lote 46",
                    area: 280, frente: 11, fondo: 23, precioM2: 69000, precioLista: 19320000, precioVenta: 18354000, codigo: "LT-030"
                },
                {
                    x: 458.25, y: 716.7062492370605, estado: "verde", nombre: "Lote 47",
                    area: 360, frente: 15, fondo: 30, precioM2: 80000, precioLista: 28800000, precioVenta: 27360000, codigo: "LT-031"
                },
                {
                    x: 464.75, y: 738.706226348877, estado: "verde", nombre: "Lote 48",
                    area: 295, frente: 12, fondo: 22, precioM2: 67000, precioLista: 19765000, precioVenta: 18776750, codigo: "LT-032"
                }
            ]

        },
        mapa2: {
            imagen: "/assets/images/Mapas/Mapa_Condominio_2.jpg",
            lotes: [
                {
                    x: 590.5, y: 214.45624923706055, estado: "verde", nombre: "Lote 1",
                    area: 381, frente: 12.4, fondo: 24.9, precioM2: 54, precioLista: 20610, precioVenta: 18549, codigo: "COD-7213"
                },
                {
                    x: 594, y: 241.95624923706055, estado: "verde", nombre: "Lote 2",
                    area: 412, frente: 14.1, fondo: 25.7, precioM2: 60, precioLista: 24720, precioVenta: 21953, codigo: "COD-8824"
                },
                {
                    x: 608, y: 264.95624923706055, estado: "verde", nombre: "Lote 3",
                    area: 356, frente: 13.3, fondo: 22.6, precioM2: 49, precioLista: 17444, precioVenta: 15700, codigo: "COD-3341"
                },
                {
                    x: 610.5, y: 290.45624923706055, estado: "verde", nombre: "Lote 4",
                    area: 427, frente: 15.6, fondo: 27.1, precioM2: 58, precioLista: 24766, precioVenta: 22500, codigo: "COD-9012"
                },
                {
                    x: 628.5, y: 312.95624923706055, estado: "verde", nombre: "Lote 5",
                    area: 398, frente: 12.9, fondo: 26.4, precioM2: 52, precioLista: 20696, precioVenta: 18626, codigo: "COD-1159"
                },
                {
                    x: 628, y: 332.95624923706055, estado: "verde", nombre: "Lote 6",
                    area: 372, frente: 13.7, fondo: 24.3, precioM2: 55, precioLista: 20460, precioVenta: 18300, codigo: "COD-5528"
                },
                {
                    x: 634.5, y: 354.95624923706055, estado: "verde", nombre: "Lote 7",
                    area: 389, frente: 14.2, fondo: 25.1, precioM2: 50, precioLista: 19450, precioVenta: 17410, codigo: "COD-7410"
                },
                {
                    x: 647.0000305175781, y: 385.45624923706055, estado: "verde", nombre: "Lote 8",
                    area: 420, frente: 15.3, fondo: 28.0, precioM2: 59, precioLista: 24780, precioVenta: 22300, codigo: "COD-2204"
                },
                {
                    x: 655.5000305175781, y: 405.45624923706055, estado: "verde", nombre: "Lote 9",
                    area: 401, frente: 12.5, fondo: 26.7, precioM2: 51, precioLista: 20451, precioVenta: 18300, codigo: "COD-6785"
                },
                {
                    x: 662.5000305175781, y: 426.45624923706055, estado: "verde", nombre: "Lote 10",
                    area: 365, frente: 13.1, fondo: 23.8, precioM2: 47, precioLista: 17155, precioVenta: 15439, codigo: "COD-9912"
                },
                {
                    x: 773.0000610351562, y: 438.4562568664551, estado: "verde", nombre: "Lote 11",
                    area: 410, frente: 12.8, fondo: 25.5, precioM2: 56, precioLista: 22960, precioVenta: 20664, codigo: "COD-8200"
                },
                {
                    x: 766.0000610351562, y: 413.4562568664551, estado: "verde", nombre: "Lote 12",
                    area: 395, frente: 13.6, fondo: 26.2, precioM2: 53, precioLista: 20935, precioVenta: 18841, codigo: "COD-7003"
                },
                {
                    x: 767.0000915527344, y: 386.9562568664551, estado: "verde", nombre: "Lote 13",
                    area: 372, frente: 11.9, fondo: 24.8, precioM2: 48, precioLista: 17856, precioVenta: 16070, codigo: "COD-3340"
                },
                {
                    x: 761.5000915527344, y: 368.4562568664551, estado: "verde", nombre: "Lote 14",
                    area: 388, frente: 12.7, fondo: 24.6, precioM2: 50, precioLista: 19400, precioVenta: 17460, codigo: "COD-6621"
                },
                {
                    x: 755.0000915527344, y: 338.4562568664551, estado: "verde", nombre: "Lote 15",
                    area: 422, frente: 15.1, fondo: 27.4, precioM2: 57, precioLista: 24054, precioVenta: 21649, codigo: "COD-5599"
                },
                {
                    x: 742.0000915527344, y: 318.9562568664551, estado: "verde", nombre: "Lote 16",
                    area: 409, frente: 14.8, fondo: 25.9, precioM2: 52, precioLista: 21268, precioVenta: 18900, codigo: "COD-7710"
                },
                {
                    x: 733.5000915527344, y: 299.4562568664551, estado: "verde", nombre: "Lote 17",
                    area: 355, frente: 12.4, fondo: 23.2, precioM2: 49, precioLista: 17395, precioVenta: 15655, codigo: "COD-2290"
                },
                {
                    x: 728.5000915527344, y: 272.4562644958496, estado: "verde", nombre: "Lote 18",
                    area: 382, frente: 13.2, fondo: 24.7, precioM2: 54, precioLista: 20628, precioVenta: 18565, codigo: "COD-3347"
                },
                {
                    x: 731.0000915527344, y: 251.4562644958496, estado: "verde", nombre: "Lote 19",
                    area: 420, frente: 15.0, fondo: 26.9, precioM2: 59, precioLista: 24780, precioVenta: 22300, codigo: "COD-9901"
                }
            ]
        },
        mapa3: {
            imagen: "/assets/images/Mapas/Mapa_Condominio_3.png",
            lotes: [
                {
                    x: 587.5000305175781, y: 467.4562339782715, estado: "amarillo", nombre: "Lote 1",
                    area: 321, frente: 12, fondo: 27, precioM2: 52000,
                    precioLista: 16692000, precioVenta: 15787440, codigo: "L-001"
                },

                {
                    x: 586.5000305175781, y: 489.4562339782715, estado: "azul", nombre: "Lote 2",
                    area: 410, frente: 15, fondo: 30, precioM2: 60000,
                    precioLista: 24600000, precioVenta: 22788000, codigo: "L-002"
                },

                {
                    x: 588.0000305175781, y: 507.9562339782715, estado: "rojo", nombre: "Lote 3",
                    area: 280, frente: 11, fondo: 22, precioM2: 48000,
                    precioLista: 13440000, precioVenta: 12184800, codigo: "L-003"
                },

                {
                    x: 590.0000305175781, y: 523.9562339782715, estado: "verde", nombre: "Lote 4",
                    area: 350, frente: 14, fondo: 28, precioM2: 55000,
                    precioLista: 19250000, precioVenta: 17212500, codigo: "L-004"
                },

                {
                    x: 590.0000305175781, y: 543.4562339782715, estado: "azul", nombre: "Lote 5",
                    area: 260, frente: 10, fondo: 21, precioM2: 47000,
                    precioLista: 12220000, precioVenta: 11364600, codigo: "L-005"
                },

                {
                    x: 592.0000305175781, y: 560.4562339782715, estado: "rojo", nombre: "Lote 6",
                    area: 390, frente: 17, fondo: 31, precioM2: 69000,
                    precioLista: 26910000, precioVenta: 24433200, codigo: "L-006"
                },

                {
                    x: 595.0000305175781, y: 580.4562339782715, estado: "amarillo", nombre: "Lote 7",
                    area: 300, frente: 12, fondo: 25, precioM2: 52000,
                    precioLista: 15600000, precioVenta: 14196000, codigo: "L-007"
                },

                {
                    x: 594.0000305175781, y: 598.4562339782715, estado: "verde", nombre: "Lote 8",
                    area: 420, frente: 18, fondo: 32, precioM2: 65000,
                    precioLista: 27300000, precioVenta: 24723000, codigo: "L-008"
                },

                {
                    x: 595.5000305175781, y: 617.4562339782715, estado: "rojo", nombre: "Lote 9",
                    area: 310, frente: 13, fondo: 24, precioM2: 56000,
                    precioLista: 17360000, precioVenta: 15689600, codigo: "L-009"
                },

                {
                    x: 596.5000305175781, y: 637.9562339782715, estado: "azul", nombre: "Lote 10",
                    area: 285, frente: 11, fondo: 23, precioM2: 51000,
                    precioLista: 14535000, precioVenta: 13417500, codigo: "L-010"
                },

                {
                    x: 525.0000305175781, y: 643.4562339782715, estado: "rojo", nombre: "Lote 11",
                    area: 350, frente: 14, fondo: 28, precioM2: 60000,
                    precioLista: 21000000, precioVenta: 19320000, codigo: "L-011"
                },

                {
                    x: 525.0000305175781, y: 624.4562339782715, estado: "verde", nombre: "Lote 12",
                    area: 230, frente: 10, fondo: 19, precioM2: 48000,
                    precioLista: 11040000, precioVenta: 10156800, codigo: "L-012"
                },

                {
                    x: 523.0000305175781, y: 605.9562339782715, estado: "amarillo", nombre: "Lote 13",
                    area: 260, frente: 11, fondo: 20, precioM2: 52000,
                    precioLista: 13520000, precioVenta: 12438400, codigo: "L-013"
                },

                {
                    x: 516.5000152587891, y: 584.9562492370605, estado: "azul", nombre: "Lote 14",
                    area: 330, frente: 15, fondo: 27, precioM2: 59000,
                    precioLista: 19470000, precioVenta: 17813400, codigo: "L-014"
                },

                {
                    x: 520.0000152587891, y: 568.9562492370605, estado: "rojo", nombre: "Lote 15",
                    area: 290, frente: 12, fondo: 22, precioM2: 55000,
                    precioLista: 15950000, precioVenta: 14674000, codigo: "L-015"
                },

                {
                    x: 519.0000152587891, y: 551.4562492370605, estado: "verde", nombre: "Lote 16",
                    area: 310, frente: 14, fondo: 24, precioM2: 53000,
                    precioLista: 16430000, precioVenta: 15117900, codigo: "L-016"
                },

                {
                    x: 519, y: 533.4562568664551, estado: "amarillo", nombre: "Lote 17",
                    area: 240, frente: 10, fondo: 20, precioM2: 49000,
                    precioLista: 11760000, precioVenta: 10936800, codigo: "L-017"
                },

                {
                    x: 516, y: 507.4562568664551, estado: "azul", nombre: "Lote 18",
                    area: 300, frente: 13, fondo: 23, precioM2: 58000,
                    precioLista: 17400000, precioVenta: 16182000, codigo: "L-018"
                },

                {
                    x: 573.9999694824219, y: 413.95624923706055, estado: "verde", nombre: "Lote 19",
                    area: 360, frente: 15, fondo: 29, precioM2: 60000,
                    precioLista: 21600000, precioVenta: 19872000, codigo: "L-019"
                },

                {
                    x: 594.9999694824219, y: 408.95624923706055, estado: "rojo", nombre: "Lote 20",
                    area: 250, frente: 12, fondo: 20, precioM2: 52000,
                    precioLista: 13000000, precioVenta: 11830000, codigo: "L-020"
                }
            ]

        }
    };

    let overlayActual = null;

    function limpiarMapa() {
        markers.forEach(m => map.removeLayer(m));
        markers = [];
    }

    function cargarMapa(nombreMapa) {

        const data = mapas[nombreMapa];

        limpiarMapa();

        if (!data) return;

        if (overlayActual) map.removeLayer(overlayActual);

        markers.forEach(m => map.removeLayer(m));
        markers = [];

        overlayActual = L.imageOverlay(data.imagen, bounds).addTo(map);

        data.lotes.forEach(lote => {
            const marcador = crearPunto(lote);
            markers.push(marcador);
        });
    }

    actualizarSelector();

    // Para obtener coordenadas
    map.on("click", (e) => {
        console.log("x:", e.latlng.lng, "y:", e.latlng.lat);
    });
});