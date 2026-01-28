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

});