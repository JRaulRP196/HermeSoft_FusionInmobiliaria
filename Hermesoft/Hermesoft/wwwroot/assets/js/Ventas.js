
(function () {
    // Mock: rol de usuario (para anulación). Cambiar según caso real.
    const isAdmin = false; // cambiar a true para probar anulación como admin

    // Helpers localStorage
    const KEY_VENTAS = "hermes_ventas";
    const KEY_BITACORA = "hermes_bitacora";
    const KEY_LOTES = "hermes_lotes_state";

    function readVentas() {
        return JSON.parse(localStorage.getItem(KEY_VENTAS) || "[]");
    }
    function writeVentas(arr) {
        localStorage.setItem(KEY_VENTAS, JSON.stringify(arr));
    }
    function addBitacora(entry) {
        const b = JSON.parse(localStorage.getItem(KEY_BITACORA) || "[]");
        b.push(entry);
        localStorage.setItem(KEY_BITACORA, JSON.stringify(b));
    }

    // Init: ensure lotes persisted and sample
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

    // Util: generar contrato único (timestamp + random)
    function generarContrato() {
        const ts = Date.now().toString(36);
        const rnd = Math.floor(Math.random() * 9000) + 1000;
        return `CTR-${ts}-${rnd}`;
    }

    // Cálculos: prima, gasto formalización, cuota mensual (sistema simple: amortización fija)
    function calcularPrima(precio, porcPrima) {
        return Math.round(precio * (porcPrima / 100));
    }
    function calcularGastoFormal(precio, seg = 0, abogados = 0, comision = 0, timbre = 0) {
        const total = precio * ((seg + abogados + comision + timbre) / 100);
        return Math.round(total);
    }
    function calcularCuotaMensual(precio, entrada = 0, tasaAnual = 7, plazoMeses = 360) {
        const monto = precio - (entrada || 0);
        const r = (tasaAnual / 100) / 12;
        if (r === 0) return Math.round(monto / plazoMeses);
        const cuota = monto * (r * Math.pow(1 + r, plazoMeses)) / (Math.pow(1 + r, plazoMeses) - 1);
        return Math.round(cuota);
    }

    // Mensajes visuales
    function showMessage(elId, html, type = "success") {
        const el = document.getElementById(elId);
        if (!el) return;
        el.innerHTML = `<div class="alert alert-${type}">${html}</div>`;
        setTimeout(() => { el.innerHTML = ""; }, 6000);
    }

    // ---------- Registro view logic ----------
    function setupRegistro() {
        const form = document.getElementById("ventaForm");
        if (!form) return;

        const cliente = document.getElementById("cliente");
        const lote = document.getElementById("lote");
        const porcPrima = document.getElementById("porcPrima");
        const fechaPrima = document.getElementById("fechaPrima");
        const desglosePrima = document.getElementById("desglosePrima");
        const calcGastoBtn = document.getElementById("calcGastoFormal");
        const gastoFormalizacion = document.getElementById("gastoFormalizacion");
        const porcSeguros = document.getElementById("porcSeguros");
        const porcAbogados = document.getElementById("porcAbogados");
        const porcComision = document.getElementById("porcComision");
        const porcTimbre = document.getElementById("porcTimbre");
        const plazoMeses = document.getElementById("plazoMeses");
        const tasaInteres = document.getElementById("tasaInteres");
        const cuotaMensual = document.getElementById("cuotaMensual");
        const moneda = document.getElementById("moneda");
        const banco = document.getElementById("banco");
        const tipoAsalariado = document.getElementById("tipoAsalariado");
        const fechaVenta = document.getElementById("fechaVenta");
        const asesor = document.getElementById("asesor");
        const generarPdfBtn = document.getElementById("generarPdfBtn");
        const enviarCorreoBtn = document.getElementById("enviarCorreoBtn");
        const registrarVentaBtn = document.getElementById("registrarVentaBtn");

        // on lote change update precio display and validations
        async function obtenerPrecioLote() {
            const codigo = lote?.value;
            if (!codigo) return null;

            const resp = await fetch(`/Ventas/ObtenerLoteJson?lote=${encodeURIComponent(codigo)}`);
            if (!resp.ok) return null;

            const json = await resp.json();
            const obj = Array.isArray(json) ? (json[0] || null) : json;

            const precio = Number(obj?.PrecioVenta ?? obj?.precioVenta ?? 0);
            return precio > 0 ? precio : null;
        }


        // Historia: no mostrar botón si no hay banco o lote
        function toggleBtnFormalizacion() {
            const tieneLote = !!(lote?.value);
            const tieneBanco = !!(banco?.value);

            if (!calcGastoBtn) return;

            calcGastoBtn.style.display = (tieneLote && tieneBanco) ? "block" : "none";
        }

        // Ejecutar al cargar
        toggleBtnFormalizacion();

        // Ejecutar cuando cambie banco o lote
        banco?.addEventListener("change", toggleBtnFormalizacion);
        lote?.addEventListener("input", toggleBtnFormalizacion);

        function actualizarPrimaYCuota() {
            const precio = obtenerPrecioLote();
            if (!precio) {
                desglosePrima.value = "";
                cuotaMensual.value = "";
                return;
            }
            const primaVal = Number(porcPrima.value || 0);
            const montoPrima = calcularPrima(precio, primaVal);
            desglosePrima.value = `${new Intl.NumberFormat().format(montoPrima)} (${primaVal}%)`;

            const entrada = montoPrima;
            const cuota = calcularCuotaMensual(precio, entrada, Number(tasaInteres.value || 0), Number(plazoMeses.value || 1));
            cuotaMensual.value = `${new Intl.NumberFormat().format(cuota)}`;

            // ingreso neto simple: cuota * 3 (ejemplo de requerimiento)
            document.getElementById("ingresoNeto").value = `${new Intl.NumberFormat().format(cuota * 3)}`;
        }

        lote.addEventListener("change", () => {
            const selected = lote.options[lote.selectedIndex];
            const estado = selected ? selected.getAttribute("data-estado") : null;
            if (estado !== "Disponible") {
                lote.classList.add("is-invalid");
            } else {
                lote.classList.remove("is-invalid");
            }
            actualizarPrimaYCuota();
        });

        porcPrima.addEventListener("input", actualizarPrimaYCuota);
        tasaInteres.addEventListener("input", actualizarPrimaYCuota);
        plazoMeses.addEventListener("input", actualizarPrimaYCuota);

        calcGastoBtn.addEventListener("click", async () => {

            const precio = await obtenerPrecioLote();
            if (!precio) {
                showMessage('formMessages', 'No se pudo obtener el precio del lote.', 'danger');
                return;
            }

            const segurosInputs = document.querySelectorAll("#porcSeguros");
            const porcVida = segurosInputs.length > 0 ? Number(segurosInputs[0].value || 0) : 0;
            const porcDesempleo = segurosInputs.length > 1 ? Number(segurosInputs[1].value || 0) : 0;

            const porcCom = Number(porcComision?.value || 0);
            const porcTim = Number(porcTimbre?.value || 0);
            const porcAbg = Number(porcAbogados?.value || 0);

            // MONTOS
            const montoSeguros = precio * ((porcVida + porcDesempleo) / 100);
            const montoComision = precio * (porcCom / 100);
            const montoTimbre = precio * (porcTim / 100);

            const montoAbogados = precio * (porcAbg / 100);
            const ivaAbogados = montoAbogados * 0.13;
            const totalAbogadosConIva = montoAbogados + ivaAbogados;

            const gastoTotal = montoSeguros + montoComision + montoTimbre + totalAbogadosConIva;

            gastoFormalizacion.value = new Intl.NumberFormat().format(Math.round(gastoTotal));

            showMessage('formMessages', 'Gasto de formalización calculado con IVA incluido en honorarios.', 'success');
        });

        async function cargarTimbreFiscal() {
            try {
                const resp = await fetch("/Calculos/ObtenerTimbre");
                const timbre = await resp.json();

                if (Number(timbre) === -1) {
                    // Caso: no encontrado
                    porcTimbre.value = -1;
                    porcTimbre.readOnly = false;
                    porcTimbre.style.border = "2px solid red";
                    porcTimbre.style.backgroundColor = "#ffe5e5";
                } else {
                    // Caso: encontrado
                    porcTimbre.value = timbre;
                    porcTimbre.readOnly = true;
                    porcTimbre.style.border = "";
                    porcTimbre.style.backgroundColor = "";
                }
            } catch {
                // Si falla la llamada, mismo comportamiento que no encontrado
                porcTimbre.value = -1;
                porcTimbre.readOnly = false;
                porcTimbre.style.border = "2px solid red";
                porcTimbre.style.backgroundColor = "#ffe5e5";
            }
        }

        // Ejecutar al cargar la vista Registro
        cargarTimbreFiscal();


        // PDF generation helper (uses jsPDF)
        async function generarPdf(venta) {
            const { jsPDF } = window.jspdf;
            const doc = new jsPDF({ unit: "pt", format: "a4" });

            doc.setFontSize(14);
            doc.text("Comprobante de Venta", 40, 50);
            doc.setFontSize(10);
            doc.text(`Contrato: ${venta.contrato}`, 40, 75);
            doc.text(`Fecha: ${venta.fecha}`, 40, 90);

            doc.text("Cliente:", 40, 120);
            doc.text(`${venta.cliente}`, 120, 120);

            doc.text("Lote:", 40, 135);
            doc.text(`${venta.lote} (${venta.condominio || ""})`, 120, 135);

            doc.text("Monto Total:", 40, 155);
            doc.text(`${venta.moneda === 'USD' ? '$' : '₡'} ${new Intl.NumberFormat().format(venta.monto)}`, 140, 155);

            doc.autoTable({
                startY: 185,
                head: [["Concepto", "Detalle"]],
                body: [
                    ["Prima %", `${venta.porcPrima}%`],
                    ["Prima monto", `${new Intl.NumberFormat().format(venta.prima)}`],
                    ["Gasto formalización", `${new Intl.NumberFormat().format(venta.gastoFormal)}`],
                    ["Cuota mensual", `${new Intl.NumberFormat().format(venta.cuotaMensual)}`],
                    ["Banco", venta.banco || ""],
                    ["Asesor", venta.asesor || ""]
                ]
            });

            return doc;
        }

        generarPdfBtn?.addEventListener("click", async () => {
            const datos = construyeVentaTemporal();
            if (!datos) return;
            const doc = await generarPdf(datos);
            // preview in new window
            const blob = doc.output("blob");
            const url = URL.createObjectURL(blob);
            window.open(url, "_blank");
        });

        enviarCorreoBtn?.addEventListener("click", async () => {
            const datos = construyeVentaTemporal();
            if (!datos) return;
            const doc = await generarPdf(datos);
            // descargar temporal para adjuntar manualmente o simular envío
            const blob = doc.output("blob");
            // Simulación: abrir cliente de correo con mailto + mensaje (no puede adjuntar PDF desde frontend sin backend)
            const mailto = `mailto:${datos.cliente}?subject=Comprobante%20de%20Venta%20${encodeURIComponent(datos.contrato)}&body=Adjuntamos%20su%20comprobante%20de%20venta.%0AContrato:%20${encodeURIComponent(datos.contrato)}`;
            window.location.href = mailto;
            showMessage('formMessages', 'Se ha abierto el cliente de correo con la dirección del cliente. (Adjunte el PDF manualmente si lo desea)', 'info');
        });

        // Construye objeto venta temporal para preview o registro
        function construyeVentaTemporal() {
            // Validaciones front
            if (!cliente.value) { showMessage('formMessages', 'Cliente es obligatorio', 'danger'); cliente.focus(); return null; }
            const selectedLote = lote.options[lote.selectedIndex];
            if (!selectedLote) { showMessage('formMessages', 'Lote obligatorio', 'danger'); return null; }
            if (selectedLote.getAttribute('data-estado') !== 'Disponible') { showMessage('formMessages', 'El lote seleccionado no está disponible', 'danger'); return null; }
            if (!fechaVenta.value) { showMessage('formMessages', 'Fecha de venta es obligatoria', 'danger'); return null; }
            if (!banco.value) { showMessage('formMessages', 'Banco es obligatorio', 'danger'); return null; }
            const bancoOpt = banco.options[banco.selectedIndex];
            if (bancoOpt && bancoOpt.getAttribute('data-plan-vigente') === 'false') {
                showMessage('formMessages', 'El plan de pago del banco seleccionado no está vigente.', 'danger');
                return null;
            }
            // Prepara objeto
            const precio = obtenerPrecioLote();
            const porcPrimaVal = Number(porcPrima.value || 0);
            const prima = calcularPrima(precio, porcPrimaVal);
            const gastoFormal = calcularGastoFormal(precio, Number(porcSeguros.value || 0), Number(porcAbogados.value || 0), Number(porcComision.value || 0), Number(porcTimbre.value || 0));
            const cuota = calcularCuotaMensual(precio, prima, Number(tasaInteres.value || 0), Number(plazoMeses.value || 1));
            const ventaTemp = {
                id: Date.now(),
                contrato: generarContrato(),
                cliente: cliente.value,
                lote: selectedLote.value,
                condominio: selectedLote.getAttribute('data-condominio') || "",
                fecha: fechaVenta.value,
                monto: precio,
                moneda: moneda.value,
                banco: banco.value,
                tipoAsalariado: tipoAsalariado.value,
                porcPrima: porcPrimaVal,
                prima: prima,
                gastoFormal: gastoFormal,
                cuotaMensual: cuota,
                tasaInteres: Number(tasaInteres.value || 0),
                plazoMeses: Number(plazoMeses.value || 0),
                asesor: asesor.value,
                estado: "En proceso"
            };
            return ventaTemp;
        }

        // Submit: registra la venta en localStorage y actualiza estado del lote
        form.addEventListener("submit", (ev) => {
            ev.preventDefault();
            const venta = construyeVentaTemporal();
            if (!venta) return;

            // Guardar venta
            const ventas = readVentas();
            ventas.push(venta);
            writeVentas(ventas);

            // Actualizar lote
            const lotes = readLotes();
            if (lotes[venta.lote]) {
                lotes[venta.lote].estado = "Vendido";
            } else {
                lotes[venta.lote] = { estado: "Vendido", precio: venta.monto, condominio: venta.condominio };
            }
            writeLotes(lotes);

            // Bitácora
            addBitacora({ accion: "Registro Venta", usuario: venta.asesor, fecha: new Date().toISOString(), detalles: `Contrato ${venta.contrato}` });

            showMessage('formMessages', 'Venta registrada correctamente. Contrato: ' + venta.contrato, 'success');

            // Reset del formulario (opcional)
            setTimeout(() => { window.location.href = '/Ventas/Detalle?contrato=' + encodeURIComponent(venta.contrato); }, 800);
        });

        // inicializar valores
        initLotes();
        actualizarPrimaYCuota();
    }

    // ---------- Historial view logic ----------
    function setupHistorial() {
        const tablaBody = document.querySelector("#tablaVentas tbody");
        const btnFiltrar = document.getElementById("btnFiltrar");
        const btnLimpiar = document.getElementById("btnLimpiar");
        const filterDesde = document.getElementById("filterDesde");
        const filterHasta = document.getElementById("filterHasta");
        const filterCondominio = document.getElementById("filterCondominio");
        const historialMessages = document.getElementById("historialMessages");

        if (!tablaBody) return;

        function renderTabla(ventas) {
            tablaBody.innerHTML = "";
            if (!ventas.length) {
                historialMessages.innerHTML = `<div class="alert alert-danger">No se encontraron resultados</div>`;
                return;
            }
            historialMessages.innerHTML = "";
            ventas.forEach(v => {
                const tr = document.createElement("tr");
                tr.innerHTML = `
          <td>${v.id}</td>
          <td>${v.contrato}</td>
          <td>${v.cliente}</td>
          <td>${v.lote}</td>
          <td>${v.fecha}</td>
          <td>${v.moneda === 'USD' ? '$' : '₡'} ${new Intl.NumberFormat().format(v.monto)}</td>
          <td>${v.estado || ''}</td>
          <td>
            <a class="btn btn-sm btn-info me-1" href="/Ventas/Detalle?contrato=${encodeURIComponent(v.contrato)}">Detalle</a>
            <button class="btn btn-sm btn-danger btn-anular" data-contrato="${v.contrato}">Anular</button>
          </td>
        `;
                tablaBody.appendChild(tr);
            });

            // bind botones anular
            document.querySelectorAll(".btn-anular").forEach(btn => {
                btn.addEventListener("click", (e) => {
                    const contrato = e.currentTarget.getAttribute("data-contrato");
                    anularVenta(contrato);
                });
            });
        }

        function obtenerVentasFiltradas() {
            const ventas = readVentas();
            let res = ventas.slice();
            const desde = filterDesde.value;
            const hasta = filterHasta.value;
            const condo = filterCondominio.value;

            if (desde) res = res.filter(v => v.fecha >= desde);
            if (hasta) res = res.filter(v => v.fecha <= hasta);
            if (condo) res = res.filter(v => (v.condominio || "").includes(condo));
            return res;
        }

        btnFiltrar?.addEventListener("click", () => {
            const filtradas = obtenerVentasFiltradas();
            renderTabla(filtradas);
        });

        btnLimpiar?.addEventListener("click", () => {
            filterDesde.value = "";
            filterHasta.value = "";
            filterCondominio.value = "";
            renderTabla(readVentas());
        });

        // Anulación (frontend): solo admin
        function anularVenta(contrato) {
            const ventas = readVentas();
            const idx = ventas.findIndex(v => v.contrato === contrato);
            if (idx === -1) { showMessage('historialMessages', 'Venta no encontrada', 'danger'); return; }
            if (!isAdmin) {
                showMessage('historialMessages', 'No posee permisos para anular ventas.', 'danger');
                return;
            }
            const motivo = prompt("Ingrese motivo de anulación:");
            if (!motivo) { showMessage('historialMessages', 'Anulación cancelada (no se ingresó motivo).', 'warning'); return; }
            ventas[idx].estado = "Anulada";
            ventas[idx].motivoAnulacion = motivo;
            writeVentas(ventas);

            // liberar lote
            const lotes = readLotes();
            if (lotes[ventas[idx].lote]) lotes[ventas[idx].lote].estado = "Disponible";
            writeLotes(lotes);

            addBitacora({ accion: "Anulación", usuario: "usuario_actual@example.com", fecha: new Date().toISOString(), detalles: `Contrato ${contrato} - Motivo: ${motivo}` });

            showMessage('historialMessages', `Venta ${contrato} anulada.`, 'success');
            renderTabla(obtenerVentasFiltradas());
        }

        // render inicial
        renderTabla(readVentas());
    }

    // ---------- Detalle view logic ----------
    function setupDetalle() {
        const cont = document.getElementById("detalleVentaContent");
        if (!cont) return;
        // Leer query string contrato
        const params = new URLSearchParams(location.search);
        const contrato = params.get("contrato");
        if (!contrato) {
            cont.innerHTML = `<div class="alert alert-warning">No se especificó contrato.</div>`;
            return;
        }
        const ventas = readVentas();
        const venta = ventas.find(v => v.contrato === contrato);
        if (!venta) {
            cont.innerHTML = `<div class="alert alert-warning">Venta no encontrada.</div>`;
            return;
        }

        cont.innerHTML = `
      <div class="row">
        <div class="col-md-6">
          <label class="form-label">Contrato</label>
          <input class="form-control" value="${venta.contrato}" disabled />
        </div>
        <div class="col-md-6">
          <label class="form-label">Fecha</label>
          <input class="form-control" value="${venta.fecha}" disabled />
        </div>
        <div class="col-md-6 mt-3">
          <label class="form-label">Cliente</label>
          <input class="form-control" value="${venta.cliente}" disabled />
        </div>
        <div class="col-md-6 mt-3">
          <label class="form-label">Lote</label>
          <input class="form-control" value="${venta.lote}" disabled />
        </div>
        <div class="col-md-6 mt-3">
          <label class="form-label">Monto</label>
          <input class="form-control" value="${new Intl.NumberFormat().format(venta.monto)} ${venta.moneda === 'USD' ? '$' : '₡'}" disabled />
        </div>
        <div class="col-md-6 mt-3">
          <label class="form-label">Estado</label>
          <input class="form-control" value="${venta.estado || ''}" disabled />
        </div>

        <div class="col-12 mt-4">
          <h6>Desglose</h6>
          <ul>
            <li>Prima: ${new Intl.NumberFormat().format(venta.prima)}</li>
            <li>Gasto de Formalización: ${new Intl.NumberFormat().format(venta.gastoFormal)}</li>
            <li>Cuota mensual estimada: ${new Intl.NumberFormat().format(venta.cuotaMensual)}</li>
            <li>Banco: ${venta.banco}</li>
            <li>Asesor: ${venta.asesor}</li>
          </ul>
        </div>
      </div>
    `;

        // Botón generar PDF
        const btnGen = document.getElementById("detalleGenerarPdf");
        btnGen?.addEventListener("click", async () => {
            const { jsPDF } = window.jspdf;
            const doc = new jsPDF({ unit: "pt", format: "a4" });
            doc.setFontSize(14); doc.text("Comprobante de Venta", 40, 50);
            doc.setFontSize(10); doc.text(`Contrato: ${venta.contrato}`, 40, 75);
            doc.autoTable({
                startY: 100,
                head: [["Campo", "Valor"]],
                body: [
                    ["Cliente", venta.cliente],
                    ["Lote", venta.lote],
                    ["Monto", `${venta.moneda === 'USD' ? '$' : '₡'} ${new Intl.NumberFormat().format(venta.monto)}`],
                    ["Prima", new Intl.NumberFormat().format(venta.prima)],
                    ["Gasto Formalización", new Intl.NumberFormat().format(venta.gastoFormal)]
                ]
            });
            const blob = doc.output("blob");
            const url = URL.createObjectURL(blob);
            window.open(url, "_blank");
        });
    }

    // INIT global (mount appropriate depending on page)
    document.addEventListener("DOMContentLoaded", function () {
        initLotes();
        setupRegistro();
        setupHistorial();
        setupDetalle();
    });

})();
