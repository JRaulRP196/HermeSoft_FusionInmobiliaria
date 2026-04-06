$(function () {
  let cuotasOriginales = [];
  let ingresoNetoOriginal = 0;
  let gastoFormalizacionOriginal = 0;
  let tipoCambio = 1;
  const formatter = new Intl.NumberFormat(navigator.language || "es-CR", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });

  function formatearNumero(valor) {
    const numero = Number(valor);
    if (Number.isNaN(numero)) return "";
    return formatter.format(numero);
  }
  let cuotaAlta = 0;

  $("#banco").on("change", function () {
    var idBanco = this.value;

    $("#escenario")
      .prop("disabled", true)
      .empty()
      .append('<option value="" selected hidden>Seleccione escenario</option>');

    $("#tipoAsalariado")
      .prop("disabled", true)
      .empty()
      .append(
        '<option value="" selected hidden>Seleccione un tipo de asalariado</option>',
      );

    cuotaAlta = 0;
    cuotasOriginales = [];
    ingresoNetoOriginal = 0;
    gastoFormalizacionOriginal = 0;

    $("#ingresoNeto").val("");
    $("#gastoFormalizacion").val("");
    $("#bodyCuotas").empty();

    $.get("/Banco/ObtenerEscenariosJS", { idBanco: idBanco }, function (data) {
      var selectEscenarios = $("#escenario");
      selectEscenarios.empty();
      selectEscenarios.append(
        '<option value="" selected hidden>Seleccione escenario</option>',
      );

      for (let i = 0; i < data.length; i++) {
        selectEscenarios.append(
          `<option value="${data[i].idEscenario}">${data[i].nombre}</option>`,
        );
      }

      selectEscenarios.prop("disabled", false);
    });

    $.get(
      "/Banco/ObtenerEndeudamientosJS",
      { idBanco: idBanco },
      function (data) {
        var selectTipoAsalariado = $("#tipoAsalariado");
        selectTipoAsalariado.empty();
        selectTipoAsalariado.append(
          '<option value="" selected hidden>Seleccione un tipo de asalariado</option>',
        );

        for (let i = 0; i < data.length; i++) {
          selectTipoAsalariado.append(
            `<option value="${data[i].idEndeudamiento}">${data[i].nombre}</option>`,
          );
        }
      },
    );

    $.get("/Banco/ObtenerInfoGeneralJS", { idBanco: idBanco }, function (data) {
      $("#porcSeguroVida").val(data.seguroVida);
      $("#porcSeguroDesempleo").val(data.seguroDesempleo);
      $("#porcAbogados").val(data.honorarioAbogados);
      $("#porcComision").val(data.comisionBancaria);
      $("#porcTimbre").val(data.timbreFiscal);
    });
  });

  function calcularGastoFormalizacion() {
    let seguroVida = $("#porcSeguroVida").val();
    let seguroDesempleo = $("#porcSeguroDesempleo").val();
    let honorarioAbogados = $("#porcAbogados").val();
    let comisionBancaria = $("#porcComision").val();
    let codLote = $("#lote").val();

    if (
      !seguroVida ||
      !seguroDesempleo ||
      !honorarioAbogados ||
      !comisionBancaria ||
      !codLote
    ) {
      return;
    }

    $.get(
      "/Calculos/CalcularGastoFormalizacionJS",
      {
        seguroVida: seguroVida,
        seguroDesempleo: seguroDesempleo,
        honorarioAbogados: honorarioAbogados,
        comisionBancaria: comisionBancaria,
        codLote: codLote,
      },
      function (data) {
        gastoFormalizacionOriginal = data;
        renderFormalizacion();
      },
    ).fail(function (xhr) {
      console.error(
        "Error al calcular gasto de formalización:",
        xhr.responseText,
      );
    });
  }

  $("#escenario").on("change", function () {
    var idEscenario = this.value;
    var codigoLote = $("#lote").val();

    if (!idEscenario || !codigoLote) {
      return;
    }

    $.get(
      "/Calculos/CalcularCuotasBancariaJS",
      {
        idEscenario: idEscenario,
        codigoLote: codigoLote,
      },
      function (data) {
        cuotaAlta = 0;
        cuotasOriginales = data;
        renderCuotas();

        $("#tipoAsalariado").prop("disabled", false);
        calcularGastoFormalizacion();
      },
    ).fail(function (xhr) {
      console.error("Error al calcular cuotas:", xhr.responseText);
    });
  });

  $("#tipoAsalariado").on("change", function () {
    calcularIngresoNeto();
  });

  function calcularIngresoNeto() {
    var idBanco = $("#banco").val();
    var idEndeudamiento = $("#tipoAsalariado").val();

    if (!idBanco || !idEndeudamiento || !cuotaAlta || cuotaAlta <= 0) {
      return;
    }

    $.get(
      "/Calculos/CalcularIngresoNetoFamiliarJS",
      {
        idBanco: idBanco,
        idEndeudamiento: idEndeudamiento,
        cuotaMensual: cuotaAlta,
      },
      function (data) {
        ingresoNetoOriginal = data;
        renderIngreso();
      },
    ).fail(function (xhr) {
      console.error("Error al calcular ingreso neto:", xhr.responseText);
    });
  }

  function renderCuotas() {
    let moneda = $("#monedaSelect").val();
    let cuerpoTabla = $("#bodyCuotas");
    cuerpoTabla.empty();
    cuotaAlta = 0;

    for (let i = 0; i < cuotasOriginales.length; i++) {
      let monto = cuotasOriginales[i].montoMensual;

      if (moneda === "USD") {
        monto = monto / tipoCambio;
      }

      if (cuotasOriginales[i].montoMensual > cuotaAlta) {
        cuotaAlta = cuotasOriginales[i].montoMensual;
      }

      cuerpoTabla.append(`
            <tr>
                <td>${cuotasOriginales[i].plazo}</td>
                <td>${cuotasOriginales[i].tasaInteres}</td>
                <td>${formatearNumero(monto)}</td>
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

    $("#ingresoNeto").val(formatearNumero(ingreso));
  }

  function renderFormalizacion() {
    let moneda = $("#monedaSelect").val();
    let gastoFormalizacion = gastoFormalizacionOriginal;

    if (moneda === "USD") {
      gastoFormalizacion = gastoFormalizacion / tipoCambio;
    }

    $("#gastoFormalizacion").val(gastoFormalizacion.toFixed(2));
    $("#gastoFormalizacionDisplay").val(formatearNumero(gastoFormalizacion));
  }

  $.get("/Calculos/ObtenerCambioDelDolarJS", function (data) {
    tipoCambio = data;
  });

  $("#monedaSelect").on("change", function () {
    renderCuotas();
    renderIngreso();
    renderFormalizacion();
  });
});
