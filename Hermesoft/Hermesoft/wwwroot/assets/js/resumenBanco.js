document.addEventListener("DOMContentLoaded", function () {

    $(document).on("stepChanged", function (e, step) {

        if (step === 4) {
            generarResumenBanco();
        }

    });

    function generarResumenBanco() {

        const container = $("#resumenBanco");
        container.html("");

        // Toodo lo generakl
        const datosHTML = `
        <h5 class="fw-bold mb-3 text-primary">
            <i class="bi bi-bank me-2"></i>Datos Generales
        </h5>

        <div class="row g-3 mb-4">
            <div class="col-md-6">
                <div class="bg-light p-3 rounded-3">
                    <small class="text-muted">Banco</small>
                    <div class="fw-semibold">${$("#Nombre").val()}</div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="bg-light p-3 rounded-3">
                    <small class="text-muted">Enlace</small>
                    <div class="fw-semibold">${$("#Enlace").val()}</div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="bg-light p-3 rounded-3">
                    <small class="text-muted">Plazo Máximo</small>
                    <div class="fw-semibold">${$("#MaxCredito").val()} años</div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="bg-light p-3 rounded-3">
                    <small class="text-muted">Comisión</small>
                    <div class="fw-semibold">${$("#Comision").val()}%</div>
                </div>
            </div>
        </div>
    `;

        container.append(datosHTML);

        // Endeudamientos
        let endeudamientoHTML = `
        <h6 class="fw-bold text-primary">Endeudamiento Máximo</h6>
        <div class="table-responsive mb-4">
            <table class="table table-sm">
                <thead>
                    <tr>
                        <th>Tipo</th>
                        <th>%</th>
                    </tr>
                </thead>
                <tbody>
    `;

        $("input[name*='PorcEndeudamiento']").each(function () {
            const valor = $(this).val();
            const tipo = $(this).closest(".input-group").find(".input-group-text").text();

            endeudamientoHTML += `
            <tr>
                <td>${tipo}</td>
                <td>${valor}%</td>
            </tr>
        `;
        });

        endeudamientoHTML += `</tbody></table></div>`;
        container.append(endeudamientoHTML);

        //Seguros
        let segurosHTML = `
        <h6 class="fw-bold text-primary">Seguros</h6>
        <div class="mb-4">
    `;

        $("input[name*='PorcSeguro']").each(function () {
            const label = $(this).closest(".mb-3").find("label").text();
            const valor = $(this).val();

            segurosHTML += `
            <div class="d-flex justify-content-between bg-light p-2 rounded mb-1">
                <span>${label}</span>
                <strong>${valor}%</strong>
            </div>
        `;
        });

        segurosHTML += `</div>`;
        container.append(segurosHTML);

        //Escenarios
        let escenariosHTML = `<h6 class="fw-bold text-primary">Escenarios</h6>`;
        var escenarios = $("#listaEscenarios .escenarios");
        escenarios.each(function (index) {

            const nombre = $(this).find("input[name*='Nombre']").val();
            const tipo = $(this).find(".tipoInteres option:selected").text();

            let plazosHTML = "";
            let inputsEscenarios = $(this).find(".inputsEscenario");
            inputsEscenarios.each(function () {
                const plazo = $(this).find("input[name$='.Plazo']").val();
                const adicional = $(this).find("input[name$='.PorcAdicional']").val();

                plazosHTML += `
                <div class="small text-muted">
                    ${plazo} meses → ${adicional}%
                </div>
            `;
            });

            escenariosHTML += `
            <div class="card mb-3 shadow-sm border-0">
                <div class="card-body">
                    <h6 class="fw-bold">Escenario #${index + 1}</h6>
                    <div><strong>Tipo:</strong> ${tipo}</div>
                    <div><strong>Nombre:</strong> ${nombre}</div>
                    <div class="mt-2">${plazosHTML}</div>
                </div>
            </div>
        `;
        });

        container.append(escenariosHTML);
    }

});
