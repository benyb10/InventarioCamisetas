const userModule = {
    users: [],
    roles: [],
    currentPage: 1,
    pageSize: 10,
    totalPages: 0,
    totalRecords: 0,

    init: function () {
        this.loadUsers();
        this.loadRoles();
        this.renderUserInterface();
    },

    renderUserInterface: function () {
        app.contentArea.innerHTML = `
            <div class="module-content">
                <div class="module-header">
                    <h2>Gestión de Usuarios</h2>
                    <button class="btn btn-primary" onclick="userModule.showCreateForm()">
                        ➕ Nuevo Usuario
                    </button>
                </div>
                
                <div class="filters-section">
                    <div class="search-container">
                        <input type="text" id="userSearch" placeholder="Buscar por nombre, email o cédula..." class="search-input">
                        <button onclick="userModule.searchUsers()" class="btn btn-secondary">🔍</button>
                    </div>
                    <div class="filter-container">
                        <select id="roleFilter" onchange="userModule.filterByRole()" class="filter-select">
                            <option value="">Todos los roles</option>
                        </select>
                        <select id="statusFilter" onchange="userModule.filterByStatus()" class="filter-select">
                            <option value="">Todos los estados</option>
                            <option value="true">Activos</option>
                            <option value="false">Inactivos</option>
                        </select>
                    </div>
                </div>

                <div class="table-container">
                    <div class="table-header">
                        <span>Total de usuarios: <strong id="totalUsersCount">0</strong></span>
                        <div class="table-actions">
                            <button onclick="userModule.exportUsers()" class="btn btn-ghost">📊 Exportar</button>
                            <button onclick="userModule.refreshUsers()" class="btn btn-ghost">🔄 Actualizar</button>
                        </div>
                    </div>
                    
                    <div id="usersTableContainer">
                        <div class="loading-spinner">Cargando usuarios...</div>
                    </div>
                    
                    <div id="paginationContainer" class="pagination-container">
                    </div>
                </div>
            </div>
        `;
    },

    async loadUsers(page = 1, search = '', roleFilter = '', statusFilter = '') {
        try {
            const params = new URLSearchParams({
                pagina: page,
                registrosPorPagina: this.pageSize
            });

            if (search) params.append('buscar', search);
            if (roleFilter) params.append('rolId', roleFilter);
            if (statusFilter !== '') params.append('activo', statusFilter);

            const response = await app.fetchWithAuth(`/api/Usuario?${params}`);

            if (response.success) {
                this.users = response.data.items || [];
                this.currentPage = response.data.currentPage || 1;
                this.totalPages = response.data.totalPages || 1;
                this.totalRecords = response.data.totalRecords || 0;

                this.renderUsersTable();
                this.renderPagination();
                this.updateUserCount();
            } else {
                app.showMessage(response.message || 'Error al cargar usuarios', 'error');
                this.renderEmptyTable();
            }
        } catch (error) {
            console.error('Error cargando usuarios:', error);
            app.showMessage('Error de conexión al cargar usuarios', 'error');
            this.renderEmptyTable();
        }
    },

    async loadRoles() {
        try {
            const response = await app.fetchWithAuth('/api/Catalogo/roles');
            if (response.success) {
                this.roles = response.data || [];
                this.populateRoleFilters();
            }
        } catch (error) {
            console.error('Error cargando roles:', error);
        }
    },

    populateRoleFilters() {
        const roleFilter = document.getElementById('roleFilter');
        if (roleFilter && this.roles.length > 0) {
            roleFilter.innerHTML = '<option value="">Todos los roles</option>';
            this.roles.forEach(role => {
                roleFilter.innerHTML += `<option value="${role.id}">${role.nombre}</option>`;
            });
        }
    },

    renderUsersTable() {
        const container = document.getElementById('usersTableContainer');

        if (!this.users || this.users.length === 0) {
            this.renderEmptyTable();
            return;
        }

        const tableHTML = `
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Cédula</th>
                        <th>Nombre Completo</th>
                        <th>Email</th>
                        <th>Teléfono</th>
                        <th>Rol</th>
                        <th>Estado</th>
                        <th>Último Acceso</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    ${this.users.map(user => `
                        <tr>
                            <td>${user.cedula}</td>
                            <td>
                                <div class="user-info">
                                    <strong>${user.nombres} ${user.apellidos}</strong>
                                </div>
                            </td>
                            <td>${user.email}</td>
                            <td>${user.telefono || '-'}</td>
                            <td>
                                <span class="role-badge role-${user.rolId}">
                                    ${user.rolNombre}
                                </span>
                            </td>
                            <td>
                                <span class="status-badge ${user.activo ? 'active' : 'inactive'}">
                                    ${user.activo ? 'Activo' : 'Inactivo'}
                                </span>
                            </td>
                            <td>${user.fechaUltimoAcceso ? this.formatDate(user.fechaUltimoAcceso) : 'Nunca'}</td>
                            <td>
                                <div class="action-buttons">
                                    <button onclick="userModule.showEditForm(${user.id})" 
                                            class="btn btn-sm btn-primary" title="Editar">
                                        ✏️
                                    </button>
                                    <button onclick="userModule.toggleUserStatus(${user.id}, ${!user.activo})" 
                                            class="btn btn-sm ${user.activo ? 'btn-warning' : 'btn-success'}" 
                                            title="${user.activo ? 'Desactivar' : 'Activar'}">
                                        ${user.activo ? '🚫' : '✅'}
                                    </button>
                                    <button onclick="userModule.showChangePasswordForm(${user.id})" 
                                            class="btn btn-sm btn-secondary" title="Cambiar Contraseña">
                                        🔑
                                    </button>
                                    ${!user.activo ? `
                                        <button onclick="userModule.confirmDeleteUser(${user.id})" 
                                                class="btn btn-sm btn-danger" title="Eliminar">
                                            🗑️
                                        </button>
                                    ` : ''}
                                </div>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;

        container.innerHTML = tableHTML;
    },

    renderEmptyTable() {
        const container = document.getElementById('usersTableContainer');
        container.innerHTML = `
            <div class="empty-state">
                <div class="empty-icon">👥</div>
                <h3>No hay usuarios para mostrar</h3>
                <p>No se encontraron usuarios con los filtros actuales.</p>
                <button onclick="userModule.showCreateForm()" class="btn btn-primary">
                    ➕ Crear Primer Usuario
                </button>
            </div>
        `;
    },

    renderPagination() {
        const container = document.getElementById('paginationContainer');

        if (this.totalPages <= 1) {
            container.innerHTML = '';
            return;
        }

        const startPage = Math.max(1, this.currentPage - 2);
        const endPage = Math.min(this.totalPages, startPage + 4);

        let paginationHTML = `
            <div class="pagination">
                <button onclick="userModule.goToPage(1)" 
                        ${this.currentPage === 1 ? 'disabled' : ''} 
                        class="btn btn-sm btn-secondary">
                    ⏮️ Primera
                </button>
                <button onclick="userModule.goToPage(${this.currentPage - 1})" 
                        ${this.currentPage === 1 ? 'disabled' : ''} 
                        class="btn btn-sm btn-secondary">
                    ⏪ Anterior
                </button>
        `;

        for (let i = startPage; i <= endPage; i++) {
            paginationHTML += `
                <button onclick="userModule.goToPage(${i})" 
                        class="btn btn-sm ${i === this.currentPage ? 'btn-primary' : 'btn-ghost'}">
                    ${i}
                </button>
            `;
        }

        paginationHTML += `
            <button onclick="userModule.goToPage(${this.currentPage + 1})" 
                    ${this.currentPage === this.totalPages ? 'disabled' : ''} 
                    class="btn btn-sm btn-secondary">
                Siguiente ⏩
            </button>
            <button onclick="userModule.goToPage(${this.totalPages})" 
                    ${this.currentPage === this.totalPages ? 'disabled' : ''} 
                    class="btn btn-sm btn-secondary">
                Última ⏭️
            </button>
        </div>
        `;

        container.innerHTML = paginationHTML;
    },

    updateUserCount() {
        const countElement = document.getElementById('totalUsersCount');
        if (countElement) {
            countElement.textContent = this.totalRecords;
        }
    },

    goToPage(page) {
        if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
            this.currentPage = page;
            this.loadUsers(page, this.getCurrentSearch(), this.getCurrentRoleFilter(), this.getCurrentStatusFilter());
        }
    },

    getCurrentSearch() {
        const searchInput = document.getElementById('userSearch');
        return searchInput ? searchInput.value.trim() : '';
    },

    getCurrentRoleFilter() {
        const roleFilter = document.getElementById('roleFilter');
        return roleFilter ? roleFilter.value : '';
    },

    getCurrentStatusFilter() {
        const statusFilter = document.getElementById('statusFilter');
        return statusFilter ? statusFilter.value : '';
    },

    searchUsers() {
        this.currentPage = 1;
        this.loadUsers(1, this.getCurrentSearch(), this.getCurrentRoleFilter(), this.getCurrentStatusFilter());
    },

    filterByRole() {
        this.currentPage = 1;
        this.loadUsers(1, this.getCurrentSearch(), this.getCurrentRoleFilter(), this.getCurrentStatusFilter());
    },

    filterByStatus() {
        this.currentPage = 1;
        this.loadUsers(1, this.getCurrentSearch(), this.getCurrentRoleFilter(), this.getCurrentStatusFilter());
    },

    refreshUsers() {
        this.loadUsers(this.currentPage, this.getCurrentSearch(), this.getCurrentRoleFilter(), this.getCurrentStatusFilter());
    },

    showCreateForm() {
        const formHTML = `
            <form id="createUserForm" onsubmit="userModule.createUser(event)">
                <div class="form-row">
                    <div class="form-group">
                        <label for="createCedula">Cédula <span class="required">*</span></label>
                        <input type="text" id="createCedula" maxlength="10" required>
                        <small class="form-hint">Ingrese la cédula de identidad</small>
                    </div>
                    <div class="form-group">
                        <label for="createTelefono">Teléfono</label>
                        <input type="tel" id="createTelefono" maxlength="10">
                        <small class="form-hint">Número de teléfono</small>
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="createNombres">Nombres <span class="required">*</span></label>
                        <input type="text" id="createNombres" required>
                        <small class="form-hint">Nombres completos</small>
                    </div>
                    <div class="form-group">
                        <label for="createApellidos">Apellidos <span class="required">*</span></label>
                        <input type="text" id="createApellidos" required>
                        <small class="form-hint">Apellidos completos</small>
                    </div>
                </div>
                
                <div class="form-group">
                    <label for="createEmail">Email <span class="required">*</span></label>
                    <input type="email" id="createEmail" required>
                    <small class="form-hint">Dirección de correo electrónico</small>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="createPassword">Contraseña <span class="required">*</span></label>
                        <input type="password" id="createPassword" minlength="6" required>
                        <small class="form-hint">Mínimo 6 caracteres</small>
                    </div>
                    <div class="form-group">
                        <label for="createRol">Rol <span class="required">*</span></label>
                        <select id="createRol" required>
                            <option value="">Seleccione un rol</option>
                            ${this.roles.map(role => `
                                <option value="${role.id}">${role.nombre}</option>
                            `).join('')}
                        </select>
                    </div>
                </div>
                
                <div class="form-actions">
                    <button type="button" class="btn btn-secondary" onclick="app.closeModal()">
                        Cancelar
                    </button>
                    <button type="submit" class="btn btn-primary">
                        <span class="btn-text">Crear Usuario</span>
                        <div class="spinner" style="display: none;"></div>
                    </button>
                </div>
            </form>
        `;

        app.showModal('Crear Nuevo Usuario', formHTML);
        this.setupFormValidation('create');
    },

    async showEditForm(userId) {
        try {
            const response = await app.fetchWithAuth(`/api/Usuario/${userId}`);

            if (!response.success) {
                app.showMessage(response.message || 'Error al cargar datos del usuario', 'error');
                return;
            }

            const user = response.data;

            const formHTML = `
                <form id="editUserForm" onsubmit="userModule.updateUser(event, ${userId})">
                    <div class="form-row">
                        <div class="form-group">
                            <label for="editCedula">Cédula <span class="required">*</span></label>
                            <input type="text" id="editCedula" value="${user.cedula}" maxlength="10" required>
                        </div>
                        <div class="form-group">
                            <label for="editTelefono">Teléfono</label>
                            <input type="tel" id="editTelefono" value="${user.telefono || ''}" maxlength="10">
                        </div>
                    </div>
                    
                    <div class="form-row">
                        <div class="form-group">
                            <label for="editNombres">Nombres <span class="required">*</span></label>
                            <input type="text" id="editNombres" value="${user.nombres}" required>
                        </div>
                        <div class="form-group">
                            <label for="editApellidos">Apellidos <span class="required">*</span></label>
                            <input type="text" id="editApellidos" value="${user.apellidos}" required>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label for="editEmail">Email <span class="required">*</span></label>
                        <input type="email" id="editEmail" value="${user.email}" required>
                    </div>
                    
                    <div class="form-row">
                        <div class="form-group">
                            <label for="editRol">Rol <span class="required">*</span></label>
                            <select id="editRol" required>
                                <option value="">Seleccione un rol</option>
                                ${this.roles.map(role => `
                                    <option value="${role.id}" ${role.id === user.rolId ? 'selected' : ''}>
                                        ${role.nombre}
                                    </option>
                                `).join('')}
                            </select>
                        </div>
                        <div class="form-group">
                            <label for="editActivo">Estado <span class="required">*</span></label>
                            <select id="editActivo" required>
                                <option value="true" ${user.activo ? 'selected' : ''}>Activo</option>
                                <option value="false" ${!user.activo ? 'selected' : ''}>Inactivo</option>
                            </select>
                        </div>
                    </div>
                    
                    <div class="form-info">
                        <p><strong>Fecha de creación:</strong> ${this.formatDate(user.fechaCreacion)}</p>
                        <p><strong>Último acceso:</strong> ${user.fechaUltimoAcceso ? this.formatDate(user.fechaUltimoAcceso) : 'Nunca'}</p>
                    </div>
                    
                    <div class="form-actions">
                        <button type="button" class="btn btn-secondary" onclick="app.closeModal()">
                            Cancelar
                        </button>
                        <button type="submit" class="btn btn-primary">
                            <span class="btn-text">Actualizar Usuario</span>
                            <div class="spinner" style="display: none;"></div>
                        </button>
                    </div>
                </form>
            `;

            app.showModal('Editar Usuario', formHTML);
            this.setupFormValidation('edit');
        } catch (error) {
            console.error('Error cargando usuario:', error);
            app.showMessage('Error al cargar datos del usuario', 'error');
        }
    },

    showChangePasswordForm(userId) {
        const formHTML = `
            <form id="changePasswordForm" onsubmit="userModule.changePassword(event, ${userId})">
                <div class="form-group">
                    <label for="currentPassword">Contraseña Actual <span class="required">*</span></label>
                    <input type="password" id="currentPassword" required>
                    <small class="form-hint">Ingrese la contraseña actual del usuario</small>
                </div>
                
                <div class="form-group">
                    <label for="newPassword">Nueva Contraseña <span class="required">*</span></label>
                    <input type="password" id="newPassword" minlength="6" required>
                    <small class="form-hint">Mínimo 6 caracteres</small>
                </div>
                
                <div class="form-group">
                    <label for="confirmPassword">Confirmar Nueva Contraseña <span class="required">*</span></label>
                    <input type="password" id="confirmPassword" minlength="6" required>
                    <small class="form-hint">Repita la nueva contraseña</small>
                </div>
                
                <div class="form-actions">
                    <button type="button" class="btn btn-secondary" onclick="app.closeModal()">
                        Cancelar
                    </button>
                    <button type="submit" class="btn btn-primary">
                        <span class="btn-text">Cambiar Contraseña</span>
                        <div class="spinner" style="display: none;"></div>
                    </button>
                </div>
            </form>
        `;

        app.showModal('Cambiar Contraseña', formHTML);
    },

    setupFormValidation(formType) {
        const cedulaInput = document.getElementById(`${formType}Cedula`);
        const telefonoInput = document.getElementById(`${formType}Telefono`);

        if (cedulaInput) {
            cedulaInput.addEventListener('input', function (e) {
                e.target.value = e.target.value.replace(/\D/g, '').substring(0, 10);
            });
        }

        if (telefonoInput) {
            telefonoInput.addEventListener('input', function (e) {
                e.target.value = e.target.value.replace(/\D/g, '').substring(0, 10);
            });
        }
    },

    async createUser(event) {
        event.preventDefault();

        const submitBtn = event.target.querySelector('button[type="submit"]');
        const btnText = submitBtn.querySelector('.btn-text');
        const spinner = submitBtn.querySelector('.spinner');

        const formData = {
            cedula: document.getElementById('createCedula').value.trim(),
            nombres: document.getElementById('createNombres').value.trim(),
            apellidos: document.getElementById('createApellidos').value.trim(),
            email: document.getElementById('createEmail').value.trim(),
            telefono: document.getElementById('createTelefono').value.trim(),
            password: document.getElementById('createPassword').value,
            rolId: parseInt(document.getElementById('createRol').value)
        };

        if (!this.validateUserForm(formData, 'create')) {
            return;
        }

        this.setLoadingState(true, submitBtn, btnText, spinner);

        try {
            const response = await app.fetchWithAuth('/api/Usuario', {
                method: 'POST',
                body: JSON.stringify(formData)
            });

            if (response.success) {
                app.showMessage('Usuario creado exitosamente', 'success');
                app.closeModal();
                this.refreshUsers();
            } else {
                app.showMessage(response.message || 'Error al crear usuario', 'error');
            }
        } catch (error) {
            console.error('Error creando usuario:', error);
            app.showMessage('Error de conexión al crear usuario', 'error');
        } finally {
            this.setLoadingState(false, submitBtn, btnText, spinner);
        }
    },

    async updateUser(event, userId) {
        event.preventDefault();

        const submitBtn = event.target.querySelector('button[type="submit"]');
        const btnText = submitBtn.querySelector('.btn-text');
        const spinner = submitBtn.querySelector('.spinner');

        const formData = {
            id: userId,
            cedula: document.getElementById('editCedula').value.trim(),
            nombres: document.getElementById('editNombres').value.trim(),
            apellidos: document.getElementById('editApellidos').value.trim(),
            email: document.getElementById('editEmail').value.trim(),
            telefono: document.getElementById('editTelefono').value.trim(),
            rolId: parseInt(document.getElementById('editRol').value),
            activo: document.getElementById('editActivo').value === 'true'
        };

        if (!this.validateUserForm(formData, 'edit')) {
            return;
        }

        this.setLoadingState(true, submitBtn, btnText, spinner);

        try {
            const response = await app.fetchWithAuth(`/api/Usuario/${userId}`, {
                method: 'PUT',
                body: JSON.stringify(formData)
            });

            if (response.success) {
                app.showMessage('Usuario actualizado exitosamente', 'success');
                app.closeModal();
                this.refreshUsers();
            } else {
                app.showMessage(response.message || 'Error al actualizar usuario', 'error');
            }
        } catch (error) {
            console.error('Error actualizando usuario:', error);
            app.showMessage('Error de conexión al actualizar usuario', 'error');
        } finally {
            this.setLoadingState(false, submitBtn, btnText, spinner);
        }
    },

    async changePassword(event, userId) {
        event.preventDefault();

        const submitBtn = event.target.querySelector('button[type="submit"]');
        const btnText = submitBtn.querySelector('.btn-text');
        const spinner = submitBtn.querySelector('.spinner');

        const currentPassword = document.getElementById('currentPassword').value;
        const newPassword = document.getElementById('newPassword').value;
        const confirmPassword = document.getElementById('confirmPassword').value;

        if (newPassword !== confirmPassword) {
            app.showMessage('Las contraseñas no coinciden', 'error');
            return;
        }

        if (newPassword.length < 6) {
            app.showMessage('La nueva contraseña debe tener al menos 6 caracteres', 'error');
            return;
        }

        const formData = {
            id: userId,
            currentPassword: currentPassword,
            newPassword: newPassword
        };

        this.setLoadingState(true, submitBtn, btnText, spinner);

        try {
            const response = await app.fetchWithAuth('/api/Usuario/change-password', {
                method: 'POST',
                body: JSON.stringify(formData)
            });

            if (response.success) {
                app.showMessage('Contraseña cambiada exitosamente', 'success');
                app.closeModal();
            } else {
                app.showMessage(response.message || 'Error al cambiar contraseña', 'error');
            }
        } catch (error) {
            console.error('Error cambiando contraseña:', error);
            app.showMessage('Error de conexión al cambiar contraseña', 'error');
        } finally {
            this.setLoadingState(false, submitBtn, btnText, spinner);
        }
    },

    async toggleUserStatus(userId, newStatus) {
        const action = newStatus ? 'activar' : 'desactivar';
        const user = this.users.find(u => u.id === userId);

        app.showConfirm(
            `¿Está seguro que desea ${action} al usuario ${user.nombres} ${user.apellidos}?`,
            async () => {
                try {
                    const response = await app.fetchWithAuth(`/api/Usuario/${userId}/toggle-status`, {
                        method: 'PUT',
                        body: JSON.stringify({ activo: newStatus })
                    });

                    if (response.success) {
                        app.showMessage(`Usuario ${action} exitosamente`, 'success');
                        this.refreshUsers();
                    } else {
                        app.showMessage(response.message || `Error al ${action} usuario`, 'error');
                    }
                } catch (error) {
                    console.error(`Error ${action} usuario:`, error);
                    app.showMessage(`Error de conexión al ${action} usuario`, 'error');
                }
            }
        );
    },

    confirmDeleteUser(userId) {
        const user = this.users.find(u => u.id === userId);

        app.showConfirm(
            `¿Está seguro que desea ELIMINAR permanentemente al usuario ${user.nombres} ${user.apellidos}? Esta acción no se puede deshacer.`,
            () => this.deleteUser(userId)
        );
    },

    async deleteUser(userId) {
        try {
            const response = await app.fetchWithAuth(`/api/Usuario/${userId}`, {
                method: 'DELETE'
            });

            if (response.success) {
                app.showMessage('Usuario eliminado exitosamente', 'success');
                this.refreshUsers();
            } else {
                app.showMessage(response.message || 'Error al eliminar usuario', 'error');
            }
        } catch (error) {
            console.error('Error eliminando usuario:', error);
            app.showMessage('Error de conexión al eliminar usuario', 'error');
        }
    },

    validateUserForm(formData, formType) {
        if (!formData.cedula || !formData.nombres || !formData.apellidos || !formData.email || !formData.rolId) {
            app.showMessage('Por favor, complete todos los campos obligatorios', 'error');
            return false;
        }

        if (formData.cedula.length !== 10) {
            app.showMessage('La cédula debe tener 10 dígitos', 'error');
            return false;
        }

        if (!this.validateCedula(formData.cedula)) {
            app.showMessage('La cédula ingresada no es válida', 'error');
            return false;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(formData.email)) {
            app.showMessage('Por favor, ingrese un email válido', 'error');
            return false;
        }

        if (formType === 'create' && (!formData.password || formData.password.length < 6)) {
            app.showMessage('La contraseña debe tener al menos 6 caracteres', 'error');
            return false;
        }

        if (formData.telefono && formData.telefono.length < 7) {
            app.showMessage('El teléfono debe tener al menos 7 dígitos', 'error');
            return false;
        }

        return true;
    },

    validateCedula(cedula) {
        if (cedula.length !== 10) return false;

        const digits = cedula.split('').map(Number);
        const lastDigit = digits[9];

        let sum = 0;
        for (let i = 0; i < 9; i++) {
            let digit = digits[i];
            if (i % 2 === 0) {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }
            sum += digit;
        }

        const expectedLastDigit = (10 - (sum % 10)) % 10;
        return lastDigit === expectedLastDigit;
    },

    setLoadingState(loading, btn, btnText, spinner) {
        btn.disabled = loading;
        if (loading) {
            btnText.style.display = 'none';
            spinner.style.display = 'inline-block';
            btn.classList.add('loading');
        } else {
            btnText.style.display = 'inline';
            spinner.style.display = 'none';
            btn.classList.remove('loading');
        }
    },

    formatDate(dateString) {
        if (!dateString) return '-';

        const date = new Date(dateString);
        return date.toLocaleDateString('es-EC', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        });
    },

    exportUsers() {
        if (!this.users || this.users.length === 0) {
            app.showMessage('No hay usuarios para exportar', 'warning');
            return;
        }

        const csvContent = this.generateCSV();
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');

        if (link.download !== undefined) {
            const url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', `usuarios_${new Date().toISOString().split('T')[0]}.csv`);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

            app.showMessage('Usuarios exportados exitosamente', 'success');
        }
    },

    generateCSV() {
        const headers = ['Cédula', 'Nombres', 'Apellidos', 'Email', 'Teléfono', 'Rol', 'Estado', 'Fecha Creación', 'Último Acceso'];
        const csvRows = [headers.join(',')];

        this.users.forEach(user => {
            const row = [
                user.cedula,
                `"${user.nombres}"`,
                `"${user.apellidos}"`,
                user.email,
                user.telefono || '',
                `"${user.rolNombre}"`,
                user.activo ? 'Activo' : 'Inactivo',
                this.formatDate(user.fechaCreacion),
                user.fechaUltimoAcceso ? this.formatDate(user.fechaUltimoAcceso) : 'Nunca'
            ];
            csvRows.push(row.join(','));
        });

        return csvRows.join('\n');
    }
};

window.userModule = userModule;