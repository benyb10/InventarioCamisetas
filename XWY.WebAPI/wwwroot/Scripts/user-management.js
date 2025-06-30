// Módulo de Usuarios, adjunto al objeto global 'app'
app.users = {
    // Inicializador del módulo
    init: function () {
        this.contentArea = app.contentArea;
        this.modal = app.modal;
        this.modalBody = app.modalBody;

        // Limpiamos listeners anteriores y añadimos el nuestro
        const newContentArea = this.contentArea.cloneNode(true);
        this.contentArea.parentNode.replaceChild(newContentArea, this.contentArea);
        this.contentArea = newContentArea;
        this.contentArea.addEventListener('click', (e) => this.handleActionClick(e));

        this.loadView();
    },

    // Manejador de clics específico para usuarios
    handleActionClick: function (e) {
        const target = e.target;
        if (target.classList.contains('btn-edit-user')) this.handleEdit(target.dataset.id);
        if (target.classList.contains('btn-delete-user')) this.handleDelete(target.dataset.id);
        if (target.classList.contains('btn-pagination')) this.loadView(target.dataset.page);
    },

    loadView: async function (page = 1) {
        this.contentArea.innerHTML = '<h2>Cargando usuarios...</h2>';
        const result = await app.fetchWithAuth(`/api/usuario?pagina=${page}&registrosPorPagina=10`);
        if (result.success) this.renderTable(result.data);
    },

    renderTable: function (pagedData) {
        const rows = pagedData.items.map(user => `
            <tr>
                <td>${user.nombres} ${user.apellidos}</td>
                <td>${user.email}</td>
                <td>${user.rolNombre}</td>
                <td><span class="status ${user.activo ? 'status-active' : 'status-inactive'}">${user.activo ? 'Activo' : 'Inactivo'}</span></td>
                <td>
                    <button class="btn-action btn-edit-user" data-id="${user.id}">Editar</button>
                    <button class="btn-action btn-delete-user" data-id="${user.id}">Eliminar</button>
                </td>
            </tr>`).join('');
        this.contentArea.innerHTML = `
            <div class="content-header"><h2>Gestión de Usuarios</h2><a href="/Web/register.html" class="btn-primary">Crear Usuario</a></div>
            <table class="data-table"><thead><tr><th>Nombre</th><th>Email</th><th>Rol</th><th>Estado</th><th>Acciones</th></tr></thead><tbody>${rows}</tbody></table>
            <div class="pagination">${app.renderPagination(pagedData)}</div>`;
    },

    handleEdit: async function (userId) {
        const result = await app.fetchWithAuth(`/api/usuario/${userId}`);
        if (!result.success) { alert('Error al obtener datos del usuario.'); return; }
        const user = result.data;
        this.modalBody.innerHTML = `<h3>Editar Usuario</h3><form id="editUserForm"><input type="hidden" name="id" value="${user.id}"><div class="input-group"><label>Nombres</label><input type="text" name="nombres" value="${user.nombres}" required></div><div class="input-group"><label>Apellidos</label><input type="text" name="apellidos" value="${user.apellidos}" required></div><div class="input-group"><label>Email</label><input type="email" name="email" value="${user.email}" required></div><div class="input-group"><label>Cédula</label><input type="text" name="cedula" value="${user.cedula}" required></div><div class="input-group"><label>Rol</label><select name="rolId"><option value="1" ${user.rolId === 1 ? 'selected' : ''}>Administrador</option><option value="2" ${user.rolId === 2 ? 'selected' : ''}>Usuario</option></select></div><div class="input-group"><label>Activo</label><input type="checkbox" name="activo" ${user.activo ? 'checked' : ''}></div><div class="btn-group"><button type="submit" class="btn btn-primary">Guardar</button></div></form>`;
        this.modal.style.display = 'block';

        document.getElementById('editUserForm').onsubmit = async (e) => {
            e.preventDefault();
            const formData = new FormData(e.target);
            const data = Object.fromEntries(formData.entries());
            data.id = parseInt(data.id);
            data.rolId = parseInt(data.rolId);
            data.activo = formData.has('activo');
            const updateResult = await app.fetchWithAuth(`/api/usuario`, { method: 'PUT', body: JSON.stringify(data) });
            if (updateResult.success) {
                this.modal.style.display = 'none';
                await this.loadView();
            } else { alert(`Error: ${updateResult.message}`); }
        };
    },

    handleDelete: function (userId) {
        this.modalBody.innerHTML = `<h3>Confirmar</h3><p>¿Seguro que quieres desactivar este usuario?</p><div class="btn-group"><button id="confirmDeleteBtn" class="btn btn-delete">Sí</button></div>`;
        this.modal.style.display = 'block';
        document.getElementById('confirmDeleteBtn').onclick = async () => {
            const result = await app.fetchWithAuth(`/api/usuario/${userId}`, { method: 'DELETE' });
            if (result.success) {
                this.modal.style.display = 'none';
                await this.loadView();
            } else { alert(`Error: ${result.message}`); }
        };
    }
};