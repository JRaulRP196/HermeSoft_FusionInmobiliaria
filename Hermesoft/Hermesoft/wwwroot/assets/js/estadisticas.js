$(function () {

    let categorias = [];
    let pagados = [];
    let pendientes = [];
    let atrasados = [];

    let chart = null; 

    cargarDatos();

    async function cargarDatos() {

        categorias = [];
        pagados = [];
        pendientes = [];
        atrasados = [];

        let fechaInicio = $("#fechaInicio").val();
        let fechaFinal = $("#fechaFinal").val();

        ocultarMensajeFiltro();

        const errorFechas = validarRangoFechas(fechaInicio, fechaFinal);
        if (errorFechas) {
            limpiarResultados();
            mostrarMensajeFiltro(errorFechas);
            mostrarMensajeSinDatos();
            return;
        }

        try {
            const response = await fetch('/Condominio/Obtener');
            if (!response.ok) {
                throw new Error("No fue posible cargar los condominios.");
            }
            const data = await response.json();

            for (let i = 0; i < data.length; i++) {

                const parametros = new URLSearchParams({ condominio: data[i].id });
                if (fechaInicio) parametros.set("fechaInicio", fechaInicio);
                if (fechaFinal) parametros.set("fechaFinal", fechaFinal);

                const res = await fetch(`/Estadistica/PagosPorCondominio?${parametros}`);
                const datos = await res.json();


                if (datos.pagados > 0 || datos.pendientes > 0 || datos.atrasados > 0) {
                    categorias.push(data[i].nombre);
                    pagados.push(datos.pagados);
                    pendientes.push(datos.pendientes);
                    atrasados.push(datos.atrasados);
                }
            }

            if (categorias.length === 0) {
                $("#opciones").addClass("d-none");
                mostrarMensajeSinDatos();
                cargarKpis();
                return;
            }
            $("#opciones").removeClass("d-none");
            cargarKpis();
            crearGrafico();
        } catch (error) {
            limpiarResultados();
            mostrarMensajeFiltro(error.message || "Ocurrió un error al cargar la estadística.");
        }
    }

    $("#filtro").on("click", function (e) {
        e.preventDefault();
        cargarDatos();
    });

    function validarRangoFechas(fechaInicio, fechaFinal) {
        if (!fechaInicio && !fechaFinal) return null;
        if (!fechaInicio || !fechaFinal) {
            return "Debe seleccionar las fechas Desde y Hasta.";
        }
        if (fechaInicio > fechaFinal) {
            return "La fecha Desde no puede ser posterior a la fecha Hasta.";
        }
        return null;
    }

    function mostrarMensajeFiltro(mensaje) {
        $("#mensajeFiltro").text(mensaje).removeClass("d-none");
    }

    function ocultarMensajeFiltro() {
        $("#mensajeFiltro").addClass("d-none").text("");
    }

    function limpiarResultados() {
        if (chart) {
            chart.destroy();
            chart = null;
        }
        document.querySelector("#column_stacked").innerHTML = "";
        $("#opciones").addClass("d-none");
        cargarKpis();
    }

    function cargarKpis() {

        let pagosDia = $("#pagosAlDia");
        let pagosPendientes = $("#pagosPendientes");
        let pagosAtrasados = $("#pagosAtrasados");

        const totalPagados = pagados.reduce((a, b) => a + b, 0);
        const totalPendientes = pendientes.reduce((a, b) => a + b, 0);
        const totalAtrasados = atrasados.reduce((a, b) => a + b, 0);

        pagosDia.text(totalPagados);
        pagosPendientes.text(totalPendientes);
        pagosAtrasados.text(totalAtrasados);
    }

    function mostrarMensajeSinDatos() {

        const contenedor = document.querySelector("#column_stacked");

        if (chart) {
            chart.destroy();
            chart = null;
        }

        contenedor.innerHTML = `
        <div class="text-center p-5">
            <h5 class="text-muted">No hay ventas registradas</h5>
            <p class="text-secondary mb-0">
                No existen datos suficientes para generar la estadística.
            </p>
        </div>
        `;
    }

    function crearGrafico() {

        const contenedor = document.querySelector("#column_stacked");

        if (chart) {
            chart.destroy();
        }

        contenedor.innerHTML = "";

        var options = {
            series: [
                { name: 'PAGOS AL DÍA', data: pagados },
                { name: 'PAGOS PENDIENTES', data: pendientes },
                { name: 'PAGOS ATRASADOS', data: atrasados }
            ],
            chart: {
                type: 'bar',
                height: 350,
                stacked: true,
                toolbar: { show: true }
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    borderRadius: 10
                }
            },
            xaxis: {
                type: 'category',
                categories: categorias
            },
            legend: {
                position: 'right',
                offsetY: 40
            },
            fill: {
                opacity: 1
            },
            colors: ['#086070', '#ed5e49', '#2651e9']
        };

        chart = new ApexCharts(contenedor, options);
        chart.render();
    }


    $("#descargar").on("click", async function () {

        const fechaInicio = $("#fechaInicio").val();
        const fechaFinal = $("#fechaFinal").val();

        if (!chart) {
            alert("No hay datos para generar el PDF");
            return;
        }

        const { imgURI } = await chart.dataURI();

        function toISO(fecha) {
            return new Date(fecha).toISOString();
        }

        fetch("/Estadistica/GenerarPdf", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                graficoBase64: imgURI,
                desde: fechaInicio ? toISO(fechaInicio) : null,
                hasta: fechaFinal ? toISO(fechaFinal) : null
            })
        })
            .then(res => res.blob())
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement("a");
                a.href = url;
                a.download = (fechaInicio && fechaFinal)
                    ? `reporte_pagos_${fechaInicio}_${fechaFinal}.pdf`
                    : `reporte_pagos_${Date.now()}.pdf`;
                a.click();
            });

    });

});
