// Lista de condominios y mapas asociados
const condominios = [
    { nombre: "Condominio A", mapa: "world_mill_en" },
    { nombre: "Condominio B", mapa: "us_merc_en" },
    { nombre: "Condominio C", mapa: "in_mill_en" }
];

let index = 0;

// ---- Animación suave ----
function actualizarNombre() {
    const label = $("#nombreCondominio");

    label.css("opacity", "0");

    setTimeout(() => {
        label.text(condominios[index].nombre);
        label.css("opacity", "1");
    }, 300);
}

// ---- Cambiar mapa ----
function cargarMapa() {

    // Destruir mapa anterior si existe
    $("#chicago-vectormap").empty();

    const mapa = condominios[index].mapa;

    $("#chicago-vectormap").vectorMap({
        map: mapa,
        backgroundColor: "transparent",
        regionStyle: {
            initial: {
                fill: "#4b7bec"
            }
        }
    });
}

// ---- Evento botones ----
$("#btnPrevCondominio").on("click", () => {
    index = (index - 1 + condominios.length) % condominios.length;
    actualizarNombre();
    cargarMapa();
});

$("#btnNextCondominio").on("click", () => {
    index = (index + 1) % condominios.length;
    actualizarNombre();
    cargarMapa();
});

// ---- Inicializar ----
$(document).ready(() => {
    actualizarNombre();
    cargarMapa();
});
