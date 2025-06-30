document.addEventListener("DOMContentLoaded", () => {
    const registerForm = document.getElementById("registerForm");
    const errorMessageDiv = document.getElementById("register-error");

    if (registerForm) {
        registerForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            errorMessageDiv.textContent = "";

            const userData = {
                cedula: e.target.cedula.value,
                nombres: e.target.nombres.value,
                apellidos: e.target.apellidos.value,
                email: e.target.email.value,
                telefono: e.target.telefono.value,
                password: e.target.password.value,
                rolId: 2 // Por defecto, asignamos el Rol 'Usuario'. El rol 1 suele ser 'Administrador'.
            };

            try {
                const response = await fetch("http://localhost:5064/api/usuario", { // Ajusta el puerto si es necesario
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(userData)
                });

                const result = await response.json();

                if (response.ok && result.success) {
                    alert("¡Registro exitoso! Serás redirigido a la página de inicio de sesión.");
                    window.location.href = "index.html";
                } else {
                    errorMessageDiv.textContent = result.message || "Error en el registro.";
                }
            } catch (error) {
                errorMessageDiv.textContent = "No se pudo conectar con el servidor.";
            }
        });
    }
});