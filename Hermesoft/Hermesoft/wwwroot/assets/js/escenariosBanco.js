$(function () {


    var botonesEscalonada = "<button type = 'button' class='btn btn-primary addInputs'>+</button>"+
        "<button type='button' class='btn btn-danger deleteInputs'>-</button> ";
    var contadorEscenarios = 1;

    function reindexarTodo() {
        $("#listaEscenarios .escenarios").each(function (i) {

            // Reindexa escenario
            $(this).find("input[name], select[name]").each(function () {
                let name = $(this).attr("name");

                name = name.replace(
                    /EscenariosTasaInteres\[\d+\]/,
                    `EscenariosTasaInteres[${i}]`
                );

                $(this).attr("name", name);
            });

            // Reindexa plazos dentro del escenario
            $(this).find(".inputsEscenario").each(function (j) {
                $(this).find("input, select").each(function () {
                    let name = $(this).attr("name");

                    name = name.replace(
                        /PlazosEscenarios\[\d+\]/,
                        `PlazosEscenarios[${j}]`
                    );

                    $(this).attr("name", name);
                });
            });
        });
    }


    $("#addEscenario").on("click", function () {
        contadorEscenarios++;
        let nuevo = $("#escenario-template").clone();
        nuevo.removeAttr("id");
        nuevo.show();
        nuevo.find(".card-title").text("Escenario #" + contadorEscenarios);
        nuevo.css({ opacity: 0, display: "none" });
        $("#listaEscenarios").append(nuevo);
        nuevo.slideDown(200).animate({ opacity: 1 }, { queue: false, duration: 300 });
        reindexarTodo();
    });

    $(document).on("click", ".deleteEscenario", function () {
        let card = $(this).closest(".escenarios");

        card.animate({ opacity: 0 }, 200)
            .slideUp(250, function () {
                card.remove();
                reindexarEscenarios();
                reindexarTodo();
            });
    });

    function reindexarEscenarios() {
        contadorEscenarios = 0;

        $("#listaEscenarios .escenarios").each(function () {
            contadorEscenarios++;
            $(this).find(".card-title").text("Escenario #" + contadorEscenarios);
        });
    }
    function manejarTipoInteres(select) {
        var valor = select.val();
        var card = select.closest(".card-body");
        var contenedor = card.find(".botonesEscalonada");

        if (valor == 2) {
            contenedor.html(botonesEscalonada);
        } else {
            contenedor.html("");
            let bloques = card.find(".inputsEscenario");
            bloques.not(":first").remove();
        }
    }

    $(document).on("change", ".tipoInteres", function () {
        manejarTipoInteres($(this));
    });

    $(".tipoInteres").each(function () {
        manejarTipoInteres($(this));
    });

    $(document).on("click", ".addInputs", function () {
        let card = $(this).closest(".card-body");
        let original = card.find(".inputsEscenario").first();
        let clon = original.clone();
        clon.find("input").val("");
        clon.find("select").val("1");
        clon.css({ opacity: 0, display: "none" });
        card.find(".inputsEscenario").last().after(clon);
        clon.slideDown(200).animate({ opacity: 1 }, { queue: false, duration: 300 });
        reindexarTodo();
    });

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
        reindexarTodo();
    });
    reindexarEscenarios();
});