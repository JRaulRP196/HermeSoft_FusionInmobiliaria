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

    document.addEventListener("DOMContentLoaded", () => {

        const btn = document.getElementById("calcGastoFormal");
        if (!btn) return;

        btn.addEventListener("click", async () => {
            try {
                const loteCodigo = document.getElementById("lote")?.value;
                const banco = document.getElementById("banco")?.value;

                // Requisito: si no hay banco o lote, no se debería calcular
                if (!loteCodigo || !banco) {
                    alert("Debe seleccionar lote y banco para calcular el gasto de formalización.");
                    return;
                }

                // 1) Leer porcentajes desde tu UI (OJO: tienes dos #porcSeguros)
                const segurosInputs = document.querySelectorAll('#porcSeguros');
                const porcVida = segurosInputs.length > 0 ? parseFloat(segurosInputs[0].value || "0") : 0;
                const porcDesempleo = segurosInputs.length > 1 ? parseFloat(segurosInputs[1].value || "0") : 0;

                const porcAbogados = parseFloat(document.getElementById("porcAbogados")?.value || "0");
                const porcComision = parseFloat(document.getElementById("porcComision")?.value || "0");
                const porcTimbre = parseFloat(document.getElementById("porcTimbre")?.value || "-1");

                // 2) Traer precio del lote usando tu endpoint actual (sin tocar HTML)
                const loteResp = await fetch(`/Ventas/ObtenerLoteJson?lote=${encodeURIComponent(loteCodigo)}`);
                if (!loteResp.ok) {
                    alert("No se pudo obtener la información del lote.");
                    return;
                }

                const loteJson = await loteResp.json();
                // Tu API devuelve lista; si no, igual lo resolvemos:
                const lote = Array.isArray(loteJson) ? (loteJson[0] || null) : loteJson;

                if (!lote) {
                    alert("El lote no existe o no se pudo leer.");
                    return;
                }

                // Ajuste por nombres posibles: PrecioVenta o precioVenta
                const valorLote = parseFloat(lote.PrecioVenta ?? lote.precioVenta ?? 0);

                if (!valorLote || valorLote <= 0) {
                    alert("El valor del lote no es válido (PrecioVenta).");
                    return;
                }

                // 3) Llamar al controlador para calcular (súper simple)
                const calcResp = await fetch("/Ventas/CalcularGastoFormalizacion", {
                    method: "POST",
                    headers: { "Content-Type": "application/x-www-form-urlencoded" },
                    body: new URLSearchParams({
                        valorLote: valorLote.toString(),
                        porcVida: porcVida.toString(),
                        porcDesempleo: porcDesempleo.toString(),
                        porcAbogados: porcAbogados.toString(),
                        porcComision: porcComision.toString(),
                        porcTimbre: porcTimbre.toString()
                    })
                });

                const result = await calcResp.json();
                if (!result.ok) {
                    alert("No se pudo calcular el gasto de formalización.");
                    return;
                }

                // 4) Mostrar resultado en tu input existente
                const out = document.getElementById("gastoFormalizacion");
                if (out) out.value = Number(result.gastoFormalizacion).toFixed(2);

            } catch (e) {
                console.error(e);
                alert("Ocurrió un error al calcular el gasto de formalización.");
            }
        });

    });

});