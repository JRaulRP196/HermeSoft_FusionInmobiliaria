document.addEventListener("DOMContentLoaded", function () {

    const idLote = $("#lote");
    const porcentajePrima = $("#porcPrima");
    const fechaVencimientoPrima = $("#fechaPrima");
    const resultadoPrima = $("#desglosePrima");

    //Para el calculo de prima automatico

    function ObtenerLote(codigoLote) {
        return fetch(`/Ventas/ObtenerLoteJson?lote=${codigoLote}`)
            .then(res => res.json());
    }
    async function calcularPrima() {
        const lote = await ObtenerLote(idLote.val());
        const inicio = new Date();
        const fin = new Date(fechaVencimientoPrima.val());
        const porcentaje = porcentajePrima.val() * 0.01;

        let meses = (fin.getFullYear() - inicio.getFullYear()) * 12 +
                    (fin.getMonth() - inicio.getMonth());

        if (fin.getDate() < inicio.getDate()) {
            meses--;
        }

        const montoPrima = lote[0].precioVenta * porcentaje;
        const mensualidad = montoPrima / meses;
        console.log("mensualidad: " + mensualidad + " meses: " + meses + " montoPrima: " + montoPrima);

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