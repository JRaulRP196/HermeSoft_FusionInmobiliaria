(function () {

    const isAdmin = false;

    const KEY_VENTAS = "hermes_ventas";
    const KEY_BITACORA = "hermes_bitacora";
    const KEY_LOTES = "hermes_lotes_state";

    function readVentas() { return JSON.parse(localStorage.getItem(KEY_VENTAS) || "[]"); }
    function writeVentas(arr) { localStorage.setItem(KEY_VENTAS, JSON.stringify(arr)); }
    function addBitacora(entry) {
        const b = JSON.parse(localStorage.getItem(KEY_BITACORA) || "[]");
        b.push(entry);
        localStorage.setItem(KEY_BITACORA, JSON.stringify(b));
    }

    function initLotes() {
        if (!localStorage.getItem(KEY_LOTES)) {
            const sample = {
                "Lote-001": { estado: "Disponible", precio: 35000000, condominio: "Vista Real" },
                "Lote-002": { estado: "Disponible", precio: 42000000, condominio: "Sol" },
                "Lote-003": { estado: "Vendido", precio: 28000000, condominio: "Lago" }
            };
            localStorage.setItem(KEY_LOTES, JSON.stringify(sample));
        }
    }
    function readLotes() { return JSON.parse(localStorage.getItem(KEY_LOTES) || "{}"); }
    function writeLotes(obj) { localStorage.setItem(KEY_LOTES, JSON.stringify(obj)); }

    function generarContrato() {
        const ts = Date.now().toString(36);
        const rnd = Math.floor(Math.random() * 9000) + 1000;
        return `CTR-${ts}-${rnd}`;
    }

    function showMessage(elId, html, type = "success") {
        const el = document.getElementById(elId);
        if (!el) return;
        el.innerHTML = `<div class="alert alert-${type}">${html}</div>`;
        setTimeout(() => { el.innerHTML = ""; }, 6000);
    }

    function fmt(n) {
        try { return new Intl.NumberFormat("es-CR").format(n); }
        catch { return n; }
    }

    async function setupRegistro() {
        const form = document.getElementById("ventaForm");
        if (!form) return;

        const cliente = document.getElementById("cliente");
        const lote = document.getElementById("lote");
        const porcPrima = document.getElementById("porcPrima");
        const fechaPrima = document.getElementById("fechaPrima");

        const calcGastoBtn = document.getElementById("calcGastoFormal");
        const gastoFormalizacion = document.getElementById("gastoFormalizacion");

        const porcAbogados = document.getElementById("porcAbogados");
        const porcComision = document.getElementById("porcComision");
        const porcTimbre = document.getElementById("porcTimbre");
        const msgTimbre = document.getElementById("msgTimbre");

       
        const plazoMeses = document.getElementById("plazoMeses");

       
        const plazoSelect = document.getElementById("plazoSelect");

        const tasaInteres = document.getElementById("tasaInteres");
        const cuotaMensual = document.getElementById("cuotaMensual");
        const ingresoNeto = document.getElementById("ingresoNeto");

        const moneda = document.getElementById("moneda");
        const banco = document.getElementById("banco");
        const tipoAsalariado = document.getElementById("tipoAsalariado");
        const fechaVenta = document.getElementById("fechaVenta");
        const asesor = document.getElementById("asesor");
        const generarPdfBtn = document.getElementById("generarPdfBtn");
        const enviarCorreoBtn = document.getElementById("enviarCorreoBtn");

        const escenario = document.getElementById("escenario");
        const btnCuota = document.getElementById("calcCuotaBancaria");
        const tablaBody = document.querySelector("#tablaCuotas tbody");

        
        const plazoMesesDisplay = document.getElementById("plazoMesesDisplay");

        calcGastoBtn?.addEventListener("click", async () => {
            try {
                const codigoLote = lote?.value;
                const idBanco = parseInt(banco?.value || "0");

                if (!codigoLote || !idBanco) {
                    showMessage("formMessages", "Debe seleccionar lote y banco.", "danger");
                    return;
                }

                const url = `/Ventas/ObtenerGastoFormalizacion?codigoLote=${encodeURIComponent(codigoLote)}&idBanco=${idBanco}`;
                const resp = await fetch(url);
                const result = await resp.json().catch(() => null);

                if (!resp.ok || !result?.ok) {
                    showMessage("formMessages", result?.message || "No se pudo calcular el gasto de formalización.", "danger");
                    return;
                }

                const d = result.data;

                const segurosInputs = document.querySelectorAll("#porcSeguros");
                if (segurosInputs.length > 0) segurosInputs[0].value = d.porcVida ?? 0;
                if (segurosInputs.length > 1) segurosInputs[1].value = d.porcDesempleo ?? 0;

                if (porcAbogados) porcAbogados.value = d.porcAbogados ?? 0;
                if (porcComision) porcComision.value = d.porcComision ?? 0;

                if (porcTimbre) {
                    porcTimbre.value = d.timbreFiscal ?? 0;

                    if (Number(d.timbreFiscal) === -1) {
                        porcTimbre.readOnly = false;
                        porcTimbre.style.border = "2px solid red";
                        porcTimbre.style.backgroundColor = "#ffe5e5";
                        msgTimbre?.classList.remove("d-none");
                    } else {
                        porcTimbre.readOnly = true;
                        porcTimbre.style.border = "";
                        porcTimbre.style.backgroundColor = "";
                        msgTimbre?.classList.add("d-none");
                    }
                }

                if (gastoFormalizacion) gastoFormalizacion.value = fmt(d.gastoFormalizacion);

                baseCRC.gasto = Number(d.gastoFormalizacion);
                
                aplicarMoneda(document.getElementById("monedaSelect")?.value || "CRC");


                showMessage("formMessages", "Gasto de formalización calculado.", "success");
            } catch (e) {
                console.error(e);
                showMessage("formMessages", "Error calculando gasto de formalización.", "danger");
            }
        });

        function toggleBtnCuota() {
            if (!btnCuota) return;

            const tieneBanco = !!(banco?.value);
            const tieneEscenario = !!(escenario?.value);
            const tieneMeses = Number(plazoMeses?.value || 0) > 0;

            btnCuota.style.display = (tieneBanco && tieneEscenario && tieneMeses) ? "inline-block" : "none";
        }

        async function cargarEscenariosBanco() {
            if (!escenario) return;

            escenario.innerHTML = `<option value="">Seleccione escenario</option>`;
            escenario.disabled = true;

            // reset plazos
            if (plazoSelect) {
                plazoSelect.innerHTML = `<option value="">Seleccione plazo</option>`;
                plazoSelect.disabled = true;
            }
            if (plazoMeses) plazoMeses.value = "";
            if (plazoMesesDisplay) plazoMesesDisplay.value = "";

            const idBanco = parseInt(banco?.value || "0");
            if (!idBanco) {
                toggleBtnCuota();
                return;
            }

            try {
                const resp = await fetch(`/Ventas/ObtenerEscenariosPorBanco?idBanco=${idBanco}`);
                const json = await resp.json().catch(() => null);

                if (!resp.ok || !json?.ok) {
                    showMessage("formMessages", json?.message || "No se pudieron cargar escenarios.", "danger");
                    toggleBtnCuota();
                    return;
                }

                const lista = json.data || [];
                if (!lista.length) {
                    showMessage("formMessages", "El banco no tiene escenarios configurados.", "warning");
                    toggleBtnCuota();
                    return;
                }

                escenario.innerHTML =
                    `<option value="">Seleccione escenario</option>` +
                    lista.map(e => `<option value="${e.idEscenario}">${e.nombre}</option>`).join("");

                escenario.disabled = false;
                toggleBtnCuota();
            } catch (e) {
                console.error(e);
                showMessage("formMessages", "Error cargando escenarios del banco.", "danger");
                toggleBtnCuota();
            }
        }

        async function cargarPlazosEscenario() {
            if (!plazoSelect) return;

            plazoSelect.innerHTML = `<option value="">Seleccione plazo</option>`;
            plazoSelect.disabled = true;
            if (plazoMeses) plazoMeses.value = "";
            if (plazoMesesDisplay) plazoMesesDisplay.value = "";

            const idBanco = parseInt(banco?.value || "0");
            const idEscenario = parseInt(escenario?.value || "0");
            if (!idBanco || !idEscenario) {
                toggleBtnCuota();
                return;
            }

            try {
                const resp = await fetch(`/Ventas/ObtenerPlazosPorEscenario?idBanco=${idBanco}&idEscenario=${idEscenario}`);
                const json = await resp.json().catch(() => null);

                if (!resp.ok || !json?.ok) {
                    showMessage("formMessages", json?.message || "No se pudieron cargar plazos del escenario.", "danger");
                    toggleBtnCuota();
                    return;
                }

                const plazos = json.data || [];
                if (!plazos.length) {
                    showMessage("formMessages", "El escenario no tiene plazos configurados.", "warning");
                    toggleBtnCuota();
                    return;
                }

                plazoSelect.innerHTML =
                    `<option value="">Seleccione plazo</option>` +
                    plazos.map(p => `<option value="${p}">${p}</option>`).join("");

                plazoSelect.disabled = false;

               
                plazoSelect.value = plazos[0];
                if (plazoMeses) plazoMeses.value = plazos[0];
                if (plazoMesesDisplay) plazoMesesDisplay.value = plazos[0];

                toggleBtnCuota();
                await cargarTasaEscenario();
            } catch (e) {
                console.error(e);
                showMessage("formMessages", "Error cargando plazos del escenario.", "danger");
                toggleBtnCuota();
            }
        }

        async function cargarTasaEscenario() {
            try {
                const idBanco = parseInt(banco?.value || "0");
                const idEscenario = parseInt(escenario?.value || "0");
                const meses = parseInt(plazoMeses?.value || "0");

                if (!idBanco || !idEscenario || !meses) return;

                const resp = await fetch(`/Ventas/ObtenerTasaInteresEscenario?idBanco=${idBanco}&idEscenario=${idEscenario}&plazoMeses=${meses}`);

                if (!resp.ok) {
                    const err = await resp.json().catch(() => null);
                    showMessage("formMessages", err?.message || "No existen datos suficientes para calcular la tasa de interés.", "danger");
                    if (tasaInteres) tasaInteres.value = "";
                    return;
                }

                const json = await resp.json();
                if (!json.ok) {
                    showMessage("formMessages", json.message || "No existen datos suficientes para calcular la tasa de interés.", "danger");
                    if (tasaInteres) tasaInteres.value = "";
                    return;
                }

                if (tasaInteres) {
                    tasaInteres.value = json.data.tasaFinal;
                    tasaInteres.readOnly = true;
                }
            } catch (e) {
                console.error(e);
                showMessage("formMessages", "Error consultando tasa de interés.", "danger");
            }
        }

        async function calcularCuotaBancaria() {
            try {
                const codigoLote = lote?.value;
                const idBanco = parseInt(banco?.value || "0");
                const idEscenario = parseInt(escenario?.value || "0");
                const meses = parseInt(plazoMeses?.value || "0");
                const primaPct = Number(porcPrima?.value || 0);

                if (!codigoLote || !idBanco || !idEscenario || !meses) return;

                const resp = await fetch(
                    `/Ventas/CalcularCuotaMensualBancaria?codigoLote=${encodeURIComponent(codigoLote)}&idBanco=${idBanco}&idEscenario=${idEscenario}&plazoMeses=${meses}&porcPrima=${primaPct}`
                );

                if (!resp.ok) {
                    const err = await resp.json().catch(() => null);
                    showMessage("formMessages", err?.message || "No se pudo calcular la cuota.", "danger");
                    return;
                }

                const json = await resp.json();
                if (!json.ok) {
                    showMessage("formMessages", json.message || "No se pudo calcular la cuota.", "danger");
                    return;
                }

                const d = json.data;

                if (tablaBody) {
                    tablaBody.innerHTML = `
                        <tr>
                            <td>${d.plazoMeses}</td>
                            <td>${d.escenario}</td>
                            <td>${d.indicador}</td>
                            <td>${d.tasaFinal}</td>
                            <td>${fmt(d.cuotaMensual)}</td>
                        </tr>
                    `;
                }

                if (plazoMesesDisplay) plazoMesesDisplay.value = d.plazoMeses;

                if (tasaInteres) {
                    tasaInteres.value = d.tasaFinal;
                    tasaInteres.readOnly = true;
                }

                if (cuotaMensual) cuotaMensual.value = fmt(d.cuotaMensual);
                if (ingresoNeto) ingresoNeto.value = fmt(Number(d.cuotaMensual || 0) * 3);

                baseCRC.cuota = Number(d.cuotaMensual);
                baseCRC.ingreso = Number(d.cuotaMensual || 0) * 3;

                aplicarMoneda(document.getElementById("monedaSelect")?.value || "CRC");

               
                baseCRC.cuota = Number(d.cuotaMensual);
                baseCRC.ingreso = Number(d.cuotaMensual || 0) * 3;

               
                aplicarMoneda(document.getElementById("monedaSelect")?.value || "CRC");


                showMessage("formMessages", "Cuota calculada con sistema francés.", "success");
            } catch (e) {
                console.error(e);
                showMessage("formMessages", "Error calculando cuota mensual bancaria.", "danger");
            }
        }

        function toggleBtnFormalizacion() {
            const tieneLote = !!(lote?.value);
            const tieneBanco = !!(banco?.value);
            if (!calcGastoBtn) return;
            calcGastoBtn.style.display = (tieneLote && tieneBanco) ? "block" : "none";
        }

        async function cargarDatosBancoDesdeBD() {
            try {
                const idBanco = parseInt(banco?.value || "0");
                if (!idBanco) return;

                const resp = await fetch(`/Ventas/ObtenerDatosBanco?idBanco=${idBanco}`);
                const json = await resp.json().catch(() => null);

                if (!resp.ok || !json?.ok) {
                    showMessage("formMessages", json?.message || "No se pudo cargar banco.", "danger");
                    return;
                }

                const d = json.data;

                const segurosInputs = document.querySelectorAll("#porcSeguros");
                if (segurosInputs.length > 0) {
                    segurosInputs[0].value = d.porcVida ?? 0;
                    segurosInputs[0].readOnly = true;
                }
                if (segurosInputs.length > 1) {
                    segurosInputs[1].value = d.porcDesempleo ?? 0;
                    segurosInputs[1].readOnly = true;
                }

                if (porcComision) {
                    porcComision.value = d.comision ?? 0;
                    porcComision.readOnly = true;
                }

                if (porcAbogados) {
                    porcAbogados.value = d.honorarioAbogado ?? 0;
                    porcAbogados.readOnly = true;
                }
            } catch (e) {
                console.error(e);
                showMessage("formMessages", "Error cargando datos del banco.", "danger");
            }
        }

        async function cargarTimbreFiscal() {
            try {
                const resp = await fetch("/Calculos/ObtenerTimbre");
                const timbre = await resp.json();

                if (Number(timbre) === -1) {
                    porcTimbre.value = -1;
                    porcTimbre.readOnly = false;
                    porcTimbre.style.border = "2px solid red";
                    porcTimbre.style.backgroundColor = "#ffe5e5";
                    if (msgTimbre) msgTimbre.classList.remove("d-none");
                } else {
                    porcTimbre.value = timbre;
                    porcTimbre.readOnly = true;
                    porcTimbre.style.border = "";
                    porcTimbre.style.backgroundColor = "";
                    if (msgTimbre) msgTimbre.classList.add("d-none");
                }
            } catch {
                porcTimbre.value = -1;
                porcTimbre.readOnly = false;
                porcTimbre.style.border = "2px solid red";
                porcTimbre.style.backgroundColor = "#ffe5e5";
                if (msgTimbre) msgTimbre.classList.remove("d-none");
            }
        }

        banco?.addEventListener("change", async () => {
            toggleBtnFormalizacion();

            await cargarEscenariosBanco();
            await cargarDatosBancoDesdeBD();

            if (tasaInteres) tasaInteres.value = "";
            if (plazoMesesDisplay) plazoMesesDisplay.value = "";
            if (tablaBody) tablaBody.innerHTML = "";
            if (cuotaMensual) cuotaMensual.value = "";
            if (ingresoNeto) ingresoNeto.value = "";

            toggleBtnCuota();
        });

        
        escenario?.addEventListener("change", async () => {
            await cargarPlazosEscenario();
        });

        
        plazoSelect?.addEventListener("change", async () => {
            const v = parseInt(plazoSelect.value || "0");
            if (plazoMeses) plazoMeses.value = v ? v : "";
            if (plazoMesesDisplay) plazoMesesDisplay.value = v ? v : "";
            toggleBtnCuota();
            await cargarTasaEscenario();
        });

        btnCuota?.addEventListener("click", calcularCuotaBancaria);

        // init
        toggleBtnFormalizacion();
        toggleBtnCuota();
        cargarTimbreFiscal();

        // si ya hay banco seleccionado al cargar
        if (banco?.value) {
            await cargarEscenariosBanco();
            await cargarDatosBancoDesdeBD();
            toggleBtnCuota();
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        initLotes();
        setupRegistro();
    });
    /////////////////
    let tipoCambio = null;

    const baseCRC = {
        gasto: null,
        cuota: null,
        ingreso: null
    };

    function parseMoney(val) {
        if (!val) return null;
        return Number(val.toString().replace(/[₡$\s,]/g, ""));
    }

    function formatCRC(n) {
        if (n == null) return "";
        return "₡" + n.toLocaleString("es-CR", { maximumFractionDigits: 0 });
    }

    function formatUSD(n) {
        if (n == null) return "";
        return "$" + n.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }


    async function cargarTipoCambio() {
        if (tipoCambio) return tipoCambio;

        const res = await fetch("/Ventas/TipoCambioActual");
        if (!res.ok) return null;

        const data = await res.json();
        if (!data.ok) return null;

        tipoCambio = Number(data.tipoCambio);
        document.getElementById("tipoCambioInput").value =
            tipoCambio.toLocaleString("es-CR", { minimumFractionDigits: 2 });

        return tipoCambio;
    }

    function aplicarMoneda(moneda) {

        const gastoInput = document.getElementById("gastoFormalizacion");
        const cuotaInput = document.getElementById("cuotaMensual");
        const ingresoInput = document.getElementById("ingresoNeto");

        if (moneda === "CRC") {
            if (baseCRC.gasto != null) gastoInput.value = formatCRC(baseCRC.gasto);
            if (baseCRC.cuota != null) cuotaInput.value = formatCRC(baseCRC.cuota);
            if (baseCRC.ingreso != null) ingresoInput.value = formatCRC(baseCRC.ingreso);
            return;
        }

        if (!tipoCambio || tipoCambio <= 0) {
            alert("No hay tipo de cambio disponible.");
            document.getElementById("monedaSelect").value = "CRC";
            aplicarMoneda("CRC");
            return;
        }

        if (baseCRC.gasto != null) gastoInput.value = formatUSD(baseCRC.gasto / tipoCambio);
        if (baseCRC.cuota != null) cuotaInput.value = formatUSD(baseCRC.cuota / tipoCambio);
        if (baseCRC.ingreso != null) ingresoInput.value = formatUSD(baseCRC.ingreso / tipoCambio);
    }

    document.getElementById("monedaSelect")?.addEventListener("change", async function () {
        if (this.value === "USD") {
            const tc = await cargarTipoCambio();
            if (!tc) {
                this.value = "CRC";
                aplicarMoneda("CRC");
                return;
            }
        }
        aplicarMoneda(this.value);
    });
})();
