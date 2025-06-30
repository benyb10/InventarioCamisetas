const loanModule = {
    loans: [],
    users: [],
    articles: [],
    estados: [],
    currentPage: 1,
    pageSize: 10,
    totalPages: 0,
    totalRecords: 0,

    init: function () {
        this.loadLoans();
        this.loadUsers();
        this.loadArticles();
        this.loadEstados();
        this.renderLoanInterface();
    },

    renderLoanInterface: function () {
        app.contentArea.innerHTML = `
            <div class="module-content">
                <div class="module-header">
                    <h2>Gestión de Préstamos</h2>
                    <button class="btn btn-primary" onclick="loanModule.showCreateForm()">
                        ➕ Nuevo Préstamo
                    </button>
                </div>
                
                <div class="filters-section">
                    <div class="search-container">
                        <input type="text" id="loanSearch" placeholder="Buscar por usuario o artículo..." class="search-input">
                        <button onclick="loanModule.searchLoans()" class="btn btn-secondary">🔍</button>
                    </div>
                    <div class="filter-container">
                        <select id="userFilter" onchange="loanModule.filterByUser()" class="filter-select">
                            <option value="">Todos los usuarios</option>
                        </select>
                        <select id="estadoFilter" onchange="loanModule.filterByEstado()" class="filter-select">
                            <option value="">Todos los estados</option>
                        </select>
                        <input type="date" id="fechaDesde" onchange="loanModule.filterByDate()" class="filter-select" placeholder="Desde">
                        <input type="date" id="fechaHasta" onchange="loanModule.filterByDate()" class="filter-select" placeholder="Hasta">
                    </div>
                </div>

                <div class="table-container">
                    <div class="table-header">
                        <span>Total de préstamos: <strong id="totalLoansCount">0</strong></span>
                        <div class="table-actions">
                            <button onclick="loanModule.exportLoans()" class="btn btn-ghost">📊 Exportar</button>
                            <button onclick="loanModule.refreshLoans()" class="btn btn-ghost">🔄 Actualizar</button>
                        </div>
                    </div>
                    
                    <div id="loansTableContainer">
                        <div class="loading-spinner">Cargando préstamos...</div>
                    </div>
                    
                    <div id="paginationContainer" class="pagination-container">
                    </div>
                </div>
            </div>
        `;
    },

    async loadLoans(page = 1, search = '', userFilter = '', estadoFilter = '', fechaDesde = '', fechaHasta = '') {
        try {
            const params = new URLSearchParams({
                pagina: page,
                registrosPorPagina: this.pageSize
            });

            if (search) params.append('buscar', search);
            if (userFilter) params.append('usuarioId', userFilter);
            if (estadoFilter) params.append('estadoPrestamoId', estadoFilter);
            if (fechaDesde) params.append('fechaDesde', fechaDesde);
            if (fechaHasta) params.append('fechaHasta', fechaHasta);

            const response = await app.fetchWithAuth(`/api/Prestamo?${params}`);

            if (response.success) {
                this.loans = response.data.items || [];
                this.currentPage = response.data.currentPage || 1;
                this.totalPages = response.data.totalPages || 1;
                this.totalRecords = response.data.totalRecords || 0;

                this.renderLoansTable();
                this.renderPagination();
                this.updateLoanCount();
            } else {
                app.showMessage(response.message || 'Error al cargar préstamos', 'error');
                this.renderEmptyTable();
            }
        } catch (error) {
            console.error('Error cargando préstamos:', error);
            app.showMessage('Error de conexión al cargar préstamos', 'error');
            this.renderEmptyTable();
        }
    },

    async loadUsers() {
        try {
            const response = await app.fetchWithAuth('/api/Usuario?registrosPorPagina=1000');
            if (response.success) {
                this.users = response.data.items || [];
                this.populateUserFilters();
            }
        } catch (error) {
            console.error('Error cargando usuarios:', error);
        }
    },

    async loadArticles() {
        try {
            const response = await app.fetchWithAuth('/api/Articulo?registrosPorPagina=1000');
            if (response.success) {
                this.articles = response.data.items || [];
            }
        } catch (error) {
            console.error('Error cargando artículos:', error);
        }
    },

    async loadEstados() {
        try {
            const response = await app.fetchWithAuth('/api/Catalogo/estados-prestamo');
            if (response.success) {
                this.estados = response.data || [];
                this.populateEstadoFilters();
            }
        } catch (error) {
            console.error('Error cargando estados:', error);
        }
    },

    populateUserFilters() {
        const userFilter = document.getElementById('userFilter');
        if (userFilter && this.users.length > 0) {
            userFilter.innerHTML = '<option value="">Todos los usuarios</option>';
            this.users.forEach(user => {
                userFilter.innerHTML += `<option value="${user.id}">${user.nombres} ${user.apellidos}</option>`;
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

    renderLoansTable() {
        const container = document.getElementById('loansTableContainer');

        if (!this.loans || this.loans.length === 0) {
            this.renderEmptyTable();
            return;
        }

        const tableHTML = `
            <table class="data-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Usuario</th>
                        <th>Artículo</th>
                        <th>Fecha Solicitud</th>
                        <th>Fecha Entrega Est.</th>
                        <th>Estado</th>
                        <th>Observaciones</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    ${this.loans.map(loan => `
                        <tr>
                            <td><strong>#${loan.id}</strong></td>
                            <td>
                                <div class="user-info">
                                    <strong>${loan.usuarioNombre || 'Usuario N/A'}</strong>
                                    <small>${loan.usuarioEmail || ''}</small>
                                </div>
                            </td>
                            <td>
                                <div class="article-info">
                                    <strong>${loan.articuloCodigo || 'Código N/A'}</strong>
                                    <small>${loan.articuloNombre || 'Artículo N/A'}</small>
                                </div>
                            </td>
                            <td>${this.formatDate(loan.fechaSolicitud)}</td>
                            <td>${this.formatDate(loan.fechaEntregaEstimada)}</td>
                            <td>
                                <span class="status-badge ${this.getStatusClass(loan.estadoPrestamoId)}">
                                    ${loan.estadoPrestamoNombre || 'Estado N/A'}
                                </span>
                            </td>
                            <td>
                                <span title="${loan.observaciones || ''}">${this.truncateText(loan.observaciones, 30)}</span>
                            </td>
                            <td>
                                <div class="action-buttons">
                                    <button onclick="loanModule.showLoanDetails(${loan.id})" 
                                            class="btn btn-sm btn-secondary" title="Ver Detalles">
                                        👁️
                                    </button>
                                    ${this.renderLoanActions(loan)}
                                </div>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;

        container.innerHTML = tableHTML;
    },

    renderLoanActions(loan) {
        const currentUserId = app.userData.id;
        const isAdmin = app.userData.rolNombre === 'Administrador';

        let actions = '';

        switch (loan.estadoPrestamoId) {
            case 1: // Pendiente
                if (isAdmin) {
                    actions += `
                        <button onclick="loanModule.approveLoan(${loan.id})" 
                                class="btn btn-sm btn-success" title="Aprobar">
                            ✅
                        </button>
                        <button onclick="loanModule.rejectLoan(${loan.id})" 
                                class="btn btn-sm btn-danger" title="Rechazar">
                            ❌
                        </button>
                    `;
                }
                if (loan.usuarioId === currentUserId || isAdmin) {
                    actions += `
                        <button onclick="loanModule.showEditForm(${loan.id})" 
                                class="btn btn-sm btn-primary" title="Editar">
                            ✏️
                        </button>
                    `;
                }
                break;
            case 2: // Aprobado
                if (isAdmin) {
                    actions += `
                        <button onclick="loanModule.deliverLoan(${loan.id})" 
                                class="btn btn-sm btn-info" title="Entregar">
                            📦
                        </button>
                    `;
                }
                break;
            case 3: // Entregado
                if (isAdmin) {
                    actions += `
                        <button onclick="loanModule.returnLoan(${loan.id})" 
                                class="btn btn-sm btn-warning" title="Registrar Devolución">
                            🔄
                        </button>
                    `;
                }
                break;
        }

        return actions;
    },

    getStatusClass(estadoId) {
        switch (estadoId) {
            case 1: return 'pendiente';    // Pendiente
            case 2: return 'aprobado';     // Aprobado
            case 3: return 'entregado';    // Entregado
            case 4: return 'devuelto';     // Devuelto
            case 5: return 'rechazado';    // Rechazado
            case 6: return 'vencido';      // Vencido
            default: return '';
        }
    },

    renderEmptyTable() {
        const container = document.getElementById('loansTableContainer');
        container.innerHTML = `
            <div class="empty-state">
                <div class="empty-icon">📋</div>
                <h3>No hay préstamos para mostrar</h3>
                <p>No se encontraron préstamos con los filtros actuales.</p>
                <button onclick="loanModule.showCreateForm()" class="btn btn-primary">
                    ➕ Crear Primer Préstamo
                </button>
            </div>
        `;
    },

    showCreateForm() {
        const availableUsers = this.users.filter(u => u.activo);
        const availableArticles = this.articles.filter(a => a.stock > 0 && a.estadoArticuloId === 1);

        const formHTML = `
            <form id="createLoanForm" onsubmit="loanModule.createLoan(event)">
                <div class="form-row">
                    <div class="form-group">
                        <label for="createUsuario">Usuario <span class="required">*</span></label>
                        <select id="createUsuario" required>
                            <option value="">Seleccione un usuario</option>
                            ${availableUsers.map(user => `
                                <option value="${user.id}">${user.nombres} ${user.apellidos} (${user.email})</option>
                            `).join('')}
                        </select>
                        <small class="form-hint">Usuario que solicita el préstamo</small>
                    </div>
                    <div class="form-group">
                        <label for="createArticulo">Artículo <span class="required">*</span></label>
                        <select id="createArticulo" required>
                            <option value="">Seleccione un artículo</option>
                            ${availableArticles.map(article => `
                                <option value="${article.id}">${article.codigo} - ${article.nombre} (Stock: ${article.stock})</option>
                            `).join('')}
                        </select>
                        <small class="form-hint">Artículo a prestar (solo disponibles)</small>
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="createFechaEntrega">Fecha de Entrega Estimada <span class="required">*</span></label>
                        <input type="datetime-local" id="createFechaEntrega" required>
                        <small class="form-hint">Fecha estimada de entrega del artículo</small>
                    </div>
                    <div class="form-group">
                        <label for="createFechaDevolucion">Fecha de Devolución Estimada</label>
                        <input type="datetime-local" id="createFechaDevolucion">
                        <small class="form-hint">Fecha estimada de devolución (opcional)</small>
                    </div>
                </div>
                
                <div class="form-group">
                    <label for="createObservaciones">Observaciones</label>
                    <textarea id="createObservaciones" rows="3" placeholder="Motivo del préstamo o comentarios adicionales..."></textarea>
                    <small class="form-hint">Información adicional sobre el préstamo</small>
                </div>
                
                <div class="form-actions">
                    <button type="button" class="btn btn-secondary" onclick="app.closeModal()">
                        Cancelar
                    </button>
                    <button type="submit" class="btn btn-primary">
                        <span class="btn-text">Crear Préstamo</span>
                        <div class="spinner" style="display: none;"></div>
                    </button>
                </div>
            </form>
        `;

        app.showModal('Crear Nuevo Préstamo', formHTML);

        // Establecer fecha mínima como hoy
        const today = new Date();
        today.setMinutes(today.getMinutes() - today.getTimezoneOffset());
        const todayString = today.toISOString().slice(0, 16);
        document.getElementById('createFechaEntrega').min = todayString;
        document.getElementById('createFechaDevolucion').min = todayString;
    },

    async showEditForm(loanId) {
        try {
            const response = await app.fetchWithAuth(`/api/Prestamo/${loanId}`);

            if (!response.success) {
                app.showMessage(response.message || 'Error al cargar datos del préstamo', 'error');
                return;
            }

            const loan = response.data;

            const formHTML = `
                <form id="editLoanForm" onsubmit="loanModule.updateLoan(event, ${loanId})">
                    <div class="form-row">
                        <div class="form-group">
                            <label for="editFechaEntrega">Fecha de Entrega Estimada <span class="required">*</span></label>
                            <input type="datetime-local" id="editFechaEntrega" 
                                   value="${this.formatDateForInput(loan.fechaEntregaEstimada)}" required>
                        </div>
                        <div class="form-group">
                            <label for="editFechaDevolucion">Fecha de Devolución Estimada</label>
                            <input type="datetime-local" id="editFechaDevolucion" 
                                   value="${loan.fechaDevolucionEstimada ? this.formatDateForInput(loan.fechaDevolucionEstimada) : ''}">
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label for="editObservaciones">Observaciones</label>
                        <textarea id="editObservaciones" rows="3">${loan.observaciones || ''}</textarea>
                    </div>
                    
                    <div class="form-info">
                        <p><strong>Usuario:</strong> ${loan.usuarioNombre}</p>
                        <p><strong>Artículo:</strong> ${loan.articuloCodigo} - ${loan.articuloNombre}</p>
                        <p><strong>Estado actual:</strong> ${loan.estadoPrestamoNombre}</p>
                        <p><strong>Fecha de solicitud:</strong> ${this.formatDate(loan.fechaSolicitud)}</p>
                    </div>
                    
                    <div class="form-actions">
                        <button type="button" class="btn btn-secondary" onclick="app.closeModal()">
                            Cancelar
                        </button>
                        <button type="submit" class="btn btn-primary">
                            <span class="btn-text">Actualizar Préstamo</span>
                            <div class="spinner" style="display: none;"></div>
                        </button>
                    </div>
                </form>
            `;

            app.showModal('Editar Préstamo', formHTML);
        } catch (error) {
            console.error('Error cargando préstamo:', error);
            app.showMessage('Error al cargar datos del préstamo', 'error');
        }
    },

    async showLoanDetails(loanId) {
        try {
            const response = await app.fetchWithAuth(`/api/Prestamo/${loanId}`);

            if (!response.success) {
                app.showMessage(response.message || 'Error al cargar detalles del préstamo', 'error');
                return;
            }

            const loan = response.data;

            const detailsHTML = `
                <div class="loan-details-view">
                    <div class="loan-header-detail">
                        <div class="loan-icon-large">📋</div>
                        <div class="loan-main-info">
                            <h2>Préstamo #${loan.id}</h2>
                            <div class="loan-badges">
                                <span class="status-badge ${this.getStatusClass(loan.estadoPrestamoId)}">
                                    ${loan.estadoPrestamoNombre}
                                </span>
                            </div>
                        </div>
                    </div>
                    
                    <div class="loan-details-grid">
                        <div class="detail-section">
                            <h4>👤 Información del Usuario</h4>
                            <div class="detail-item">
                                <label>Nombre:</label>
                                <span>${loan.usuarioNombre}</span>
                            </div>
                            <div class="detail-item">
                                <label>Email:</label>
                                <span>${loan.usuarioEmail}</span>
                            </div>
                        </div>
                        
                        <div class="detail-section">
                            <h4>📦 Información del Artículo</h4>
                            <div class="detail-item">
                                <label>Código:</label>
                                <span>${loan.articuloCodigo}</span>
                            </div>
                            <div class="detail-item">
                                <label>Nombre:</label>
                                <span>${loan.articuloNombre}</span>
                            </div>
                        </div>
                        
                        <div class="detail-section">
                            <h4>📅 Fechas del Préstamo</h4>
                            <div class="detail-item">
                                <label>Fecha de Solicitud:</label>
                                <span>${this.formatDate(loan.fechaSolicitud)}</span>
                            </div>
                            <div class="detail-item">
                                <label>Fecha Entrega Estimada:</label>
                                <span>${this.formatDate(loan.fechaEntregaEstimada)}</span>
                            </div>
                            ${loan.fechaEntregaReal ? `
                                <div class="detail-item">
                                    <label>Fecha Entrega Real:</label>
                                    <span>${this.formatDate(loan.fechaEntregaReal)}</span>
                                </div>
                            ` : ''}
                            ${loan.fechaDevolucionEstimada ? `
                                <div class="detail-item">
                                    <label>Fecha Devolución Estimada:</label>
                                    <span>${this.formatDate(loan.fechaDevolucionEstimada)}</span>
                                </div>
                            ` : ''}
                            ${loan.fechaDevolucionReal ? `
                                <div class="detail-item">
                                    <label>Fecha Devolución Real:</label>
                                    <span>${this.formatDate(loan.fechaDevolucionReal)}</span>
                                </div>
                            ` : ''}
                        </div>
                        
                        ${loan.aprobadoPorNombre ? `
                            <div class="detail-section">
                                <h4>✅ Información de Aprobación</h4>
                                <div class="detail-item">
                                    <label>Aprobado por:</label>
                                    <span>${loan.aprobadoPorNombre}</span>
                                </div>
                                <div class="detail-item">
                                    <label>Fecha de Aprobación:</label>
                                    <span>${this.formatDate(loan.fechaAprobacion)}</span>
                                </div>
                                ${loan.observacionesAprobacion ? `
                                    <div class="detail-item">
                                        <label>Observaciones de Aprobación:</label>
                                        <span>${loan.observacionesAprobacion}</span>
                                    </div>
                                ` : ''}
                            </div>
                        ` : ''}
                    </div>
                    
                    ${loan.observaciones ? `
                        <div class="loan-observations">
                            <h4>📝 Observaciones:</h4>
                            <p>${loan.observaciones}</p>
                        </div>
                    ` : ''}
                    
                    <div class="loan-actions-detail">
                        ${this.renderDetailActions(loan)}
                        <button onclick="app.closeModal();" class="btn btn-secondary">
                            Cerrar
                        </button>
                    </div>
                </div>
            `;

            app.showModal('Detalles del Préstamo', detailsHTML);
        } catch (error) {
            console.error('Error cargando detalles:', error);
            app.showMessage('Error al cargar detalles del préstamo', 'error');
        }
    },

    renderDetailActions(loan) {
        const currentUserId = app.userData.id;
        const isAdmin = app.userData.rolNombre === 'Administrador';
        let actions = '';

        switch (loan.estadoPrestamoId) {
            case 1: // Pendiente
                if (isAdmin) {
                    actions += `
                        <button onclick="loanModule.approveLoan(${loan.id}); app.closeModal();" 
                                class="btn btn-success">
                            ✅ Aprobar Préstamo
                        </button>
                        <button onclick="loanModule.rejectLoan(${loan.id}); app.closeModal();" 
                                class="btn btn-danger">
                            ❌ Rechazar Préstamo
                        </button>
                    `;
                }
                if (loan.usuarioId === currentUserId || isAdmin) {
                    actions += `
                        <button onclick="loanModule.showEditForm(${loan.id}); app.closeModal();" 
                                class="btn btn-primary">
                            ✏️ Editar Préstamo
                        </button>
                    `;
                }
                break;
            case 2: // Aprobado
                if (isAdmin) {
                    actions += `
                        <button onclick="loanModule.deliverLoan(${loan.id}); app.closeModal();" 
                                class="btn btn-info">
                            📦 Registrar Entrega
                        </button>
                    `;
                }
                break;
            case 3: // Entregado
                if (isAdmin) {
                    actions += `
                        <button onclick="loanModule.returnLoan(${loan.id}); app.closeModal();" 
                                class="btn btn-warning">
                            🔄 Registrar Devolución
                        </button>
                    `;
                }
                break;
        }

        return actions;
    },

    async createLoan(event) {
        event.preventDefault();

        const submitBtn = event.target.querySelector('button[type="submit"]');
        const btnText = submitBtn.querySelector('.btn-text');
        const spinner = submitBtn.querySelector('.spinner');

        const formData = {
            usuarioId: parseInt(document.getElementById('createUsuario').value),
            articuloId: parseInt(document.getElementById('createArticulo').value),
            fechaEntregaEstimada: document.getElementById('createFechaEntrega').value,
            fechaDevolucionEstimada: document.getElementById('createFechaDevolucion').value || null,
            observaciones: document.getElementById('createObservaciones').value.trim() || null
        };

        if (!this.validateLoanForm(formData, 'create')) {
            return;
        }

        this.setLoadingState(true, submitBtn, btnText, spinner);

        try {
            const response = await app.fetchWithAuth('/api/Prestamo', {
                method: 'POST',
                body: JSON.stringify(formData)
            });

            if (response.success) {
                app.showMessage('Préstamo creado exitosamente', 'success');
                app.closeModal();
                this.refreshLoans();
            } else {
                app.showMessage(response.message || 'Error al crear préstamo', 'error');
            }
        } catch (error) {
            console.error('Error creando préstamo:', error);
            app.showMessage('Error de conexión al crear préstamo', 'error');
        } finally {
            this.setLoadingState(false, submitBtn, btnText, spinner);
        }
    },

    async updateLoan(event, loanId) {
        event.preventDefault();

        const submitBtn = event.target.querySelector('button[type="submit"]');
        const btnText = submitBtn.querySelector('.btn-text');
        const spinner = submitBtn.querySelector('.spinner');

        const formData = {
            id: loanId,
            fechaEntregaEstimada: document.getElementById('editFechaEntrega').value,
            fechaDevolucionEstimada: document.getElementById('editFechaDevolucion').value || null,
            observaciones: document.getElementById('editObservaciones').value.trim() || null
        };

        this.setLoadingState(true, submitBtn, btnText, spinner);

        try {
            const response = await app.fetchWithAuth(`/api/Prestamo/${loanId}`, {
                method: 'PUT',
                body: JSON.stringify(formData)
            });

            if (response.success) {
                app.showMessage('Préstamo actualizado exitosamente', 'success');
                app.closeModal();
                this.refreshLoans();
            } else {
                app.showMessage(response.message || 'Error al actualizar préstamo', 'error');
            }
        } catch (error) {
            console.error('Error actualizando préstamo:', error);
            app.showMessage('Error de conexión al actualizar préstamo', 'error');
        } finally {
            this.setLoadingState(false, submitBtn, btnText, spinner);
        }
    },

    async approveLoan(loanId) {
        const observaciones = prompt('Observaciones de aprobación (opcional):') || '';

        try {
            const response = await app.fetchWithAuth(`/api/Prestamo/${loanId}/approve`, {
                method: 'PUT',
                body: JSON.stringify({
                    id: loanId,
                    aprobado: true,
                    aprobadoPor: app.userData.id,
                    observacionesAprobacion: observaciones,
                    fechaEntregaReal: new Date().toISOString()
                })
            });

            if (response.success) {
                app.showMessage('Préstamo aprobado exitosamente', 'success');
                this.refreshLoans();
            } else {
                app.showMessage(response.message || 'Error al aprobar préstamo', 'error');
            }
        } catch (error) {
            console.error('Error aprobando préstamo:', error);
            app.showMessage('Error de conexión al aprobar préstamo', 'error');
        }
    },

    async rejectLoan(loanId) {
        const motivo = prompt('Motivo del rechazo:');
        if (!motivo) return;

        try {
            const response = await app.fetchWithAuth(`/api/Prestamo/${loanId}/approve`, {
                method: 'PUT',
                body: JSON.stringify({
                    id: loanId,
                    aprobado: false,
                    aprobadoPor: app.userData.id,
                    observacionesAprobacion: motivo
                })
            });

            if (response.success) {
                app.showMessage('Préstamo rechazado', 'success');
                this.refreshLoans();
            } else {
                app.showMessage(response.message || 'Error al rechazar préstamo', 'error');
            }
        } catch (error) {
            console.error('Error rechazando préstamo:', error);
            app.showMessage('Error de conexión al rechazar préstamo', 'error');
        }
    },

    async deliverLoan(loanId) {
        try {
            const response = await app.fetchWithAuth(`/api/Prestamo/${loanId}/deliver`, {
                method: 'PUT',
                body: JSON.stringify({
                    id: loanId,
                    fechaEntregaReal: new Date().toISOString()
                })
            });

            if (response.success) {
                app.showMessage('Entrega registrada exitosamente', 'success');
                this.refreshLoans();
            } else {
                app.showMessage(response.message || 'Error al registrar entrega', 'error');
            }
        } catch (error) {
            console.error('Error registrando entrega:', error);
            app.showMessage('Error de conexión al registrar entrega', 'error');
        }
    },

    async returnLoan(loanId) {
        const observaciones = prompt('Observaciones de devolución (opcional):') || '';

        try {
            const response = await app.fetchWithAuth(`/api/Prestamo/${loanId}/return`, {
                method: 'PUT',
                body: JSON.stringify({
                    id: loanId,
                    fechaDevolucionReal: new Date().toISOString(),
                    observacionesDevolucion: observaciones
                })
            });

            if (response.success) {
                app.showMessage('Devolución registrada exitosamente', 'success');
                this.refreshLoans();
            } else {
                app.showMessage(response.message || 'Error al registrar devolución', 'error');
            }
        } catch (error) {
            console.error('Error registrando devolución:', error);
            app.showMessage('Error de conexión al registrar devolución', 'error');
        }
    },

    validateLoanForm(formData, formType) {
        if (!formData.usuarioId || !formData.articuloId || !formData.fechaEntregaEstimada) {
            app.showMessage('Por favor, complete todos los campos obligatorios', 'error');
            return false;
        }

        const fechaEntrega = new Date(formData.fechaEntregaEstimada);
        const ahora = new Date();

        if (fechaEntrega < ahora) {
            app.showMessage('La fecha de entrega no puede ser anterior a la fecha actual', 'error');
            return false;
        }

        if (formData.fechaDevolucionEstimada) {
            const fechaDevolucion = new Date(formData.fechaDevolucionEstimada);
            if (fechaDevolucion < fechaEntrega) {
                app.showMessage('La fecha de devolución no puede ser anterior a la fecha de entrega', 'error');
                return false;
            }
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
               <button onclick="loanModule.goToPage(1)" 
                       ${this.currentPage === 1 ? 'disabled' : ''} 
                       class="btn btn-sm btn-secondary">
                   ⏮️ Primera
               </button>
               <button onclick="loanModule.goToPage(${this.currentPage - 1})" 
                       ${this.currentPage === 1 ? 'disabled' : ''} 
                       class="btn btn-sm btn-secondary">
                   ⏪ Anterior
               </button>
       `;

        for (let i = startPage; i <= endPage; i++) {
            paginationHTML += `
               <button onclick="loanModule.goToPage(${i})" 
                       class="btn btn-sm ${i === this.currentPage ? 'btn-primary' : 'btn-ghost'}">
                   ${i}
               </button>
           `;
        }

        paginationHTML += `
           <button onclick="loanModule.goToPage(${this.currentPage + 1})" 
                   ${this.currentPage === this.totalPages ? 'disabled' : ''} 
                   class="btn btn-sm btn-secondary">
               Siguiente ⏩
           </button>
           <button onclick="loanModule.goToPage(${this.totalPages})" 
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
            this.loadLoans(page, this.getCurrentSearch(), this.getCurrentUserFilter(), this.getCurrentEstadoFilter(), this.getCurrentFechaDesde(), this.getCurrentFechaHasta());
        }
    },

    getCurrentSearch() {
        const searchInput = document.getElementById('loanSearch');
        return searchInput ? searchInput.value.trim() : '';
    },

    getCurrentUserFilter() {
        const userFilter = document.getElementById('userFilter');
        return userFilter ? userFilter.value : '';
    },

    getCurrentEstadoFilter() {
        const estadoFilter = document.getElementById('estadoFilter');
        return estadoFilter ? estadoFilter.value : '';
    },

    getCurrentFechaDesde() {
        const fechaDesde = document.getElementById('fechaDesde');
        return fechaDesde ? fechaDesde.value : '';
    },

    getCurrentFechaHasta() {
        const fechaHasta = document.getElementById('fechaHasta');
        return fechaHasta ? fechaHasta.value : '';
    },

    searchLoans() {
        this.currentPage = 1;
        this.loadLoans(1, this.getCurrentSearch(), this.getCurrentUserFilter(), this.getCurrentEstadoFilter(), this.getCurrentFechaDesde(), this.getCurrentFechaHasta());
    },

    filterByUser() {
        this.currentPage = 1;
        this.loadLoans(1, this.getCurrentSearch(), this.getCurrentUserFilter(), this.getCurrentEstadoFilter(), this.getCurrentFechaDesde(), this.getCurrentFechaHasta());
    },

    filterByEstado() {
        this.currentPage = 1;
        this.loadLoans(1, this.getCurrentSearch(), this.getCurrentUserFilter(), this.getCurrentEstadoFilter(), this.getCurrentFechaDesde(), this.getCurrentFechaHasta());
    },

    filterByDate() {
        this.currentPage = 1;
        this.loadLoans(1, this.getCurrentSearch(), this.getCurrentUserFilter(), this.getCurrentEstadoFilter(), this.getCurrentFechaDesde(), this.getCurrentFechaHasta());
    },

    refreshLoans() {
        this.loadLoans(this.currentPage, this.getCurrentSearch(), this.getCurrentUserFilter(), this.getCurrentEstadoFilter(), this.getCurrentFechaDesde(), this.getCurrentFechaHasta());
    },

    updateLoanCount() {
        const countElement = document.getElementById('totalLoansCount');
        if (countElement) {
            countElement.textContent = this.totalRecords;
        }
    },

    exportLoans() {
        if (!this.loans || this.loans.length === 0) {
            app.showMessage('No hay préstamos para exportar', 'warning');
            return;
        }

        const csvContent = this.generateCSV();
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');

        if (link.download !== undefined) {
            const url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', `prestamos_${new Date().toISOString().split('T')[0]}.csv`);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);

            app.showMessage('Préstamos exportados exitosamente', 'success');
        }
    },

    generateCSV() {
        const headers = ['ID', 'Usuario', 'Email', 'Artículo Código', 'Artículo Nombre', 'Fecha Solicitud', 'Fecha Entrega Est.', 'Fecha Devolución Est.', 'Estado', 'Observaciones'];
        const csvRows = [headers.join(',')];

        this.loans.forEach(loan => {
            const row = [
                loan.id,
                `"${loan.usuarioNombre || ''}"`,
                loan.usuarioEmail || '',
                loan.articuloCodigo || '',
                `"${loan.articuloNombre || ''}"`,
                this.formatDate(loan.fechaSolicitud),
                this.formatDate(loan.fechaEntregaEstimada),
                loan.fechaDevolucionEstimada ? this.formatDate(loan.fechaDevolucionEstimada) : '',
                loan.estadoPrestamoNombre || '',
                `"${loan.observaciones || ''}"`
            ];
            csvRows.push(row.join(','));
        });

        return csvRows.join('\n');
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

    formatDateForInput(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
        return date.toISOString().slice(0, 16);
    },

    truncateText(text, maxLength) {
        if (!text) return '-';
        return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
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
    }
};

window.loanModule = loanModule;