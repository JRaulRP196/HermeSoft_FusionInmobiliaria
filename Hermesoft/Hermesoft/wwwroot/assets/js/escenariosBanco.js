$(function () {
    var botonesEscalonada = "<button type='button' class='btn btn-primary addInputs'>+</button>" +
        "<button type='button' class='btn btn-danger deleteInputs'>-</button>";
    var contadorEscenarios = 1;
    var escenarioPendienteEliminar = null;

    function crearModalConfirmacion() {
        if ($("#confirmDeleteEscenarioModal").length) {
            return;
        }

        var modalHtml = `
            <div class="modal fade" id="confirmDeleteEscenarioModal" tabindex="-1" aria-labelledby="confirmDeleteEscenarioLabel" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="confirmDeleteEscenarioLabel">Eliminar escenario</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                        </div>
                        <div class="modal-body">
                            Esta accion eliminara el escenario del formulario. Desea continuar?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-danger" id="confirmDeleteEscenarioBtn">Eliminar</button>
                        </div>
                    </div>
                </div>
            </div>`;

        $("body").append(modalHtml);
    }

    function reindexarTodo() {
        $("#listaEscenarios .escenarios").each(function (i) {
            $(this).find("input[name], select[name]").each(function () {
                var name = $(this).attr("name");
                if (!name) return;

                name = name.replace(
                    /EscenariosTasaInteres\[\d+\]/,
                    "EscenariosTasaInteres[" + i + "]"
                );

                $(this).attr("name", name);
            });

            $(this).find(".inputsEscenario").each(function (j) {
                $(this).find("input[name], select[name]").each(function () {
                    var name = $(this).attr("name");
                    if (!name) return;

                    name = name.replace(
                        /PlazosEscenarios\[\d+\]/,
                        "PlazosEscenarios[" + j + "]"
                    );

                    $(this).attr("name", name);
                });
            });
        });
    }

    function reindexarEscenarios() {
        contadorEscenarios = 0;

        $("#listaEscenarios .escenarios").each(function () {
            contadorEscenarios++;
            $(this).find(".card-title").first().text("Escenario #" + contadorEscenarios);
        });
    }

    function inicializarPopovers(contexto) {
        var elementos = (contexto || document).querySelectorAll('[data-bs-toggle="popover"]');
        elementos.forEach(function (elemento) {
            bootstrap.Popover.getOrCreateInstance(elemento);
        });
    }

    function actualizarAccionesEscenario() {
        var escenarios = $("#listaEscenarios .escenarios");
        var total = escenarios.length;

        escenarios.each(function (index) {
            var cardBody = $(this).find(".card-body").first();
            var title = cardBody.find(".card-title").first();
            var acciones = cardBody.find(".escenario-actions");

            cardBody.children(".deleteEscenario").not(".escenario-actions .deleteEscenario").remove();

            if (!acciones.length) {
                acciones = $("<div class='escenario-actions d-flex justify-content-end gap-2 mb-3'></div>");
                acciones.append("<button type='button' class='btn btn-sm btn-outline-primary addEscenarioInline' title='Agregar escenario'>+</button>");
                acciones.append("<button type='button' class='btn btn-sm btn-outline-danger deleteEscenario' title='Eliminar escenario'>Eliminar</button>");
                title.after(acciones);
            }

            acciones.find(".addEscenarioInline").toggle(index === total - 1);
            acciones.find(".deleteEscenario").toggle(total > 1);
        });
    }

    function agregarEscenario() {
        contadorEscenarios++;
        var nuevo = $("#escenario-template").clone();
        nuevo.removeAttr("id");
        nuevo.show();
        nuevo.find(".card-title").first().text("Escenario #" + contadorEscenarios);
        nuevo.find(".deleteEscenario").remove();
        nuevo.css({ opacity: 0, display: "none" });
        $("#listaEscenarios").append(nuevo);
        nuevo.slideDown(200).animate({ opacity: 1 }, { queue: false, duration: 300 });
        inicializarPopovers(nuevo[0]);

        reindexarTodo();
        reindexarEscenarios();
        actualizarAccionesEscenario();
    }

    function eliminarEscenario(card) {
        card.animate({ opacity: 0 }, 200)
            .slideUp(250, function () {
                card.remove();
                reindexarEscenarios();
                reindexarTodo();
                actualizarAccionesEscenario();
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
            var bloques = card.find(".inputsEscenario");
            bloques.not(":first").remove();
        }
    }

    crearModalConfirmacion();
    inicializarPopovers(document);
    $("#addEscenario").hide();

    $(document).on("click", "#addEscenario, .addEscenarioInline", function () {
        agregarEscenario();
    });

    $(document).on("click", ".deleteEscenario", function () {
        escenarioPendienteEliminar = $(this).closest(".escenarios");
        var modalElement = document.getElementById("confirmDeleteEscenarioModal");
        var modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();
    });

    $(document).on("click", "#confirmDeleteEscenarioBtn", function () {
        if (!escenarioPendienteEliminar || !escenarioPendienteEliminar.length) {
            return;
        }

        var modalElement = document.getElementById("confirmDeleteEscenarioModal");
        var modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.hide();

        eliminarEscenario(escenarioPendienteEliminar);
        escenarioPendienteEliminar = null;
    });

    $(document).on("hidden.bs.modal", "#confirmDeleteEscenarioModal", function () {
        escenarioPendienteEliminar = null;
    });

    $(document).on("change", ".tipoInteres", function () {
        manejarTipoInteres($(this));
    });

    $(".tipoInteres").each(function () {
        manejarTipoInteres($(this));
    });

    $(document).on("click", ".addInputs", function () {
        var card = $(this).closest(".card-body");
        var original = card.find(".inputsEscenario").first();
        var clon = original.clone();

        clon.find("input").val("");
        clon.find("select").val("1");
        clon.css({ opacity: 0, display: "none" });

        card.find(".inputsEscenario").last().after(clon);
        inicializarPopovers(clon[0]);
        clon.slideDown(200).animate({ opacity: 1 }, { queue: false, duration: 300 });
        reindexarTodo();
    });

    $(document).on("click", ".deleteInputs", function () {
        var card = $(this).closest(".card-body");
        var bloques = card.find(".inputsEscenario");

        if (bloques.length > 1) {
            var ultimo = bloques.last();
            ultimo.animate({ opacity: 0 }, 200)
                .slideUp(250, function () {
                    ultimo.remove();
                    reindexarTodo();
                });
            return;
        }

        reindexarTodo();
    });

    reindexarEscenarios();
    reindexarTodo();
    actualizarAccionesEscenario();
});
