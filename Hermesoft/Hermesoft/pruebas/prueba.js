// script-banco.js
import http from "k6/http";
import { check, sleep } from "k6";
import { login } from "./helpers/auth.js";
import { BASE_URL, UMBRALES } from "./config.js";

export let options = {
  stages: [
    { duration: "10s", target: 5 }, // Calentamiento
    { duration: "10s", target: 10 }, // Rampa
    { duration: "10s", target: 10 }, // Carga sostenida
    { duration: "10s", target: 0 }, // Bajada
  ],
  thresholds: {
    http_req_duration: [`p(95)<${UMBRALES.consultas_ms}`],
    http_req_failed: [`rate<${UMBRALES.error_rate}`],
  },
};

let loggedIn = false; // Estado de sesión por VU

export default function () {
  // ── Login (solo una vez por VU) ──────────────────────────
  if (!loggedIn) {
    // Escalonar logins por número de VU: VU1=1.5s, VU2=3s ... VU10=15s
    // Evita que todos los VUs llamen a /Auth/Login (y por ende a BCCR) al mismo tiempo
    if (__ITER === 0) sleep(__VU * 1.5);

    const resultado = login();
    if (resultado === null) {
      sleep(5); // Pausa antes de reintentar en la siguiente iteración
      return;
    }
    loggedIn = true;
    sleep(2); // Pausa después del login antes de empezar pruebas
  }

  // ── Lectura ──────────────────────────────────────────────
  let res = http.get(`${BASE_URL}/Ventas`);
  check(res, {
    "lectura OK": (r) => r.status === 200,
    "sesion activa": (r) => !r.url.includes("Login"),
    "tiempo OK": (r) => r.timings.duration < UMBRALES.consultas_ms,
  });

  sleep(1.5);
}
