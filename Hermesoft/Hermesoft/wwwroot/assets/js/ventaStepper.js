$(function () {

    let cuotasOriginales = [];
    let ingresoNetoOriginal = 0;
    let gastoFormalizacionOriginal = 0;
    let tipoCambio = 1;

    $("#banco").on("change", function () {
        var idBanco = this.value;

        $.get('/Banco/ObtenerEscenariosJS', { idBanco: idBanco }, function (data) {
            var selectEscenarios = $("#escenario");
            selectEscenarios.empty();
            selectEscenarios.append('<option value="" selected hidden>Seleccione escenario</option>');

            for (let i = 0; i < data.length; i++) {
                selectEscenarios.append(`<option value="${data[i].idEscenario}">${data[i].nombre}</option>`)
            }
            selectEscenarios.prop("disabled", false);
        });

        $.get('/Banco/ObtenerEndeudamientosJS', { idBanco: idBanco }, function (data) {

            var selectTipoAsalariado = $("#tipoAsalariado");
            selectTipoAsalariado.empty();
            selectTipoAsalariado.append('<option value="" selected hidden>Seleccione un tipo de asalariado</option>');
            for (let i = 0; i < data.length; i++) {
                selectTipoAsalariado.append(`<option value="${data[i].idEndeudamiento}">${data[i].nombre}</option>`)
            }
        });

        $.get('/Banco/ObtenerInfoGeneralJS', { idBanco: idBanco }, function (data) {
            $("#porcSeguroVida").val(data.seguroVida);
            $("#porcSeguroDesempleo").val(data.seguroDesempleo);
            $("#porcAbogados").val(data.honorarioAbogados);
            $("#porcComision").val(data.comisionBancaria);
            $("#porcTimbre").val(data.timbreFiscal);
            calcularGastoFormalizacion();
        });
    });

    $("#calcGastoFormal").on("click", function () {
        calcularGastoFormalizacion();
    });

    function calcularGastoFormalizacion() {
        let seguroVida = $("#porcSeguroVida").val();
        let seguroDesempleo = $("#porcSeguroDesempleo").val();
        let honorarioAbogados = $("#porcAbogados").val();
        let comisionBancaria = $("#porcComision").val();
        let codLote = $("#lote").val();
        $.get('/Calculos/CalcularGastoFormalizacionJS', {
            seguroVida: seguroVida, seguroDesempleo: seguroDesempleo, honorarioAbogados: honorarioAbogados,
            comisionBancaria: comisionBancaria, codLote: codLote
        }, function (data) {

            gastoFormalizacionOriginal = data;
            renderFormalizacion();
        });
    }

    let cuotaAlta = 0;

    $("#escenario").on("change", function () {
        var idEscenario = this.value;
        var codigoLote = $("#lote").val();
        var selectTipoAsalariado = $("#tipoAsalariado");
        $.get('/Calculos/CalcularCuotasBancariaJS', { idEscenario: idEscenario, codigoLote: codigoLote }, function (data) {
            cuotaAlta = 0; 
            cuotasOriginales = data;
            renderCuotas();
            calcularIngresoNeto();

        });
        selectTipoAsalariado.prop("disabled", false);
    });

    $("#tipoAsalariado").on("change", function () {
        calcularIngresoNeto();
    });

    function calcularIngresoNeto() {
        var idBanco = $("#banco").val();
        var idEndeudamiento = $("#tipoAsalariado").val();
        $.get('/Calculos/CalcularIngresoNetoFamiliarJS', { idBanco: idBanco, idEndeudamiento: idEndeudamiento, cuotaMensual: cuotaAlta }, function (data) {

            ingresoNetoOriginal = data;
            renderIngreso();

        });
    }

    function renderCuotas() {

        let moneda = $("#monedaSelect").val();
        let cuerpoTabla = $("#bodyCuotas");
        cuerpoTabla.empty();

        for (let i = 0; i < cuotasOriginales.length; i++) {

            let monto = cuotasOriginales[i].montoMensual;

            if (moneda === "USD") {
                monto = monto / tipoCambio;
            }

            if (cuotaAlta < cuotasOriginales[i].montoMensual) {
                cuotaAlta = cuotasOriginales[i].montoMensual
            }

            cuerpoTabla.append(`
            <tr>
                <td>${cuotasOriginales[i].plazo}</td>
                <td>${cuotasOriginales[i].tasaInteres}</td>
                <td>${monto.toFixed(2)}</td>
            </tr>
        `);
        }
    }

    function renderIngreso() {

        let moneda = $("#monedaSelect").val();
        let ingreso = ingresoNetoOriginal;

        if (moneda === "USD") {
            ingreso = ingreso / tipoCambio;
        }

        $("#ingresoNeto").val(ingreso.toFixed(2));
    }

    function renderFormalizacion() {
        let moneda = $("#monedaSelect").val();
        let gastoFormalizacion = gastoFormalizacionOriginal;

        if (moneda === "USD") {
            gastoFormalizacion = gastoFormalizacion / tipoCambio;
        }

        $("#gastoFormalizacion").val(gastoFormalizacion.toFixed(2));
    }

    $.get('/Calculos/ObtenerCambioDelDolarJS', function (data) {
        tipoCambio = data;
    });

    $("#monedaSelect").on("change", function () {
        renderCuotas();
        renderIngreso();
        renderFormalizacion();
    });

});
