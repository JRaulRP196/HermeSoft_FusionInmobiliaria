import http from "k6/http";
import { check, sleep } from "k6";
import { BASE_URL, CREDENCIALES } from "../config.js";

// Extrae el token antiforgery que ASP.NET Core inserta en cada formulario
export function obtenerTokenAntiforgery(htmlBody) {
  const patrones = [
    /name="__RequestVerificationToken"[^>]*value="([^"]+)"/,
    /value="([^"]+)"[^>]*name="__RequestVerificationToken"/,
  ];
  for (const patron of patrones) {
    const match = htmlBody.match(patron);
    if (match) return match[1];
  }
  return "";
}

// Realiza login y deja la cookie de sesion en el jar del VU.
// GET /Auth/Login llama APIs externas de BCCR (puede tardar 1-3s); se reintenta
// hasta 3 veces si el servidor no responde con 200 antes de abortar.
export function login() {
  let loginPageRes;
  let intentos = 0;
  const MAX_INTENTOS = 3;

  do {
    if (intentos > 0) sleep(3); // Pausa entre reintentos para no acumular carga en BCCR
    loginPageRes = http.get(`${BASE_URL}/Auth/Login`, {
      redirects: 5,
      timeout: "30s",
    });
    intentos++;
  } while (loginPageRes.status !== 200 && intentos < MAX_INTENTOS);

  check(loginPageRes, {
    "login: pagina cargada": (r) => r.status === 200,
  });

  if (loginPageRes.status !== 200) {
    console.error(`VU ${__VU}: login page fallo tras ${intentos} intentos (HTTP ${loginPageRes.status})`);
    return null;
  }

  const token = obtenerTokenAntiforgery(loginPageRes.body);

  const loginRes = http.post(
    `${BASE_URL}/Auth/Login`,
    {
      Correo: CREDENCIALES.correo,
      Password: CREDENCIALES.password,
      __RequestVerificationToken: token,
    },
    {
      redirects: 5,
      timeout: "30s",
    },
  );

  check(loginRes, {
    "login: autenticacion exitosa (no redirige a Login)": (r) =>
      !r.url.includes("Login") && !r.url.includes("login"),
    "login: sin error HTTP": (r) => r.status < 400,
  });

  sleep(0.3);
  return loginRes;
}
