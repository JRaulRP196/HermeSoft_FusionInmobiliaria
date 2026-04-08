document.addEventListener("DOMContentLoaded", function () {

    $(document).on("stepChanged", function (e, step) {

        if (step === 4) {
            generarResumen("resumenContainer");
        }

    });

    function generarResumen(containerId) {
        const container = document.getElementById(containerId);
        container.innerHTML = "";

        const elementos = document.querySelectorAll("[data-resumen]");

        const secciones = {
            "Datos": [],
            "Financiero": [],
            "Otros": []
        };

        elementos.forEach(el => {
            let label = el.getAttribute("data-resumen");
            let valor = "";

            if (el.tagName === "SELECT") {
                valor = el.options[el.selectedIndex]?.text || "";
            } else {
                valor = el.value;
            }

            if (!valor || valor.trim() === "") return;

            if (label.includes("Cliente") || label.includes("Lote") || label.includes("Banco")) {
                secciones["Datos"].push({ label, valor });
            } else if (label.includes("Ingreso") || label.includes("Gasto")) {
                secciones["Financiero"].push({ label, valor });
            } else {
                secciones["Otros"].push({ label, valor });
            }
        });

        Object.keys(secciones).forEach(seccion => {
            if (secciones[seccion].length === 0) return;

            const bloque = document.createElement("div");
            bloque.className = "mb-4";

            bloque.innerHTML = `
                        <h6 class="fw-bold text-primary mb-3">
                            <i class="bi bi-folder2-open me-2"></i>${seccion}
                        </h6>
                    `;

            secciones[seccion].forEach(item => {
                const fila = document.createElement("div");
                fila.className = "d-flex justify-content-between align-items-center mb-2 p-2 rounded-3 bg-light";

                fila.innerHTML = `
                            <span class="text-muted small">${item.label}</span>
                            <span class="fw-semibold">${item.valor}</span>
                        `;

                bloque.appendChild(fila);
            });

            container.appendChild(bloque);
        });
    }

});


