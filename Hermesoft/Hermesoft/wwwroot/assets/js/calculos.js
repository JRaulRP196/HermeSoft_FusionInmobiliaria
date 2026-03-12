
document.addEventListener("DOMContentLoaded", function () {
    const idLote = document.getElementById("lote");
    const porcentajePrima = document.getElementById("porcPrima");
    const fechaVencimientoPrima = document.getElementById("fechaPrima");
    const checkDescuento = document.getElementById("aplicaDescuentoPrima");
    const contenedorDescuento = document.getElementById("contenedorDescuentoPrima");
    const inputDescuento = document.getElementById("descuentoPrima");
    const botonCalcular = document.getElementById("calcPrima");

    function toggleDescuento() {
        if (!checkDescuento || !contenedorDescuento || !inputDescuento) return;

        if (checkDescuento.checked) {
            contenedorDescuento.classList.remove("d-none");
            inputDescuento.disabled = false;
        } else {
            contenedorDescuento.classList.add("d-none");
            inputDescuento.disabled = true;
            inputDescuento.value = 0;
        }
    }

    function datosCompletosParaPrima() {
        if (!idLote || !idLote.value) return false;
        if (!fechaVencimientoPrima || !fechaVencimientoPrima.value) return false;
        if (!porcentajePrima || !porcentajePrima.value) return false;

        const hoy = new Date();
        const fin = new Date(fechaVencimientoPrima.value);

        if (fin <= hoy) return false;

        if (fin.getMonth() === hoy.getMonth() && fin.getFullYear() === hoy.getFullYear()) {
            return false;
        }

        if (checkDescuento && checkDescuento.checked) {
            const descuento = Number(inputDescuento.value || 0);
            if (descuento < 0 || descuento > 100) return false;
        }

        return true;
    }

    function actualizarBotonPrima() {
        if (!botonCalcular) return;

        if (datosCompletosParaPrima()) {
            botonCalcular.classList.remove("d-none");
        } else {
            botonCalcular.classList.add("d-none");
        }
    }

    function calcularPrima() {
        const lote = idLote.value;
        const fin = fechaVencimientoPrima.value;
        const porcentaje = porcentajePrima.value;

        let descuento = 0;
        if (checkDescuento && checkDescuento.checked) {
            descuento = inputDescuento.value || 0;
        }

        window.location.href =
            `/Calculos/CalcularPrima?codigoLote=${encodeURIComponent(lote)}&porcentajePrima=${encodeURIComponent(porcentaje)}&fechaFinal=${encodeURIComponent(fin)}&porcentajeDescuento=${encodeURIComponent(descuento)}`;
    }

    if (checkDescuento) {
        checkDescuento.addEventListener("change", function () {
            toggleDescuento();
            actualizarBotonPrima();
        });
    }

    if (fechaVencimientoPrima) {
        fechaVencimientoPrima.addEventListener("change", actualizarBotonPrima);
    }

    if (porcentajePrima) {
        porcentajePrima.addEventListener("change", actualizarBotonPrima);
        porcentajePrima.addEventListener("keyup", actualizarBotonPrima);
    }

    if (inputDescuento) {
        inputDescuento.addEventListener("change", actualizarBotonPrima);
        inputDescuento.addEventListener("keyup", actualizarBotonPrima);
    }

    if (botonCalcular) {
        botonCalcular.addEventListener("click", calcularPrima);
    }

    toggleDescuento();
    actualizarBotonPrima();
});