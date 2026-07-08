// Configuracion central para todos los scripts de prueba k6
// Modifica BASE_URL si el servidor cambia

export const BASE_URL = 'http://srv1550723.hstgr.cloud';

export const CREDENCIALES = {
  correo: 'rootFusion@fusion.com',
  password: 'rootFusion2026GRECIA',
};

// Umbrales de rendimiento segun requisitos del proyecto
export const UMBRALES = {
  consultas_ms: 3000,  // Consultas a BD <= 3 segundos
  registros_ms: 2500,  // Registros/formularios <= 2.5 segundos
  error_rate: 0.01,    // Tasa de error HTTP < 1%
};
