document.addEventListener("DOMContentLoaded", function () {

    const idLote = $("#lote");
    const porcentajePrima = $("#porcPrima");
    const fechaVencimientoPrima = $("#fechaPrima");
    const resultadoPrima = $("#desglosePrima");

    //Para el calculo de prima automatico
    function calcularPrima() {
        const lote = idLote.val();
        const fin = fechaVencimientoPrima.val();
        const porcentaje = porcentajePrima.val();

        window.location.href =
            `/Calculos/CalcularPrima?codigoLote=${lote}&porcentajePrima=${porcentaje}&fechaFinal=${fin}`;
    }

    function datosCompletosParaPrima() {

        if (!idLote.val()) return false;
        if (!fechaVencimientoPrima.val()) return false;
        if (!porcentajePrima.val()) return false;

        const hoy = new Date();
        const fin = new Date(fechaVencimientoPrima.val());

        if (fin <= hoy) {
            return false;
        } else if (fin.getMonth() == hoy.getMonth()) {
            if (fin.getFullYear() == hoy.getFullYear()) {
                return false;
            }
        }

        return true;
    }

    function actualizarBotonPrima() {
        if (datosCompletosParaPrima()) {
            $("#calcPrima").removeClass("d-none");
        } else {
            $("#calcPrima").addClass("d-none");
        }
    }

    fechaVencimientoPrima.on("change", actualizarBotonPrima);
    porcentajePrima.on("change keyup", actualizarBotonPrima);

    $("#calcPrima").on("click", function () {
        calcularPrima();
    });

    // ================================
    // GASTO DE FORMALIZACIÓN
    // ================================
    $("#calcGastoFormal").on("click", function () {

        const lote = $("#lote").val();
        const banco = $("#banco").val();

        if (!lote || !banco) {
            $("#gastoFormalizacion").val("Seleccione lote y banco");
            return;
        }

        const seguroVida = parseFloat($("#porcSeguroVida").val()) || 0;
        const seguroDesempleo = parseFloat($("#porcSeguroDesempleo").val()) || 0;
        const honorarios = parseFloat($("#porcAbogados").val()) || 0;
        const comision = parseFloat($("#porcComision").val()) || 0;
        const timbre = parseFloat($("#porcTimbre").val()) || 0;

        const porcentajeTotal = seguroVida + seguroDesempleo + honorarios + comision + timbre;

        // Por ahora: mock
        const precioLote = 100000000;

        const gasto = precioLote * (porcentajeTotal / 100);

        $("#gastoFormalizacion").val(
            "₡ " + gasto.toLocaleString("es-CR", { minimumFractionDigits: 2 })
        );
        // ========================================
        // VISIBILIDAD BOTÓN GASTO FORMALIZACIÓN
        // Escenario 3
        // ========================================
        function actualizarBotonGastoFormal() {
            const lote = $("#lote").val();
            const banco = $("#banco").val();

            if (lote && banco) {
                $("#calcGastoFormal").removeClass("d-none");
            } else {
                $("#calcGastoFormal").addClass("d-none");
                $("#gastoFormalizacion").val("");
            }
        }

        // Detectar cambios
        $("#lote").on("change keyup", actualizarBotonGastoFormal);
        $("#banco").on("change", actualizarBotonGastoFormal);

        // Ejecutar al cargar la página
        actualizarBotonGastoFormal();

    });




});