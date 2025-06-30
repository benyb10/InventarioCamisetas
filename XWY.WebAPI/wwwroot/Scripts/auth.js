document.addEventListener("DOMContentLoaded", () => {
    const loginForm = document.getElementById("loginForm");
    const errorMessageDiv = document.getElementById("login-error");

    if (loginForm) {
        loginForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            errorMessageDiv.textContent = "";

            const email = e.target.email.value;
            const password = e.target.password.value;

            try {
                // ---> CORRECCIÓN CLAVE AQUÍ <---
                // Apuntamos a la URL base actual, usando el puerto correcto.
                const response = await fetch("/api/auth/login", {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ email, password })
                });

                const result = await response.json();

                if (response.ok && result.success) {
                    localStorage.setItem("authToken", result.data.token);
                    localStorage.setItem("userData", JSON.stringify(result.data.usuario));
                    window.location.href = "/Web/dashboard.html";
                } else {
                    errorMessageDiv.textContent = result.message || "Error al iniciar sesión.";
                }
            } catch (error) {
                errorMessageDiv.textContent = "No se pudo conectar con el servidor.";
                console.error("Error en el fetch:", error);
            }
        });
    }
});