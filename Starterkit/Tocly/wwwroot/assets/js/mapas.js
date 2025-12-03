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

    function crearPunto(x, y, color, texto) {
        return L.circleMarker([y, x], {
            radius: 14,
            color: color,
            fillColor: color,
            fillOpacity: 0.9
        }).addTo(map).bindPopup(texto);
    }

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
                { x: 359.53, y: 803.91, estado: "verde", nombre: "Lote 12" },
                    { x: 374.53, y: 785.91, estado: "amarillo", nombre: "Lote 14" },
                    { x: 402.53, y: 770.91, estado: "rojo", nombre: "Lote 15" },
                    { x: 379.03, y: 740.91, estado: "azul", nombre: "Lote 20" },
                    { x: 364.53, y: 719.41, estado: "verde", nombre: "Lote 21" },
                    { x: 354.53, y: 699.91, estado: "amarillo", nombre: "Lote 22" },
                    { x: 343.02, y: 677.91, estado: "rojo", nombre: "Lote 23" },
                    { x: 378.5, y: 614.353120803833, estado: "rojo", nombre: "Lote 24" },
                    { x: 398.75, y: 607.603120803833, estado: "rojo", nombre: "Lote 25" },
                    { x: 420, y: 599.853120803833, estado: "rojo", nombre: "Lote 26" },
                    { x: 442, y: 592.8531246185303, estado: "rojo", nombre: "Lote 27" },
                    { x: 462, y: 581.8531246185303, estado: "rojo", nombre: "Lote 28" },
                    { x: 486.75, y: 567.8531246185303, estado: "rojo", nombre: "Lote 29" },
                    { x: 499.25, y: 550.6031169891357, estado: "rojo", nombre: "Lote 30" },
                    { x: 511.5, y: 537.3531169891357, estado: "rojo", nombre: "Lote 31" },
                    { x: 524.7500152587891, y: 521.1031227111816, estado: "rojo", nombre: "Lote 32" },
                    { x: 537.0000152587891, y: 503.85312271118164, estado: "rojo", nombre: "Lote 33" },
                    { x: 546.2500152587891, y: 485.60311126708984, estado: "rojo", nombre: "Lote 34" },
                    { x: 560.0000152587891, y: 469.35311126708984, estado: "rojo", nombre: "Lote 35" },
                    { x: 571.5000305175781, y: 453.85311126708984, estado: "rojo", nombre: "Lote 36" },
                    { x: 581.2500305175781, y: 436.35311126708984, estado: "rojo", nombre: "Lote 37" },
                    { x: 596.5000305175781, y: 419.35311126708984, estado: "rojo", nombre: "Lote 38" },
                    { x: 606.5000305175781, y: 403.85311126708984, estado: "rojo", nombre: "Lote 39" },
                    { x: 617.2500152587891, y: 385.6031093597412, estado: "rojo", nombre: "Lote 40" },
                    { x: 628.7500152587891, y: 369.8531093597412, estado: "rojo", nombre: "Lote 41" },
                    { x: 640, y: 351.85309982299805, estado: "rojo", nombre: "Lote 42" },
                    { x: 651.75, y: 335.10309982299805, estado: "rojo", nombre: "Lote 43" },
                    { x: 423.75, y: 662.7062492370605, estado: "verde", nombre: "Lote 44" },
                    { x: 436.75, y: 683.2062492370605, estado: "verde", nombre: "Lote 45" },
                    { x: 445.25, y: 699.2062492370605, estado: "verde", nombre: "Lote 46" },
                    { x: 458.25, y: 716.7062492370605, estado: "verde", nombre: "Lote 47" },
                    { x: 464.75, y: 738.706226348877, estado: "verde", nombre: "Lote 48" }
            ]
        },
        mapa2: {
            imagen: "/assets/images/Mapas/Mapa_Condominio_2.jpg",
            lotes: [
                { x: 590.5, y: 214.45624923706055, estado: "verde", nombre: "Lote 1" },
                { x: 594, y: 241.95624923706055, estado: "verde", nombre: "Lote 2" },
                { x: 608, y: 264.95624923706055, estado: "verde", nombre: "Lote 3" },
                { x: 610.5, y: 290.45624923706055, estado: "verde", nombre: "Lote 4" },
                { x: 628.5, y: 312.95624923706055, estado: "verde", nombre: "Lote 5" },
                { x: 628, y: 332.95624923706055, estado: "verde", nombre: "Lote 6" },
                { x: 634.5, y: 354.95624923706055, estado: "verde", nombre: "Lote 7" },
                { x: 647.0000305175781, y: 385.45624923706055, estado: "verde", nombre: "Lote 8" },
                { x: 655.5000305175781, y: 405.45624923706055, estado: "verde", nombre: "Lote 9" },
                { x: 662.5000305175781, y: 426.45624923706055, estado: "verde", nombre: "Lote 10" },
                { x: 773.0000610351562, y: 438.4562568664551, estado: "verde", nombre: "Lote 11" },
                { x: 766.0000610351562, y: 413.4562568664551, estado: "verde", nombre: "Lote 12" },
                { x: 767.0000915527344, y: 386.9562568664551, estado: "verde", nombre: "Lote 13" },
                { x: 761.5000915527344, y: 368.4562568664551, estado: "verde", nombre: "Lote 14" },
                { x: 755.0000915527344, y: 338.4562568664551, estado: "verde", nombre: "Lote 15" },
                { x: 742.0000915527344, y: 318.9562568664551, estado: "verde", nombre: "Lote 16" },
                { x: 733.5000915527344, y: 299.4562568664551, estado: "verde", nombre: "Lote 17" },
                { x: 728.5000915527344, y: 272.4562644958496, estado: "verde", nombre: "Lote 18" },
                { x: 731.0000915527344, y: 251.4562644958496, estado: "verde", nombre: "Lote 19" },
                { x: 655.4999694824219, y: 447.9562339782715, estado: "azul", nombre: "Lote 43" },
                { x: 647.9999694824219, y: 465.9562339782715, estado: "azul", nombre: "Lote 44" },
                { x: 620.4999694824219, y: 484.4562339782715, estado: "azul", nombre: "Lote 45" },
                { x: 602.9999694824219, y: 507.4562339782715, estado: "azul", nombre: "Lote 46" },
                { x: 587.4999694824219, y: 525.9562492370605, estado: "azul", nombre: "Lote 47" },
                { x: 566.9999694824219, y: 540.9562492370605, estado: "azul", nombre: "Lote 48" },
                { x: 551.4999694824219, y: 563.4562492370605, estado: "azul", nombre: "Lote 49" },
                { x: 684.4999694824219, y: 549.956241607666, estado: "azul", nombre: "Lote 50" },
                { x: 718.4999694824219, y: 537.956241607666, estado: "azul", nombre: "Lote 51" },
                { x: 745.4999694824219, y: 532.956241607666, estado: "azul", nombre: "Lote 52" },
                { x: 778.4999694824219, y: 528.956241607666, estado: "azul", nombre: "Lote 53" },
                { x: 811.9999694824219, y: 525.456241607666, estado: "azul", nombre: "Lote 54" },
                { x: 834.4999694824219, y: 520.456241607666, estado: "azul", nombre: "Lote 55" }
            ]
        },
        mapa3: {
            imagen: "/assets/images/Mapas/Mapa_Condominio_3.png",
            lotes: [
                { x: 587.5000305175781, y: 467.4562339782715, estado: "amarillo", nombre: "Lote 1" },
                { x: 586.5000305175781, y: 489.4562339782715, estado: "azul", nombre: "Lote 2" },
                { x: 588.0000305175781, y: 507.9562339782715, estado: "rojo", nombre: "Lote 3" },
                { x: 590.0000305175781, y: 523.9562339782715, estado: "verde", nombre: "Lote 4" },
                { x: 590.0000305175781, y: 543.4562339782715, estado: "azul", nombre: "Lote 5" },
                { x: 592.0000305175781, y: 560.4562339782715, estado: "rojo", nombre: "Lote 6" },
                { x: 595.0000305175781, y: 580.4562339782715, estado: "amarillo", nombre: "Lote 7" },
                { x: 594.0000305175781, y: 598.4562339782715, estado: "verde", nombre: "Lote 8" },
                { x: 595.5000305175781, y: 617.4562339782715, estado: "rojo", nombre: "Lote 9" },
                { x: 596.5000305175781, y: 637.9562339782715, estado: "azul", nombre: "Lote 10" },
                { x: 525.0000305175781, y: 643.4562339782715, estado: "rojo", nombre: "Lote 11" },
                { x: 525.0000305175781, y: 624.4562339782715, estado: "verde", nombre: "Lote 12" },
                { x: 523.0000305175781, y: 605.9562339782715, estado: "amarillo", nombre: "Lote 13" },
                { x: 516.5000152587891, y: 584.9562492370605, estado: "azul", nombre: "Lote 14" },
                { x: 520.0000152587891, y: 568.9562492370605, estado: "rojo", nombre: "Lote 15" },
                { x: 519.0000152587891, y: 551.4562492370605, estado: "verde", nombre: "Lote 16" },
                { x: 519, y: 533.4562568664551, estado: "amarillo", nombre: "Lote 17" },
                { x: 516, y: 507.4562568664551, estado: "azul", nombre: "Lote 18" },
                { x: 573.9999694824219, y: 413.95624923706055, estado: "verde", nombre: "Lote 19" },
                { x: 594.9999694824219, y: 408.95624923706055, estado: "rojo", nombre: "Lote 20" }
            ]
        }
    };

    let overlayActual = null;
    let markers = [];

    function cargarMapa(nombreMapa) {

        const data = mapas[nombreMapa];
        if (!data) return;

        if (overlayActual) map.removeLayer(overlayActual);

        markers.forEach(m => map.removeLayer(m));
        markers = [];

        overlayActual = L.imageOverlay(data.imagen, bounds).addTo(map);

        data.lotes.forEach(lote => {
            const marcador = crearPunto(lote.x, lote.y, colores[lote.estado], lote.nombre);
            markers.push(marcador);
        });
    }

    actualizarSelector();

    // Para obtener coordenadas
    map.on("click", (e) => {
        console.log("x:", e.latlng.lng, "y:", e.latlng.lat);
    });
});