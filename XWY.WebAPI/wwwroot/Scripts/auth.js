document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    const errorDiv = document.getElementById('login-error');

    if (loginForm) {
        loginForm.addEventListener('submit', handleLogin);
    }

    if (localStorage.getItem('authToken')) {
        window.location.href = '/Web/dashboard.html';
    }
});

async function handleLogin(e) {
    e.preventDefault();

    const submitBtn = e.target.querySelector('button[type="submit"]');
    const btnText = submitBtn.querySelector('.btn-text');
    const spinner = submitBtn.querySelector('.spinner');
    const errorDiv = document.getElementById('login-error');

    const formData = {
        email: document.getElementById('email').value.trim(),
        password: document.getElementById('password').value
    };

    if (!formData.email || !formData.password) {
        showError('Por favor, complete todos los campos.');
        return;
    }

    setLoading(true, submitBtn, btnText, spinner);
    clearError();

    try {
        const response = await fetch('/api/Auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });

        const result = await response.json();

        if (result.success && result.data) {
            localStorage.setItem('authToken', result.data.token);
            localStorage.setItem('userData', JSON.stringify(result.data.usuario));
            localStorage.setItem('tokenExpiration', result.data.expiration);

            showSuccess('Iniciando sesión...');

            setTimeout(() => {
                window.location.href = '/Web/dashboard.html';
            }, 1000);
        } else {
            showError(result.message || 'Error al iniciar sesión. Verifique sus credenciales.');
        }
    } catch (error) {
        console.error('Error de conexión:', error);
        showError('Error de conexión. Verifique su conexión a internet.');
    } finally {
        setLoading(false, submitBtn, btnText, spinner);
    }
}

function setLoading(loading, btn, btnText, spinner) {
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

function showError(message) {
    const errorDiv = document.getElementById('login-error');
    if (errorDiv) {
        errorDiv.textContent = message;
        errorDiv.style.display = 'block';
        errorDiv.classList.add('shake');

        setTimeout(() => {
            errorDiv.classList.remove('shake');
        }, 600);
    }
}

function showSuccess(message) {
    const errorDiv = document.getElementById('login-error');
    if (errorDiv) {
        errorDiv.textContent = message;
        errorDiv.style.display = 'block';
        errorDiv.className = 'success-message';
    }
}

function clearError() {
    const errorDiv = document.getElementById('login-error');
    if (errorDiv) {
        errorDiv.style.display = 'none';
        errorDiv.textContent = '';
        errorDiv.className = 'error-message';
    }
}

function isTokenExpired() {
    const expiration = localStorage.getItem('tokenExpiration');
    if (!expiration) return true;

    const expirationDate = new Date(expiration);
    const now = new Date();

    return now >= expirationDate;
}

function checkAuthStatus() {
    const token = localStorage.getItem('authToken');
    if (!token || isTokenExpired()) {
        logout();
        return false;
    }
    return true;
}

function logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('userData');
    localStorage.removeItem('tokenExpiration');
    window.location.href = '/Web/index.html';
}

window.authUtils = {
    checkAuthStatus,
    logout,
    isTokenExpired
};