$(function () {

    let categorias = [];
    let pagados = [];
    let pendientes = [];
    let atrasados = [];

    let chart = null;
    let hayDatos = false; 

    cargarDatos();

  
    async function cargarDatos() {

        categorias = [];
        pagados = [];
        pendientes = [];
        atrasados = [];
        hayDatos = false;

        let fechaInicio = $("#fechaInicio").val();
        let fechaFinal = $("#fechaFinal").val();

        const response = await fetch('/Condominio/Obtener');
        const data = await response.json();

        for (let i = 0; i < data.length; i++) {

            const res = await fetch(`/Estadistica/PagosPorCondominio?condominio=${data[i].id}&fechaInicio=${fechaInicio}&fechaFinal=${fechaFinal}`);
            const datos = await res.json();

            if (datos.pagados > 0 || datos.pendientes > 0 || datos.atrasados > 0) {
                categorias.push(data[i].nombre);
                pagados.push(datos.pagados);
                pendientes.push(datos.pendientes);
                atrasados.push(datos.atrasados);
            }
        }

       
        if (categorias.length === 0) {
            hayDatos = false;
            $("#descargarPdf").prop("disabled", true);
            mostrarMensaje();
            return;
        }

        hayDatos = true;
        $("#descargarPdf").prop("disabled", false);

        crearGrafico();
    }

    $("#filtro").on("click", function (e) {
        e.preventDefault();
        cargarDatos();
    });

    function mostrarMensaje() {

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

    // ==============================
    // GRÁFICO
    // ==============================
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
                toolbar: { show: false }
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    borderRadius: 10
                }
            },
            xaxis: {
                categories: categorias
            },
            legend: {
                position: 'right'
            },
            fill: {
                opacity: 1
            },
            colors: ['#086070', '#ed5e49', '#2651e9']
        };

        chart = new ApexCharts(contenedor, options);
        chart.render();
    }

    // ==============================
    // DESCARGAR PDF
    // ==============================
    $("#descargarPdf").on("click", async function () {

        let fechaInicio = $("#fechaInicio").val();
        let fechaFinal = $("#fechaFinal").val();

        if (!fechaInicio || !fechaFinal) {
            alert("Debe seleccionar un rango de fechas");
            return;
        }

      
        if (!hayDatos) {
            alert("No hay datos para generar el reporte.");
            return;
        }

        const responseCondominios = await fetch('/Condominio/Obtener');
        const condominios = await responseCondominios.json();

        const response = await fetch('/Estadistica/DescargarPdf', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                fechaInicio,
                fechaFinal,
                condominios
            })
        });

        if (!response.ok) {
            const mensaje = await response.text();
            alert(mensaje);
            return;
        }

        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);

        const a = document.createElement("a");
        a.href = url;
        a.download = "ReportePagos.pdf";
        a.click();
    });

});