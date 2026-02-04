document.addEventListener("DOMContentLoaded", function () {

    const botonesEscalonada = `
        <button type="button" class="btn btn-primary addInputs">+</button>
        <button type="button" class="btn btn-danger deleteInputs">-</button>
    `;

    // Agregar escenario (usa tu template, no cambia UI)
    $("#addEscenario").on("click", function () {
        const nuevo = $("#escenario-template").clone();
        nuevo.removeAttr("id").show();

        // título (#)
        const total = $("#listaEscenarios .escenarios").length + 1;
        nuevo.find(".card-title").text("Escenario #" + total);

        // limpiar
        nuevo.find("input").val("");
        nuevo.find("select").each(function () {
            const first = $(this).find("option:first").val();
            $(this).val(first);
        });

        // dejar solo 1 tramo inicial
        const bloques = nuevo.find(".inputsEscenario");
        bloques.not(":first").remove();

        // botones escalonada inicialmente vacíos
        nuevo.find(".botonesEscalonada").html("");

        $("#listaEscenarios").append(nuevo);
    });

    // Eliminar escenario
    $(document).on("click", ".deleteEscenario", function () {
        $(this).closest(".escenarios").remove();
        reindexarEscenarios();
    });

    function reindexarEscenarios() {
        let i = 0;
        $("#listaEscenarios .escenarios .card-title").each(function () {
            i++;
            $(this).text("Escenario #" + i);
        });
    }

    // Cambio tipo tasa: si escalonada -> mostrar botones +/-
    $(document).on("change", ".tipoInteres", function () {
        const tipo = $(this).val();
        const card = $(this).closest(".card-body");
        const contenedor = card.find(".botonesEscalonada");

        if (tipo === "Tasa_Escalonada") {
            contenedor.html(botonesEscalonada);
        } else {
            // Tasa Variable: solo 1 tramo
            contenedor.html("");
            const bloques = card.find(".inputsEscenario");
            bloques.not(":first").remove();
        }
    });

    // Agregar tramo (solo escalonada)
    $(document).on("click", ".addInputs", function () {
        const card = $(this).closest(".card-body");
        const original = card.find(".inputsEscenario").first();
        const clon = original.clone();

        clon.find("input").val("");
        clon.find("select").each(function () {
            const first = $(this).find("option:first").val();
            $(this).val(first);
        });

        card.find(".inputsEscenario").last().after(clon);
    });

    // Quitar tramo (si hay > 1)
    $(document).on("click", ".deleteInputs", function () {
        const card = $(this).closest(".card-body");
        const bloques = card.find(".inputsEscenario");
        if (bloques.length > 1) {
            bloques.last().remove();
        }
    });

    // Armar JSON + Validar antes de enviar
    $("form").on("submit", function (e) {
        const escenarios = [];
        const errores = [];

        $("#listaEscenarios .escenarios .card-body").each(function (idx) {
            const card = $(this);

            const tipoTasa = card.find(".tipoInteres").val();
            const nombre = (card.find(".nombreEscenario").val() || "").trim();

            if (!nombre) {
                errores.push(`Escenario #${idx + 1}: Falta el nombre del escenario.`);
            }

            const tramos = [];
            card.find(".inputsEscenario").each(function (t) {
                const bloque = $(this);

                const plazo = parseInt(bloque.find(".tramoPlazo").val(), 10);
                const adicional = parseFloat(bloque.find(".tramoAdicional").val());
                const indicador = bloque.find(".tramoIndicador").val();

                if (!plazo || plazo <= 0) {
                    errores.push(`Escenario #${idx + 1}, tramo #${t + 1}: Plazo inválido.`);
                }
                if (isNaN(adicional)) {
                    errores.push(`Escenario #${idx + 1}, tramo #${t + 1}: % adicional requerido.`);
                }
                if (!indicador) {
                    errores.push(`Escenario #${idx + 1}, tramo #${t + 1}: Indicador requerido.`);
                }

                tramos.push({
                    plazo: plazo,
                    porcentajeAdicional: adicional,
                    indicador: indicador
                });
            });

            // Reglas principales
            if (tipoTasa === "Tasa_Variable" && tramos.length !== 1) {
                errores.push(`Escenario #${idx + 1}: Tasa Variable SOLO permite 1 tramo.`);
            }
            if (tipoTasa === "Tasa_Escalonada" && tramos.length < 1) {
                errores.push(`Escenario #${idx + 1}: Tasa Escalonada requiere al menos 1 tramo.`);
            }

            escenarios.push({
                tipoTasa: tipoTasa,
                nombre: nombre,
                tramos: tramos
            });
        });

        if (escenarios.length === 0) {
            errores.push("Debe existir al menos 1 escenario.");
        }

        if (errores.length > 0) {
            e.preventDefault();
            alert("Corrige estos errores:\n\n" + errores.join("\n"));
            return;
        }

        $("#escenariosJson").val(JSON.stringify(escenarios));
    });
});
