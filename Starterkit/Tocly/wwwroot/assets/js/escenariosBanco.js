document.addEventListener("DOMContentLoaded", function () {

    var botonesEscalonada =
        "<button type='button' class='btn btn-primary addInputs'>+</button> " +
        "<button type='button' class='btn btn-danger deleteInputs'>-</button>";

    function setScenarioIndex($esc, i) {
        // Actualiza data-index
        $esc.attr("data-index", i);

        // Cambia todos los name="escenariosTasa[...].campo"
        $esc.find("[name]").each(function () {
            let name = $(this).attr("name");

            // Reemplaza placeholder __i__
            name = name.replaceAll("__i__", i);

            // Reemplaza escenariosTasa[<numero>] por escenariosTasa[i]
            name = name.replace(/escenariosTasa\[\d+\]/g, "escenariosTasa[" + i + "]");

            $(this).attr("name", name);
        });

        // Título visual
        $esc.find(".card-title").first().text("Escenario #" + (i + 1));
    }

    function reindexarEscenarios() {
        $("#listaEscenarios .escenarios").each(function (idx) {
            setScenarioIndex($(this), idx);
        });
    }

    // Agregar escenario
    $("#addEscenario").on("click", function () {
        let count = $("#listaEscenarios .escenarios").length; // siguiente índice

        let nuevo = $("#escenario-template").clone();
        nuevo.removeAttr("id");
        nuevo.show();

        // Aplica index correcto
        setScenarioIndex(nuevo, count);

        // Animación
        nuevo.css({ opacity: 0, display: "none" });
        $("#listaEscenarios").append(nuevo);
        nuevo.slideDown(200).animate({ opacity: 1 }, { queue: false, duration: 300 });
    });

    // Eliminar escenario
    $(document).on("click", ".deleteEscenario", function () {
        let card = $(this).closest(".escenarios");

        card.animate({ opacity: 0 }, 200)
            .slideUp(250, function () {
                card.remove();
                reindexarEscenarios();
            });
    });

    // Detecta si es escalonada (por texto, no por value)
    $(document).on("change", ".tipoInteres", function () {
        var txt = $(this).find("option:selected").text().toLowerCase();
        var card = $(this).closest(".card-body");
        var contenedor = card.find(".botonesEscalonada");

        if (txt.includes("escalonad")) {
            contenedor.html(botonesEscalonada);
        } else {
            contenedor.html("");
            let bloques = card.find(".inputsEscenario");
            bloques.not(":first").remove();
        }
    });

    // Agregar bloque escalonado (duplica inputsEscenario)
    $(document).on("click", ".addInputs", function () {
        let card = $(this).closest(".card-body");
        let original = card.find(".inputsEscenario").first();
        let clon = original.clone();

        clon.find("input").val("");
        clon.css({ opacity: 0, display: "none" });

        card.find(".inputsEscenario").last().after(clon);
        clon.slideDown(200).animate({ opacity: 1 }, { queue: false, duration: 300 });
    });

    // Eliminar bloque escalonado
    $(document).on("click", ".deleteInputs", function () {
        let card = $(this).closest(".card-body");
        let bloques = card.find(".inputsEscenario");
        if (bloques.length > 1) {
            let ultimo = bloques.last();
            ultimo.animate({ opacity: 0 }, 200)
                .slideUp(250, function () {
                    ultimo.remove();
                });
        }
    });

    // Asegura index correcto del primer escenario que ya existe (0)
    reindexarEscenarios();
});
