document.addEventListener('DOMContentLoaded', function () {
    const registerForm = document.getElementById('registerForm');

    if (registerForm) {
        registerForm.addEventListener('submit', handleRegister);
    }

    if (localStorage.getItem('authToken')) {
        window.location.href = '/Web/dashboard.html';
    }

    setupFormValidation();
});

function setupFormValidation() {
    const cedulaInput = document.getElementById('cedula');
    const telefonoInput = document.getElementById('telefono');
    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');

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

    if (emailInput) {
        emailInput.addEventListener('blur', validateEmail);
    }

    if (passwordInput) {
        passwordInput.addEventListener('input', validatePassword);
    }
}

function validateEmail(e) {
    const email = e.target.value;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (email && !emailRegex.test(email)) {
        e.target.setCustomValidity('Por favor, ingrese un email válido');
        e.target.classList.add('invalid');
    } else {
        e.target.setCustomValidity('');
        e.target.classList.remove('invalid');
    }
}

function validatePassword(e) {
    const password = e.target.value;
    const passwordHelper = document.getElementById('password-helper');

    if (password.length < 6) {
        e.target.setCustomValidity('La contraseña debe tener al menos 6 caracteres');
        e.target.classList.add('invalid');
    } else {
        e.target.setCustomValidity('');
        e.target.classList.remove('invalid');
    }
}

function validateCedula(cedula) {
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
}

async function handleRegister(e) {
    e.preventDefault();

    const submitBtn = e.target.querySelector('button[type="submit"]');
    const btnText = submitBtn.querySelector('.btn-text');
    const spinner = submitBtn.querySelector('.spinner');

    const formData = {
        cedula: document.getElementById('cedula').value.trim(),
        nombres: document.getElementById('nombres').value.trim(),
        apellidos: document.getElementById('apellidos').value.trim(),
        email: document.getElementById('email').value.trim(),
        telefono: document.getElementById('telefono').value.trim(),
        password: document.getElementById('password').value,
        rolId: 2
    };

    if (!validateForm(formData)) {
        return;
    }

    setLoading(true, submitBtn, btnText, spinner);
    clearMessages();

    try {
        const response = await fetch('/api/Auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });

        const result = await response.json();

        if (result.success) {
            showSuccess('¡Registro exitoso! Redirigiendo al login...');

            setTimeout(() => {
                window.location.href = '/Web/index.html';
            }, 2000);
        } else {
            showError(result.message || 'Error al registrar usuario.');
        }
    } catch (error) {
        console.error('Error de conexión:', error);
        showError('Error de conexión. Verifique su conexión a internet.');
    } finally {
        setLoading(false, submitBtn, btnText, spinner);
    }
}

function validateForm(formData) {
    if (!formData.cedula || !formData.nombres || !formData.apellidos ||
        !formData.email || !formData.password) {
        showError('Por favor, complete todos los campos obligatorios.');
        return false;
    }

    if (!validateCedula(formData.cedula)) {
        showError('Por favor, ingrese una cédula válida.');
        return false;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(formData.email)) {
        showError('Por favor, ingrese un email válido.');
        return false;
    }

    if (formData.password.length < 6) {
        showError('La contraseña debe tener al menos 6 caracteres.');
        return false;
    }

    if (formData.telefono && formData.telefono.length < 7) {
        showError('Por favor, ingrese un teléfono válido.');
        return false;
    }

    return true;
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
    const errorDiv = document.getElementById('register-error');
    const successDiv = document.getElementById('register-success');

    if (successDiv) successDiv.style.display = 'none';

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
    const errorDiv = document.getElementById('register-error');
    const successDiv = document.getElementById('register-success');

    if (errorDiv) errorDiv.style.display = 'none';

    if (successDiv) {
        successDiv.textContent = message;
        successDiv.style.display = 'block';
    }
}

function clearMessages() {
    const errorDiv = document.getElementById('register-error');
    const successDiv = document.getElementById('register-success');

    if (errorDiv) {
        errorDiv.style.display = 'none';
        errorDiv.textContent = '';
    }

    if (successDiv) {
        successDiv.style.display = 'none';
        successDiv.textContent = '';
    }
}