document.addEventListener("DOMContentLoaded", function () {
    // Aplicar la configuración guardada cuando cargue cualquier página
    applyLayoutMode();

    // Cuando el usuario haga clic, alternar el modo
    document.getElementById("toggleLayout").addEventListener("click", function (e) {
        e.preventDefault();

        const current = localStorage.getItem("layoutMode") || "light";

        if (current === "dark") {
            localStorage.setItem("layoutMode", "light");
        } else {
            localStorage.setItem("layoutMode", "dark");
        }

        applyLayoutMode();
    });
});

/* Función que aplica los cambios */
function applyLayoutMode() {
    const mode = localStorage.getItem("layoutMode");

    if (mode === "dark") {
        document.documentElement.setAttribute("data-bs-theme", "dark");
        document.body.classList.add("dark-sidebar");   // Si Tocly usa esta clase
    } else {
        document.documentElement.setAttribute("data-bs-theme", "light");
        document.body.classList.remove("dark-sidebar");
    }
}