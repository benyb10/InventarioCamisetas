// Objeto global de la aplicación que actuará como nuestro orquestador
const app = {
    // Propiedades compartidas
    token: null,
    contentArea: null,
    modal: null,
    modalBody: null,

    // Inicialización principal de la aplicación
    init: function () {
        this.token = localStorage.getItem("authToken");
        if (!this.token) {
            window.location.href = "/Web/index.html";
            return;
        }

        // Cache de elementos del DOM
        this.contentArea = document.getElementById("content-area");
        this.modal = document.getElementById("actionModal");
        this.modalBody = document.getElementById("modal-body");

        const userData = JSON.parse(localStorage.getItem("userData"));
        document.getElementById("userName").textContent = userData.nombres;

        this.setupEventListeners();
    },

    // Configura los listeners de eventos principales
    setupEventListeners: function () {
        document.getElementById("logoutBtn").addEventListener("click", this.logout);
        document.querySelector(".close-button").onclick = () => this.modal.style.display = "none";
        window.onclick = (event) => {
            if (event.target == this.modal) this.modal.style.display = "none";
        };

        // Llama a los módulos correspondientes al hacer clic en el menú
        document.getElementById("menu-usuarios").addEventListener("click", (e) => {
            e.preventDefault();
            app.users.init(); // Llama al módulo de usuarios
        });
        document.getElementById("menu-articulos").addEventListener("click", (e) => {
            e.preventDefault();
            app.articles.init(); // Llama al módulo de artículos
        });
    },

    // --- FUNCIONES AUXILIARES GLOBALES ---
    logout: function () {
        localStorage.removeItem("authToken");
        localStorage.removeItem("userData");
        window.location.href = "/Web/index.html";
    },

    fetchWithAuth: async function (url, options = {}) {
        const headers = { ...options.headers, 'Authorization': `Bearer ${this.token}` };
        if (options.body) headers['Content-Type'] = 'application/json';
        try {
            const response = await fetch(url, { ...options, headers });
            return await response.json();
        } catch (error) {
            return { success: false, message: 'Error de conexión.' };
        }
    },

    renderPagination: function (pagedData) {
        let html = '';
        if (pagedData.tienePaginaAnterior) html += `<button class="btn-pagination" data-page="${pagedData.paginaActual - 1}">Anterior</button>`;
        html += `<span> Página ${pagedData.paginaActual} de ${pagedData.totalPaginas} </span>`;
        if (pagedData.tienePaginaSiguiente) html += `<button class="btn-pagination" data-page="${pagedData.paginaActual + 1}">Siguiente</button>`;
        return html;
    }
};

// Iniciar la aplicación principal cuando el DOM esté listo
document.addEventListener("DOMContentLoaded", () => app.init());