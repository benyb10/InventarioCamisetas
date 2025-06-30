const articleModule = {
    articles: [],
    categories: [],
    estados: [],
    currentPage: 1,
    pageSize: 12,
    totalPages: 0,
    totalRecords: 0,

    init: function () {
        this.loadArticles();
        this.loadCategories();
        this.loadEstados();
        this.renderArticleInterface();
    },

    renderArticleInterface: function () {
        app.contentArea.innerHTML = `
            <div class="module-content">
                <div class="module-header">
                    <h2>Gestión de Artículos</h2>
                    <button class="btn btn-primary" onclick="articleModule.showCreateForm()">
                        ➕ Nuevo Artículo
                    </button>
                </div>
                
                <div class="filters-section">
                    <div class="search-container">
                        <input type="text" id="articleSearch" placeholder="Buscar por código o nombre..." class="search-input">
                        <button onclick="articleModule.searchArticles()" class="btn btn-secondary">🔍</button>
                    </div>
                    <div class="filter-container">
                        <select id="categoryFilter" onchange="articleModule.filterByCategory()" class="filter-select">
                            <option value="">Todas las categorías</option>
                        </select>
                        <select id="estadoFilter" onchange="articleModule.filterByEstado()" class="filter-select">
                            <option value="">Todos los estados</option>
                        </select>
                        <select id="equipoFilter" onchange="articleModule.filterByEquipo()" class="filter-select">
                            <option value="">Todos los equipos</option>
                        </select>
                    </div>
                </div>

                <div class="view-controls">
                    <div class="view-toggle">
                        <button id="gridViewBtn" class="btn btn-ghost active" onclick="articleModule.setView('grid')">
                            📱 Tarjetas
                        </button>
                        <button id="listViewBtn" class="btn btn-ghost" onclick="articleModule.setView('list')">
                            📋 Lista
                        </button>
                    </div>
                    <div class="stats-info">
                        <span>Total de artículos: <strong id="totalArticlesCount">0</strong></span>
                    </div>
                </div>

                <div id="articlesContainer" class="articles-grid">
                    <div class="loading-spinner">Cargando artículos...</div>
                </div>
                
                <div id="paginationContainer" class="pagination-container">
                    <!-- Paginación se carga dinámicamente -->
                </div>
            </div>
        `;
    },

    async loadArticles(page = 1, search = '', categoryFilter = '', estadoFilter = '', equipoFilter = '') {
        try {
            const params = new URLSearchParams({
                pagina: page,
                registrosPorPagina: this.pageSize,
                ...(search && { buscar: search }),
                ...(categoryFilter && { categoriaId: categoryFilter }),
                ...(estadoFilter && { estadoId: estadoFilter }),
                ...(equipoFilter && { equipo: equipoFilter })
            });

            const response = await app.fetchWithAuth(`/api/Articulo?${params}`);

            if (response.success) {
                this.articles = response.data.items || [];
                this.currentPage = response.data.currentPage || 1;
                this.totalPages = response.data.totalPages || 1;
                this.totalRecords = response.data.totalRecords || 0;

                this.renderArticles();
                this.renderPagination();
                this.updateArticleCount();
            } else {
                app.showMessage(response.message || 'Error al cargar artículos', 'error');
                this.renderEmptyState();
            }
        } catch (error) {
            console.error('Error cargando artículos:', error);
            app.showMessage('Error de conexión al cargar artículos', 'error');
            this.renderEmptyState();
        }
    },

    async loadCategories() {
        try {
            const response = await app.fetchWithAuth('/api/Catalogo/categorias');
            if (response.success) {
                this.categories = response.data || [];
                this.populateCategoryFilters();
            }
        } catch (error) {
            console.error('Error cargando categorías:', error);
        }
    },

    async loadEstados() {
        try {
            const response = await app.fetchWithAuth('/api/Catalogo/estados-articulo');
            if (response.success) {
                this.estados = response.data || [];
                this.populateEstadoFilters();
            }
        } catch (error) {
            console.error('Error cargando estados:', error);
        }
    },

    populateCategoryFilters() {
        const categoryFilter = document.getElementById('categoryFilter');
        if (categoryFilter && this.categories.length > 0) {
            categoryFilter.innerHTML = '<option value="">Todas las categorías</option>';
            this.categories.forEach(category => {
                categoryFilter.innerHTML += `<option value="${category.id}">${category.nombre}</option>`;
            });
        }
    },

    populateEstadoFilters() {
        const estadoFilter = document.getElementById('estadoFilter');
        if (estadoFilter && this.estados.length > 0) {
            estadoFilter.innerHTML = '<option value="">Todos los estados</option>';
            this.estados.forEach(estado => {
                estadoFilter.innerHTML += `<option value="${estado.id}">${estado.nombre}</option>`;
            });
        }
    },

    renderArticles() {
        const container = document.getElementById('articlesContainer');

        if (!this.articles || this.articles.length === 0) {
            this.renderEmptyState();
            return;
        }

        const isGridView = container.classList.contains('articles-grid');

        if (isGridView) {
            this.renderGridView(container);
        } else {
            this.renderListView(container);
        }
    },

    renderGridView(container) {
        const articlesHTML = this.articles.map(article => `
            <div class="article-card">
                <div class="article-header">
                    <span class="article-code">${article.codigo}</span>
                    <span class="article-status ${this.getStatusClass(article.estadoArticuloId)}">
                        ${article.estadoArticuloNombre}
                    </span>
                </div>
                
                <div class="article-body">
                    <div class="article-image">
                        <div class="article-placeholder">
                            ${this.getArticleIcon(article.equipo)}
                        </div>
                    </div>
                    
                    <div class="article-info">
                        <h3 class="article-name">${article.nombre}</h3>
                        <p class="article-details">
                            <span class="article-team ${article.equipo.toLowerCase()}">${article.equipo}</span>
                            <span class="article-category">${article.categoriaNombre}</span>
                        </p>
                        
                        <div class="article-stats">
                            <div class="stat-item">
                                <span class="stat-label">Stock:</span>
                                <span class="stat-value ${article.stock <= 5 ? 'low-stock' : ''}">${article.stock}</span>
                            </div>
                            ${article.precio ? `
                                <div class="stat-item">
                                    <span class="stat-label">Precio:</span>
                                    <span class="stat-value">$${article.precio}</span>
                                </div>
                            ` : ''}
                        </div>
                        
                        ${article.ubicacion ? `
                            <p class="article-location">📍 ${article.ubicacion}</p>
                        ` : ''}
                    </div>
                </div>
                
                <div class="article-actions">
                    <button onclick="articleModule.showEditForm(${article.id})" 
                            class="btn btn-sm btn-primary" title="Editar">
                        ✏️
                    </button>
                    <button onclick="articleModule.showArticleDetails(${article.id})" 
                            class="btn btn-sm btn-secondary" title="Ver Detalles">
                        👁️
                    </button>
                    ${article.stock > 0 ? `
                        <button onclick="articleModule.createLoan(${article.id})" 
                                class="btn btn-sm btn-success" title="Crear Préstamo">
                            📋
                        </button>
                    ` : ''}
                    <button onclick="articleModule.confirmDeleteArticle(${article.id})" 
                            class="btn btn-sm btn-danger" title="Eliminar">
                        🗑️
                    </button>
                </div>
            </div>
        `).join('');

        container.innerHTML = articlesHTML;
    },

    renderListView(container) {
        const tableHTML = `
            <table class="data-table">
                <thead>
                    <tr>
                        <th>Código</th>
                        <th>Nombre</th>
                        <th>Equipo</th>
                        <th>Categoría</th>
                        <th>Estado</th>
                        <th>Stock</th>
                        <th>Precio</th>
                        <th>Ubicación</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    ${this.articles.map(article => `
                        <tr>
                            <td><strong>${article.codigo}</strong></td>
                            <td>${article.nombre}</td>
                            <td>
                                <span class="team-badge ${article.equipo.toLowerCase()}">
                                    ${article.equipo}
                                </span>
                            </td>
                            <td>${article.categoriaNombre}</td>
                            <td>
                                <span class="status-badge ${this.getStatusClass(article.estadoArticuloId)}">
                                    ${article.estadoArticuloNombre}
                                </span>
                            </td>
                            <td>
                                <span class="stock-value ${article.stock <= 5 ? 'low-stock' : ''}">
                                    ${article.stock}
                                </span>
                            </td>
                            <td>${article.precio ? `$${article.precio}` : '-'}</td>
                            <td>${article.ubicacion || '-'}</td>
                            <td>
                                <div class="action-buttons">
                                    <button onclick="articleModule.showEditForm(${article.id})" 
                                            class="btn btn-sm btn-primary" title="Editar">
                                        ✏️
                                    </button>
                                    <button onclick="articleModule.showArticleDetails(${article.id})" 
                                            class="btn btn-sm btn-secondary" title="Ver Detalles">
                                        👁️
                                    </button>
                                    ${article.stock > 0 ? `
                                        <button onclick="articleModule.createLoan(${article.id})" 
                                                class="btn btn-sm btn-success" title="Crear Préstamo">
                                            📋
                                        </button>
                                    ` : ''}
                                    <button onclick="articleModule.confirmDeleteArticle(${article.id})" 
                                            class="btn btn-sm btn-danger" title="Eliminar">
                                        🗑️
                                    </button>
                                </div>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;

        container.innerHTML = tableHTML;
    },

    renderEmptyState() {
        const container = document.getElementById('articlesContainer');
        container.innerHTML = `
            <div class="empty-state">
                <div class="empty-icon">📦</div>
                <h3>No hay artículos para mostrar</h3>
                <p>No se encontraron artículos con los filtros actuales.</p>
                <button onclick="articleModule.showCreateForm()" class="btn btn-primary">
                    ➕ Crear Primer Artículo
                </button>
            </div>
        `;
    },

    setView(viewType) {
        const container = document.getElementById('articlesContainer');
        const gridBtn = document.getElementById('gridViewBtn');
        const listBtn = document.getElementById('listViewBtn');

        if (viewType === 'grid') {
            container.className = 'articles-grid';
            gridBtn.classList.add('active');
            listBtn.classList.remove('active');
        } else {
            container.className = 'articles-list';
            gridBtn.classList.remove('active');
            listBtn.classList.add('active');
        }

        this.renderArticles();
    },

    getArticleIcon(equipo) {
        switch (equipo) {
            case 'Masculino': return '👔';
            case 'Femenino': return '👚';
            case 'Mixto': return '👕';
            default: return '📦';
        }
    },

    getStatusClass(estadoId) {
        switch (estadoId) {
            case 1: return 'disponible';
            case 2: return 'prestado';
            case 3: return 'mantenimiento';
            case 4: return 'dañado';
            default: return '';
        }
    },

    showCreateForm() {
        const formHTML = `
            <form id="createArticleForm" onsubmit="articleModule.createArticle(event)">
                <div class="form-row">
                    <div class="form-group">
                        <label for="createCodigo">Código <span class="required">*</span></label>
                        <input type="text" id="createCodigo" required>
                        <small class="form-hint">Código único del artículo (ej: PSG-001-L)</small>
                    </div>
                    <div class="form-group">
                        <label for="createEquipo">Equipo <span class="required">*</span></label>
                        <input type="text" id="createEquipo" placeholder="ej: Paris Saint-Germain" required>
                        <small class="form-hint">Nombre del equipo de fútbol</small>
                    </div>
                </div>
                
                <div class="form-group">
                    <label for="createNombre">Nombre <span class="required">*</span></label>
                    <input type="text" id="createNombre" placeholder="ej: Camiseta PSG Local 2024" required>
                    <small class="form-hint">Nombre descriptivo del artículo</small>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="createTemporada">Temporada</label>
                        <input type="text" id="createTemporada" placeholder="ej: 2023-2024">
                        <small class="form-hint">Temporada del equipo</small>
                    </div>
                    <div class="form-group">
                        <label for="createTalla">Talla</label>
                        <select id="createTalla">
                            <option value="">Seleccione una talla</option>
                            <option value="XS">XS</option>
                            <option value="S">S</option>
                            <option value="M">M</option>
                            <option value="L">L</option>
                            <option value="XL">XL</option>
                            <option value="XXL">XXL</option>
                        </select>
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="createColor">Color</label>
                        <input type="text" id="createColor" placeholder="ej: Azul Marino">
                        <small class="form-hint">Color principal de la camiseta</small>
                    </div>
                    <div class="form-group">
                        <label for="createCategoria">Categoría <span class="required">*</span></label>
                        <select id="createCategoria" required>
                            <option value="">Seleccione una categoría</option>
                            ${this.categories.map(cat => `
                                <option value="${cat.id}">${cat.nombre}</option>
                            `).join('')}
                        </select>
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="createEstado">Estado <span class="required">*</span></label>
                        <select id="createEstado" required>
                            <option value="">Seleccione un estado</option>
                            ${this.estados.map(estado => `
                                <option value="${estado.id}">${estado.nombre}</option>
                            `).join('')}
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="createStock">Stock <span class="required">*</span></label>
                        <input type="number" id="createStock" min="0" value="1" required>
                        <small class="form-hint">Cantidad disponible</small>
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="createPrecio">Precio</label>
                        <input type="number" id="createPrecio" step="0.01" min="0" placeholder="89.99">
                        <small class="form-hint">Precio unitario (opcional)</small>
                    </div>
                    <div class="form-group">
                        <label for="createUbicacion">Ubicación</label>
                        <input type="text" id="createUbicacion" placeholder="ej: Estante A-1">
                        <small class="form-hint">Ubicación física del artículo</small>
                    </div>
                </div>
                
                <div class="form-group">
                    <label for="createDescripcion">Descripción</label>
                    <textarea id="createDescripcion" rows="3" placeholder="Descripción detallada de la camiseta..."></textarea>
                    <small class="form-hint">Descripción detallada del artículo</small>
                </div>
                
                <div class="form-actions">
                    <button type="button" class="btn btn-secondary" onclick="app.closeModal()">
                        Cancelar
                    </button>
                    <button type="submit" class="btn btn-primary">
                        <span class="btn-text">Crear Artículo</span>
                        <div class="spinner" style="display: none;"></div>
                    </button>
                </div>
            </form>
        `;

        app.showModal('Crear Nuevo Artículo', formHTML);
    },

    async showEditForm(articleId) {
        try {
            const response = await app.fetchWithAuth(`/api/Articulo/${articleId}`);

            if (!response.success) {
                app.showMessage(response.message || 'Error al cargar datos del artículo', 'error');
                return;
            }

            const article = response.data;

            const formHTML = `
                <form id="editArticleForm" onsubmit="articleModule.updateArticle(event, ${articleId})">
                    <div class="form-row">
                        <div class="form-group">
                            <label for="editCodigo">Código <span class="required">*</span></label>
                            <input type="text" id="editCodigo" value="${article.codigo}" required>
                        </div>
                        <div class="form-group">
                            <label for="editEquipo">Equipo <span class="required">*</span></label>
                            <input type="text" id="editEquipo" value="${article.equipo}" required>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label for="editNombre">Nombre <span class="required">*</span></label>
                        <input type="text" id="editNombre" value="${article.nombre}" required>
                    </div>
                    
                    <div class="form-row">
                        <div class="form-group">
                            <label for="editTemporada">Temporada</label>
                            <input type="text" id="editTemporada" value="${article.temporada || ''}">
                        </div>
                        <div class="form-group">
                            <label for="editTalla">Talla</label>
                            <select id="editTalla">
                                <option value="">Seleccione una talla</option>
                                <option value="XS" ${article.talla === 'XS' ? 'selected' : ''}>XS</option>
                                <option value="S" ${article.talla === 'S' ? 'selected' : ''}>S</option>
                                <option value="M" ${article.talla === 'M' ? 'selected' : ''}>M</option>
                                <option value="L" ${article.talla === 'L' ? 'selected' : ''}>L</option>
                                <option value="XL" ${article.talla === 'XL' ? 'selected' : ''}>XL</option>
                                <option value="XXL" ${article.talla === 'XXL' ? 'selected' : ''}>XXL</option>
                            </select>
                        </div>
                    </div>
                    
                    <div class="form-row">
                        <div class="form-group">
                            <label for="editColor">Color</label>
                            <input type="text" id="editColor" value="${article.color || ''}">
                        </div>
                        <div class="form-group">
                            <label for="editCategoria">Categoría <span class="required">*</span></label>
                            <select id="editCategoria" required>
                                <option value="">Seleccione una categoría</option>
                                ${this.categories.map(cat => `
                                    <option value="${cat.id}" ${cat.id === article.categoriaId ? 'selected' : ''}>
                                        ${cat.nombre}
                                    </option>
                                `).join('')}
                            </select>
                        </div>
                    </div>
                        <div class="form-group">
                            <label for="editEstado">Estado <span class="required">*</span></label>
                            <select id="editEstado" required>
                                <option value="">Seleccione un estado</option>
                                ${this.estados.map(estado => `
                                    <option value="${estado.id}" ${estado.id === article.estadoArticuloId ? 'selected' : ''}>
                                        ${estado.nombre}
                                    </option>
                                `).join('')}
                            </select>
                        </div>
                    </div>
                    
                    <div class="form-row">
                        <div class="form-group">
                            <label for="editStock">Stock <span class="required">*</span></label>
                            <input type="number" id="editStock" value="${article.stock}" min="0" required>
                        </div>
                        <div class="form-group">
                            <label for="editPrecio">Precio</label>
                            <input type="number" id="editPrecio" value="${article.precio || ''}" step="0.01" min="0">
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label for="editUbicacion">Ubicación</label>
                        <input type="text" id="editUbicacion" value="${article.ubicacion || ''}">
                    </div>
                    
                    <div class="form-group">
                        <label for="editDescripcion">Descripción</label>
                        <textarea id="editDescripcion" rows="3">${article.descripcion || ''}</textarea>
                    </div>
                    
                    <div class="form-info">
                        <p><strong>Fecha de creación:</strong> ${this.formatDate(article.fechaCreacion)}</p>
                        <p><strong>Última actualización:</strong> ${this.formatDate(article.fechaActualizacion)}</p>
                    </div>
                    
                    <div class="form-actions">
                        <button type="button" class="btn btn-secondary" onclick="app.closeModal()">
                            Cancelar
                        </button>
                        <button type="submit" class="btn btn-primary">
                            <span class="btn-text">Actualizar Artículo</span>
                            <div class="spinner" style="display: none;"></div>
                        </button>
                    </div>
                </form>
            `;

            app.showModal('Editar Artículo', formHTML);
        } catch (error) {
            console.error('Error cargando artículo:', error);
            app.showMessage('Error al cargar datos del artículo', 'error');
        }
    },

    async showArticleDetails(articleId) {
        try {
            const response = await app.fetchWithAuth(`/api/Articulo/${articleId}`);

            if (!response.success) {
                app.showMessage(response.message || 'Error al cargar detalles del artículo', 'error');
                return;
            }

            const article = response.data;

            const detailsHTML = `
                <div class="article-details-view">
                    <div class="article-header-detail">
                        <div class="article-icon-large">
                            ${this.getArticleIcon(article.equipo)}
                        </div>
                        <div class="article-main-info">
                            <h2>${article.nombre}</h2>
                            <p class="article-code-large">Código: <strong>${article.codigo}</strong></p>
                            <div class="article-badges">
                                <span class="team-badge ${article.equipo.toLowerCase()}">${article.equipo}</span>
                                <span class="status-badge ${this.getStatusClass(article.estadoArticuloId)}">
                                    ${article.estadoArticuloNombre}
                                </span>
                            </div>
                        </div>
                    </div>
                    
                    <div class="article-details-grid">
                        <div class="detail-item">
                            <label>Categoría:</label>
                            <span>${article.categoriaNombre}</span>
                        </div>
                        <div class="detail-item">
                            <label>Stock Disponible:</label>
                            <span class="${article.stock <= 5 ? 'low-stock' : ''}">${article.stock} unidades</span>
                        </div>
                        ${article.precio ? `
                            <div class="detail-item">
                                <label>Precio:</label>
                                <span>${article.precio}</span>
                            </div>
                        ` : ''}
                        ${article.ubicacion ? `
                            <div class="detail-item">
                                <label>Ubicación:</label>
                                <span>${article.ubicacion}</span>
                            </div>
                        ` : ''}
                    </div>
                    
                    ${article.descripcion ? `
                        <div class="article-description">
                            <h4>Descripción:</h4>
                            <p>${article.descripcion}</p>
                        </div>
                    ` : ''}
                    
                    <div class="article-metadata">
                        <div class="metadata-item">
                            <label>Fecha de creación:</label>
                            <span>${this.formatDate(article.fechaCreacion)}</span>
                        </div>
                        <div class="metadata-item">
                            <label>Última actualización:</label>
                            <span>${this.formatDate(article.fechaActualizacion)}</span>
                        </div>
                    </div>
                    
                    <div class="article-actions-detail">
                        <button onclick="articleModule.showEditForm(${article.id}); app.closeModal();" 
                                class="btn btn-primary">
                            ✏️ Editar Artículo
                        </button>
                        ${article.stock > 0 ? `
                            <button onclick="articleModule.createLoan(${article.id}); app.closeModal();" 
                                    class="btn btn-success">
                                📋 Crear Préstamo
                            </button>
                        ` : ''}
                        <button onclick="app.closeModal();" class="btn btn-secondary">
                            Cerrar
                        </button>
                    </div>
                </div>
            `;

            app.showModal('Detalles del Artículo', detailsHTML);
        } catch (error) {
            console.error('Error cargando detalles:', error);
            app.showMessage('Error al cargar detalles del artículo', 'error');
        }
    },

    async createArticle(event) {
        event.preventDefault();

        const submitBtn = event.target.querySelector('button[type="submit"]');
        const btnText = submitBtn.querySelector('.btn-text');
        const spinner = submitBtn.querySelector('.spinner');

        const formData = {
            codigo: document.getElementById('createCodigo').value.trim(),
            nombre: document.getElementById('createNombre').value.trim(),
            equipo: document.getElementById('createEquipo').value.trim(),
            temporada: document.getElementById('createTemporada').value.trim() || null,
            talla: document.getElementById('createTalla').value || null,
            color: document.getElementById('createColor').value.trim() || null,
            categoriaId: parseInt(document.getElementById('createCategoria').value),
            estadoArticuloId: parseInt(document.getElementById('createEstado').value),
            stock: parseInt(document.getElementById('createStock').value),
            precio: document.getElementById('createPrecio').value ? parseFloat(document.getElementById('createPrecio').value) : null,
            ubicacion: document.getElementById('createUbicacion').value.trim() || null,
            descripcion: document.getElementById('createDescripcion').value.trim() || null
        };

        if (!this.validateArticleForm(formData, 'create')) {
            return;
        }

        this.setLoadingState(true, submitBtn, btnText, spinner);

        try {
            const response = await app.fetchWithAuth('/api/Articulo', {
                method: 'POST',
                body: JSON.stringify(formData)
            });

            if (response.success) {
                app.showMessage('Artículo creado exitosamente', 'success');
                app.closeModal();
                this.refreshArticles();
            } else {
                app.showMessage(response.message || 'Error al crear artículo', 'error');
            }
        } catch (error) {
            console.error('Error creando artículo:', error);
            app.showMessage('Error de conexión al crear artículo', 'error');
        } finally {
            this.setLoadingState(false, submitBtn, btnText, spinner);
        }
    },

    async updateArticle(event, articleId) {
        event.preventDefault();

        const submitBtn = event.target.querySelector('button[type="submit"]');
        const btnText = submitBtn.querySelector('.btn-text');
        const spinner = submitBtn.querySelector('.spinner');

        const formData = {
            id: articleId,
            codigo: document.getElementById('editCodigo').value.trim(),
            nombre: document.getElementById('editNombre').value.trim(),
            equipo: document.getElementById('editEquipo').value.trim(),
            temporada: document.getElementById('editTemporada').value.trim() || null,
            talla: document.getElementById('editTalla').value || null,
            color: document.getElementById('editColor').value.trim() || null,
            categoriaId: parseInt(document.getElementById('editCategoria').value),
            estadoArticuloId: parseInt(document.getElementById('editEstado').value),
            stock: parseInt(document.getElementById('editStock').value),
            precio: document.getElementById('editPrecio').value ? parseFloat(document.getElementById('editPrecio').value) : null,
            ubicacion: document.getElementById('editUbicacion').value.trim() || null,
            descripcion: document.getElementById('editDescripcion').value.trim() || null
        };

        if (!this.validateArticleForm(formData, 'edit')) {
            return;
        }

        this.setLoadingState(true, submitBtn, btnText, spinner);

        try {
            const response = await app.fetchWithAuth(`/api/Articulo/${articleId}`, {
                method: 'PUT',
                body: JSON.stringify(formData)
            });

            if (response.success) {
                app.showMessage('Artículo actualizado exitosamente', 'success');
                app.closeModal();
                this.refreshArticles();
            } else {
                app.showMessage(response.message || 'Error al actualizar artículo', 'error');
            }
        } catch (error) {
            console.error('Error actualizando artículo:', error);
            app.showMessage('Error de conexión al actualizar artículo', 'error');
        } finally {
            this.setLoadingState(false, submitBtn, btnText, spinner);
        }
    },

    confirmDeleteArticle(articleId) {
        const article = this.articles.find(a => a.id === articleId);

        app.showConfirm(
            `¿Está seguro que desea eliminar el artículo "${article.nombre}" (${article.codigo})? Esta acción no se puede deshacer.`,
            () => this.deleteArticle(articleId)
        );
    },

    async deleteArticle(articleId) {
        try {
            const response = await app.fetchWithAuth(`/api/Articulo/${articleId}`, {
                method: 'DELETE'
            });

            if (response.success) {
                app.showMessage('Artículo eliminado exitosamente', 'success');
                this.refreshArticles();
            } else {
                app.showMessage(response.message || 'Error al eliminar artículo', 'error');
            }
        } catch (error) {
            console.error('Error eliminando artículo:', error);
            app.showMessage('Error de conexión al eliminar artículo', 'error');
        }
    },

    createLoan(articleId) {
        console.log(`Crear préstamo para artículo ${articleId}`);
        app.showMessage('Función de préstamos en desarrollo', 'info');
    },

    validateArticleForm(formData, formType) {
        if (!formData.codigo || !formData.nombre || !formData.equipo || !formData.categoriaId || !formData.estadoArticuloId || formData.stock === undefined) {
            app.showMessage('Por favor, complete todos los campos obligatorios', 'error');
            return false;
        }

        if (formData.codigo.length < 2) {
            app.showMessage('El código debe tener al menos 2 caracteres', 'error');
            return false;
        }

        if (formData.stock < 0) {
            app.showMessage('El stock no puede ser negativo', 'error');
            return false;
        }

        if (formData.precio !== null && formData.precio < 0) {
            app.showMessage('El precio no puede ser negativo', 'error');
            return false;
        }

        return true;
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
                <button onclick="articleModule.goToPage(1)" 
                        ${this.currentPage === 1 ? 'disabled' : ''} 
                        class="btn btn-sm btn-secondary">
                    ⏮️ Primera
                </button>
                <button onclick="articleModule.goToPage(${this.currentPage - 1})" 
                        ${this.currentPage === 1 ? 'disabled' : ''} 
                        class="btn btn-sm btn-secondary">
                    ⏪ Anterior
                </button>
        `;

        for (let i = startPage; i <= endPage; i++) {
            paginationHTML += `
                <button onclick="articleModule.goToPage(${i})" 
                        class="btn btn-sm ${i === this.currentPage ? 'btn-primary' : 'btn-ghost'}">
                    ${i}
                </button>
            `;
        }

        paginationHTML += `
            <button onclick="articleModule.goToPage(${this.currentPage + 1})" 
                    ${this.currentPage === this.totalPages ? 'disabled' : ''} 
                    class="btn btn-sm btn-secondary">
                Siguiente ⏩
            </button>
            <button onclick="articleModule.goToPage(${this.totalPages})" 
                    ${this.currentPage === this.totalPages ? 'disabled' : ''} 
                    class="btn btn-sm btn-secondary">
                Última ⏭️
            </button>
        </div>
        `;

        container.innerHTML = paginationHTML;
    },

    goToPage(page) {
        if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
            this.currentPage = page;
            this.loadArticles(page, this.getCurrentSearch(), this.getCurrentCategoryFilter(), this.getCurrentEstadoFilter(), this.getCurrentEquipoFilter());
        }
    },

    getCurrentSearch() {
        const searchInput = document.getElementById('articleSearch');
        return searchInput ? searchInput.value.trim() : '';
    },

    getCurrentCategoryFilter() {
        const categoryFilter = document.getElementById('categoryFilter');
        return categoryFilter ? categoryFilter.value : '';
    },

    getCurrentEstadoFilter() {
        const estadoFilter = document.getElementById('estadoFilter');
        return estadoFilter ? estadoFilter.value : '';
    },

    getCurrentEquipoFilter() {
        const equipoFilter = document.getElementById('equipoFilter');
        return equipoFilter ? equipoFilter.value : '';
    },

    searchArticles() {
        this.currentPage = 1;
        this.loadArticles(1, this.getCurrentSearch(), this.getCurrentCategoryFilter(), this.getCurrentEstadoFilter(), this.getCurrentEquipoFilter());
    },

    filterByCategory() {
        this.currentPage = 1;
        this.loadArticles(1, this.getCurrentSearch(), this.getCurrentCategoryFilter(), this.getCurrentEstadoFilter(), this.getCurrentEquipoFilter());
    },

    filterByEstado() {
        this.currentPage = 1;
        this.loadArticles(1, this.getCurrentSearch(), this.getCurrentCategoryFilter(), this.getCurrentEstadoFilter(), this.getCurrentEquipoFilter());
    },

    filterByEquipo() {
        this.currentPage = 1;
        this.loadArticles(1, this.getCurrentSearch(), this.getCurrentCategoryFilter(), this.getCurrentEstadoFilter(), this.getCurrentEquipoFilter());
    },

    refreshArticles() {
        this.loadArticles(this.currentPage, this.getCurrentSearch(), this.getCurrentCategoryFilter(), this.getCurrentEstadoFilter(), this.getCurrentEquipoFilter());
    },

    updateArticleCount() {
        const countElement = document.getElementById('totalArticlesCount');
        if (countElement) {
            countElement.textContent = this.totalRecords;
        }
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
    }
};

window.articleModule = articleModule;