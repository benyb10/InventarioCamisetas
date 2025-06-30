// Módulo de Artículos, adjunto al objeto global 'app'
app.articles = {
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

    handleActionClick: function (e) {
        const target = e.target;
        if (target.id === 'createNewArticleBtn') this.handleCreateOrEdit();
        if (target.classList.contains('btn-edit-article')) this.handleCreateOrEdit(target.dataset.id);
        if (target.classList.contains('btn-delete-article')) this.handleDelete(target.dataset.id);
        if (target.classList.contains('btn-pagination')) this.loadView(target.dataset.page);
    },

    loadView: async function (page = 1) {
        this.contentArea.innerHTML = '<h2>Cargando artículos...</h2>';
        const result = await app.fetchWithAuth(`/api/articulo?pagina=${page}&registrosPorPagina=10`);
        if (result.success) this.renderTable(result.data);
    },

    renderTable: function (pagedData) {
        const rows = pagedData.items.map(item => `
            <tr>
                <td>${item.codigo}</td>
                <td>${item.nombre}</td>
                <td>${item.categoriaNombre}</td>
                <td>${item.stock}</td>
                <td><span class="status ${item.activo ? 'status-active' : 'status-inactive'}">${item.activo ? 'Activo' : 'Inactivo'}</span></td>
                <td>
                    <button class="btn-action btn-edit-article" data-id="${item.id}">Editar</button>
                    <button class="btn-action btn-delete-article" data-id="${item.id}">Eliminar</button>
                </td>
            </tr>`).join('');
        this.contentArea.innerHTML = `
            <div class="content-header"><h2>Gestión de Artículos</h2><button id="createNewArticleBtn" class="btn-primary">Crear Artículo</button></div>
            <table class="data-table"><thead><tr><th>Código</th><th>Nombre</th><th>Categoría</th><th>Stock</th><th>Estado</th><th>Acciones</th></tr></thead><tbody>${rows}</tbody></table>
            <div class="pagination">${app.renderPagination(pagedData)}</div>`;
    },

    handleCreateOrEdit: async function (articleId = null) {
        const isEditing = articleId !== null;
        const [articleRes, catalogsRes] = await Promise.all([
            isEditing ? app.fetchWithAuth(`/api/articulo/${articleId}`) : Promise.resolve({ success: true, data: {} }),
            app.fetchWithAuth('/api/catalogo/all')
        ]);

        if (!articleRes.success || !catalogsRes.success) { alert('Error al cargar datos del formulario.'); return; }

        const article = articleRes.data;
        const catalogs = catalogsRes.data;
        const categoriesOptions = catalogs.categorias.map(c => `<option value="${c.id}" ${isEditing && c.id === article.categoriaId ? 'selected' : ''}>${c.nombre}</option>`).join('');
        const statesOptions = catalogs.estadosArticulo.map(s => `<option value="${s.id}" ${isEditing && s.id === article.estadoArticuloId ? 'selected' : ''}>${s.nombre}</option>`).join('');

        this.modalBody.innerHTML = `<h3>${isEditing ? 'Editar' : 'Crear'} Artículo</h3><form id="articleForm"><input type="hidden" name="id" value="${isEditing ? article.id : '0'}"><div class="form-grid"><div class="input-group"><label>Código</label><input type="text" name="codigo" value="${isEditing ? article.codigo : ''}" required></div><div class="input-group"><label>Nombre</label><input type="text" name="nombre" value="${isEditing ? article.nombre : ''}" required></div><div class="input-group full-width"><label>Descripción</label><input type="text" name="descripcion" value="${isEditing ? article.descripcion : ''}"></div><div class="input-group"><label>Equipo</label><input type="text" name="equipo" value="${isEditing ? article.equipo : ''}"></div><div class="input-group"><label>Temporada</label><input type="text" name="temporada" value="${isEditing ? article.temporada : ''}"></div><div class="input-group"><label>Talla</label><input type="text" name="talla" value="${isEditing ? article.talla : ''}"></div><div class="input-group"><label>Color</label><input type="text" name="color" value="${isEditing ? article.color : ''}"></div><div class="input-group"><label>Precio</label><input type="number" step="0.01" name="precio" value="${isEditing ? article.precio : '0.00'}"></div><div class="input-group"><label>Stock</label><input type="number" name="stock" value="${isEditing ? article.stock : '0'}" required></div><div class="input-group"><label>Categoría</label><select name="categoriaId">${categoriesOptions}</select></div><div class="input-group"><label>Estado</label><select name="estadoArticuloId">${statesOptions}</select></div><div class="input-group full-width"><label>Ubicación</label><input type="text" name="ubicacion" value="${isEditing ? article.ubicacion : ''}"></div>${isEditing ? `<div class="input-group"><label>Activo</label><input type="checkbox" name="activo" ${article.activo ? 'checked' : ''}></div>` : ''}</div><div class="btn-group"><button type="submit" class="btn btn-primary">Guardar</button></div></form>`;
        this.modal.style.display = 'block';

        document.getElementById('articleForm').onsubmit = async (e) => {
            e.preventDefault();
            const formData = new FormData(e.target);
            const data = Object.fromEntries(formData.entries());
            const numberFields = ['id', 'precio', 'stock', 'categoriaId', 'estadoArticuloId'];
            numberFields.forEach(field => { if (data[field]) data[field] = parseFloat(data[field]); });
            if (isEditing) data.activo = formData.has('activo');

            const result = await app.fetchWithAuth(isEditing ? '/api/articulo' : '/api/articulo', { method: isEditing ? 'PUT' : 'POST', body: JSON.stringify(data) });
            if (result.success) {
                this.modal.style.display = 'none';
                await this.loadView();
            } else { alert(`Error: ${result.message}`); }
        };
    },

    handleDelete: function (articleId) {
        this.modalBody.innerHTML = `<h3>Confirmar</h3><p>¿Seguro que quieres desactivar este artículo?</p><div class="btn-group"><button id="confirmDeleteBtn" class="btn btn-delete">Sí, Eliminar</button></div>`;
        this.modal.style.display = 'block';
        document.getElementById('confirmDeleteBtn').onclick = async () => {
            const result = await app.fetchWithAuth(`/api/articulo/${articleId}`, { method: 'DELETE' });
            if (result.success) {
                this.modal.style.display = 'none';
                await this.loadView();
            } else { alert(`Error: ${result.message}`); }
        };
    }
};