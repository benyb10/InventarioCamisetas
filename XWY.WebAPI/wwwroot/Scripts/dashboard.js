const app = {
    token: null,
    userData: null,
    contentArea: null,
    modal: null,
    modalBody: null,
    confirmModal: null,
    currentModule: 'home',

    init: function () {
        this.token = localStorage.getItem("authToken");
        this.userData = JSON.parse(localStorage.getItem("userData") || '{}');

        if (!this.token || !this.userData.id) {
            window.location.href = "/Web/index.html";
            return;
        }

        this.contentArea = document.getElementById("content-area");
        this.modal = document.getElementById("actionModal");
        this.modalBody = document.getElementById("modal-body");
        this.confirmModal = document.getElementById("confirmModal");

        this.setupUserInterface();
        this.setupEventListeners();
        this.loadDashboardStats();
    },

    setupUserInterface: function () {
        document.getElementById("userName").textContent = this.userData.nombres || 'Usuario';
        document.getElementById("welcomeUserName").textContent = this.userData.nombres || 'Usuario';
        document.getElementById("userRole").textContent = this.userData.rolNombre || 'Usuario';
    },

    setupEventListeners: function () {
        document.getElementById("logoutBtn").addEventListener("click", this.logout);
        document.getElementById("refreshBtn").addEventListener("click", () => this.refreshCurrentView());

        document.querySelector(".close-button").onclick = () => this.closeModal();
        document.querySelector(".close-confirm").onclick = () => this.closeConfirmModal();

        window.onclick = (event) => {
            if (event.target === this.modal) this.closeModal();
            if (event.target === this.confirmModal) this.closeConfirmModal();
        };

        this.setupMenuEvents();
    },

    setupMenuEvents: function () {
        const menuItems = document.querySelectorAll('.menu-item');

        menuItems.forEach(item => {
            item.addEventListener('click', (e) => {
                e.preventDefault();

                menuItems.forEach(mi => mi.classList.remove('active'));
                item.classList.add('active');

                const moduleId = item.id.replace('menu-', '');
                this.currentModule = moduleId;
                this.loadModule(moduleId);
            });
        });
    },

    loadModule: function (moduleId) {
        const pageTitle = document.getElementById('pageTitle');

        switch (moduleId) {
            case 'home':
                pageTitle.textContent = 'Dashboard Principal';
                this.loadDashboard();
                break;
            case 'usuarios':
                pageTitle.textContent = 'Gestión de Usuarios';
                if (window.userModule) {
                    window.userModule.init();
                } else {
                    this.loadModuleNotAvailable('Usuarios');
                }
                break;
            case 'articulos':
                pageTitle.textContent = 'Gestión de Artículos';
                if (window.articleModule) {
                    window.articleModule.init();
                } else {
                    this.loadModuleNotAvailable('Artículos');
                }
                break;
            case 'prestamos':
                pageTitle.textContent = 'Gestión de Préstamos';
                if (window.loanModule) {
                    window.loanModule.init();
                } else {
                    this.loadModuleNotAvailable('Préstamos');
                }
                break;
            case 'reportes':
                pageTitle.textContent = 'Reportes del Sistema';
                this.loadReportes();
                break;
            default:
                this.loadDashboard();
        }
    },

    loadModuleNotAvailable: function (moduleName) {
        this.contentArea.innerHTML = `
            <div class="module-content">
                <div class="empty-state">
                    <div class="empty-icon">⚠️</div>
                    <h3>Módulo ${moduleName} No Disponible</h3>
                    <p>El módulo de gestión de ${moduleName.toLowerCase()} no está cargado correctamente.</p>
                    <p>Por favor, verifique que el archivo JavaScript correspondiente esté incluido.</p>
                    <button onclick="location.reload()" class="btn btn-primary">
                        🔄 Recargar Página
                    </button>
                </div>
            </div>
        `;
    },

    loadDashboard: function () {
        this.contentArea.innerHTML = `
            <div class="welcome-section">
                <h2>¡Bienvenido, ${this.userData.nombres}!</h2>
                <p>Selecciona una opción del menú para comenzar a gestionar el inventario.</p>
                
                <div class="dashboard-stats">
                    <div class="stat-card">
                        <div class="stat-icon">📦</div>
                        <div class="stat-content">
                            <h3 id="totalArticulos">-</h3>
                            <p>Total Artículos</p>
                        </div>
                    </div>
                    <div class="stat-card">
                        <div class="stat-icon">📋</div>
                        <div class="stat-content">
                            <h3 id="prestamosPendientes">-</h3>
                            <p>Préstamos Activos</p>
                        </div>
                    </div>
                    <div class="stat-card">
                        <div class="stat-icon">👥</div>
                        <div class="stat-content">
                            <h3 id="totalUsuarios">-</h3>
                            <p>Usuarios Activos</p>
                        </div>
                    </div>
                </div>

                <div class="recent-activity">
                    <h3>Actividad Reciente</h3>
                    <div id="recentActivity" class="activity-list">
                        <p>Cargando actividad reciente...</p>
                    </div>
                </div>
            </div>
        `;
        this.loadDashboardStats();
    },

    loadPrestamos: function () {
        // Esta función ya no se usa, se redirige al módulo de préstamos
        if (window.loanModule) {
            window.loanModule.init();
        } else {
            this.loadModuleNotAvailable('Préstamos');
        }
    },

    loadReportes: function () {
        this.contentArea.innerHTML = `
            <div class="module-content">
                <div class="module-header">
                    <h2>Reportes del Sistema</h2>
                </div>
                <div class="reports-grid">
                    <div class="report-card" onclick="app.generateReport('articulos')">
                        <div class="report-icon">📦</div>
                        <h3>Reporte de Artículos</h3>
                        <p>Inventario completo de artículos</p>
                    </div>
                    <div class="report-card" onclick="app.generateReport('prestamos')">
                        <div class="report-icon">📋</div>
                        <h3>Reporte de Préstamos</h3>
                        <p>Historial de préstamos</p>
                    </div>
                    <div class="report-card" onclick="app.generateReport('usuarios')">
                        <div class="report-icon">👥</div>
                        <h3>Reporte de Usuarios</h3>
                        <p>Lista de usuarios del sistema</p>
                    </div>
                </div>
            </div>
        `;
    },

    async loadDashboardStats() {
        try {
            const [articulosRes, prestamosRes, usuariosRes] = await Promise.all([
                this.fetchWithAuth('/api/Articulo/count'),
                this.fetchWithAuth('/api/Prestamo/active-count'),
                this.fetchWithAuth('/api/Usuario/active-count')
            ]);

            if (articulosRes.success) {
                const totalArticulosEl = document.getElementById('totalArticulos');
                if (totalArticulosEl) totalArticulosEl.textContent = articulosRes.data || '0';
            }

            if (prestamosRes.success) {
                const prestamosPendientesEl = document.getElementById('prestamosPendientes');
                if (prestamosPendientesEl) prestamosPendientesEl.textContent = prestamosRes.data || '0';
            }

            if (usuariosRes.success) {
                const totalUsuariosEl = document.getElementById('totalUsuarios');
                if (totalUsuariosEl) totalUsuariosEl.textContent = usuariosRes.data || '0';
            }
        } catch (error) {
            console.error('Error cargando estadísticas:', error);
        }
    },

    refreshCurrentView: function () {
        this.loadModule(this.currentModule);
    },

    showPrestamoForm: function () {
        this.showModal('Nuevo Préstamo', `
            <form id="prestamoForm">
                <div class="form-group">
                    <label for="usuarioSelect">Usuario:</label>
                    <select id="usuarioSelect" required>
                        <option value="">Seleccione un usuario</option>
                    </select>
                </div>
                <div class="form-group">
                    <label for="articuloSelect">Artículo:</label>
                    <select id="articuloSelect" required>
                        <option value="">Seleccione un artículo</option>
                    </select>
                </div>
                <div class="form-group">
                    <label for="fechaEntrega">Fecha de Entrega:</label>
                    <input type="date" id="fechaEntrega" required>
                </div>
                <div class="form-group">
                    <label for="fechaDevolucion">Fecha de Devolución:</label>
                    <input type="date" id="fechaDevolucion">
                </div>
                <div class="form-group">
                    <label for="observaciones">Observaciones:</label>
                    <textarea id="observaciones" rows="3"></textarea>
                </div>
                <div class="form-actions">
                    <button type="button" class="btn btn-secondary" onclick="app.closeModal()">Cancelar</button>
                    <button type="submit" class="btn btn-primary">Crear Préstamo</button>
                </div>
            </form>
        `);
    },

    generateReport: function (type) {
        console.log(`Generando reporte de ${type}...`);
        this.showMessage(`Generando reporte de ${type}...`, 'info');
    },

    showModal: function (title, content) {
        document.getElementById('modal-title').textContent = title;
        this.modalBody.innerHTML = content;
        this.modal.style.display = 'block';

        setTimeout(() => {
            this.modal.classList.add('show');
        }, 10);
    },

    closeModal: function () {
        this.modal.classList.remove('show');
        setTimeout(() => {
            this.modal.style.display = 'none';
        }, 300);
    },

    showConfirm: function (message, onConfirm) {
        document.getElementById('confirm-message').textContent = message;
        this.confirmModal.style.display = 'block';

        document.getElementById('confirmYes').onclick = () => {
            this.closeConfirmModal();
            if (onConfirm) onConfirm();
        };

        document.getElementById('confirmNo').onclick = () => {
            this.closeConfirmModal();
        };
    },

    closeConfirmModal: function () {
        this.confirmModal.style.display = 'none';
    },

    showMessage: function (message, type = 'info') {
        const messageDiv = document.createElement('div');
        messageDiv.className = `message message-${type}`;
        messageDiv.textContent = message;

        document.body.appendChild(messageDiv);

        setTimeout(() => {
            messageDiv.classList.add('show');
        }, 10);

        setTimeout(() => {
            messageDiv.classList.remove('show');
            setTimeout(() => {
                document.body.removeChild(messageDiv);
            }, 300);
        }, 3000);
    },

    logout: function () {
        if (window.confirm('¿Está seguro que desea cerrar sesión?')) {
            localStorage.removeItem("authToken");
            localStorage.removeItem("userData");
            localStorage.removeItem("tokenExpiration");
            window.location.href = "/Web/index.html";
        }
    },

    fetchWithAuth: async function (url, options = {}) {
        const headers = {
            ...options.headers,
            'Authorization': `Bearer ${this.token}`,
            'Content-Type': 'application/json'
        };

        try {
            const response = await fetch(url, {
                ...options,
                headers
            });

            if (response.status === 401) {
                this.logout();
                return { success: false, message: 'Sesión expirada' };
            }

            return await response.json();
        } catch (error) {
            console.error('Error en fetchWithAuth:', error);
            return { success: false, message: 'Error de conexión' };
        }
    }
};

document.addEventListener('DOMContentLoaded', function () {
    app.init();
});

window.app = app;