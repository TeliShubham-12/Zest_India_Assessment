const API_BASE = '/api';

// State
let studentsList = [];
let jwtToken = localStorage.getItem('jwt_token') || '';
let currentUser = JSON.parse(localStorage.getItem('user_info') || 'null');

document.addEventListener('DOMContentLoaded', () => {
    initApp();
});

function initApp() {
    setupAuthTabs();
    setupAuthForms();
    setupStudentEvents();
    updateUserStatus();

    // Auto load students if token exists
    if (jwtToken && currentUser) {
        fetchStudents();
    }
}

// 1. Auth Form Switching (Login <-> Register)
function setupAuthTabs() {
    const gotoRegister = document.getElementById('goto-register');
    const gotoLogin = document.getElementById('goto-login');
    const loginForm = document.getElementById('login-form');
    const registerForm = document.getElementById('register-form');

    gotoRegister?.addEventListener('click', () => {
        loginForm.classList.add('hidden');
        registerForm.classList.remove('hidden');
    });

    gotoLogin?.addEventListener('click', () => {
        registerForm.classList.add('hidden');
        loginForm.classList.remove('hidden');
    });
}

// 2. Authentication Logic
function setupAuthForms() {
    document.getElementById('login-form')?.addEventListener('submit', async (e) => {
        e.preventDefault();
        const username = document.getElementById('login-username').value;
        const password = document.getElementById('login-password').value;
        await loginUser(username, password);
    });

    document.getElementById('register-form')?.addEventListener('submit', async (e) => {
        e.preventDefault();
        const username = document.getElementById('reg-username').value;
        const email = document.getElementById('reg-email').value;
        const password = document.getElementById('reg-password').value;
        await registerUser(username, email, password);
    });

    document.getElementById('btn-quick-admin')?.addEventListener('click', () => {
        loginUser('admin', 'Admin@123');
    });

    document.getElementById('btn-logout')?.addEventListener('click', () => {
        logoutUser();
    });
}

async function loginUser(username, password) {
    try {
        const res = await fetch(`${API_BASE}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });

        const data = await res.json();
        if (data.success && data.data) {
            saveSession(data.data.token, { username: data.data.username, role: data.data.role });
            showToast(`Welcome back, ${data.data.username}!`);
            fetchStudents();
        } else {
            showToast(data.message || 'Login failed', 'error');
        }
    } catch (err) {
        showToast('Server connection error during login.', 'error');
    }
}

async function registerUser(username, email, password) {
    try {
        const res = await fetch(`${API_BASE}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, email, password })
        });

        const data = await res.json();
        if (data.success && data.data) {
            saveSession(data.data.token, { username: data.data.username, role: data.data.role });
            showToast('Account registered successfully!');
            fetchStudents();
        } else {
            showToast(data.message || 'Registration failed', 'error');
        }
    } catch (err) {
        showToast('Server connection error during registration.', 'error');
    }
}

function saveSession(token, user) {
    jwtToken = token;
    currentUser = user;
    localStorage.setItem('jwt_token', token);
    localStorage.setItem('user_info', JSON.stringify(user));
    updateUserStatus();
}

function logoutUser() {
    jwtToken = '';
    currentUser = null;
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user_info');
    updateUserStatus();
    renderStudentTable([]);
    showToast('Logged out');
}

function updateUserStatus() {
    const textEl = document.getElementById('user-info-text');
    const quickBtn = document.getElementById('btn-quick-admin');
    const logoutBtn = document.getElementById('btn-logout');
    const authSection = document.getElementById('auth-section');
    const studentSection = document.getElementById('student-section');

    if (jwtToken && currentUser) {
        if (textEl) textEl.innerHTML = `Logged in as: <strong>${currentUser.username}</strong>`;
        if (quickBtn) quickBtn.classList.add('hidden');
        if (logoutBtn) logoutBtn.classList.remove('hidden');

        // Hide auth card, show student table section
        if (authSection) authSection.classList.add('hidden');
        if (studentSection) studentSection.classList.remove('hidden');
    } else {
        if (textEl) textEl.textContent = 'Not Logged In';
        if (quickBtn) quickBtn.classList.remove('hidden');
        if (logoutBtn) logoutBtn.classList.add('hidden');

        // Show auth card, hide student table section
        if (authSection) authSection.classList.remove('hidden');
        if (studentSection) studentSection.classList.add('hidden');
    }
}

// 3. Student CRUD Functions
function setupStudentEvents() {
    document.getElementById('btn-add-student')?.addEventListener('click', () => openStudentModal());
    document.getElementById('close-modal')?.addEventListener('click', closeStudentModal);
    document.getElementById('btn-cancel')?.addEventListener('click', closeStudentModal);
    document.getElementById('student-form')?.addEventListener('submit', saveStudent);

    document.getElementById('search-input')?.addEventListener('input', (e) => {
        filterStudents(e.target.value);
    });
}

async function fetchStudents() {
    const tbody = document.getElementById('student-table-body');
    if (!tbody) return;

    tbody.innerHTML = `<tr><td colspan="7" class="text-center">Loading student records...</td></tr>`;

    try {
        const headers = {};
        if (jwtToken) {
            headers['Authorization'] = `Bearer ${jwtToken}`;
        }

        const res = await fetch(`${API_BASE}/students`, { headers });
        const data = await res.json();

        if (res.ok && data.success) {
            studentsList = data.data || [];
            renderStudentTable(studentsList);
        } else if (res.status === 401) {
            tbody.innerHTML = `<tr><td colspan="7" class="text-center">🔒 Authentication Required. Please login to view or edit students.</td></tr>`;
            logoutUser();
        } else {
            showToast(data.message || 'Failed to load students', 'error');
        }
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="7" class="text-center">Error connecting to server.</td></tr>`;
    }
}

function renderStudentTable(list) {
    const tbody = document.getElementById('student-table-body');
    if (!tbody) return;

    if (list.length === 0) {
        tbody.innerHTML = `<tr><td colspan="7" class="text-center">No student records found.</td></tr>`;
        return;
    }

    tbody.innerHTML = list.map(s => `
        <tr>
            <td><strong>#${s.id}</strong></td>
            <td>${escapeHtml(s.name)}</td>
            <td>${escapeHtml(s.email)}</td>
            <td>${s.age}</td>
            <td><span class="badge-course">${escapeHtml(s.course)}</span></td>
            <td>${new Date(s.createdDate).toLocaleDateString()}</td>
            <td>
                <div class="action-btns">
                    <button class="btn-action-edit" onclick="openStudentModal(${s.id})">Edit</button>
                    <button class="btn-action-delete" onclick="deleteStudent(${s.id}, '${escapeHtml(s.name)}')">Delete</button>
                </div>
            </td>
        </tr>
    `).join('');
}

function filterStudents(query) {
    const q = query.toLowerCase().trim();
    if (!q) {
        renderStudentTable(studentsList);
        return;
    }

    const filtered = studentsList.filter(s =>
        s.name.toLowerCase().includes(q) ||
        s.email.toLowerCase().includes(q) ||
        s.course.toLowerCase().includes(q)
    );

    renderStudentTable(filtered);
}

function openStudentModal(id = null) {
    const modal = document.getElementById('student-modal');
    const modalTitle = document.getElementById('modal-title');
    const form = document.getElementById('student-form');

    form.reset();

    if (id) {
        const student = studentsList.find(s => s.id === id);
        if (student) {
            modalTitle.textContent = `Edit Student #${id}`;
            document.getElementById('student-id').value = student.id;
            document.getElementById('student-name').value = student.name;
            document.getElementById('student-email').value = student.email;
            document.getElementById('student-age').value = student.age;
            document.getElementById('student-course').value = student.course;
        }
    } else {
        modalTitle.textContent = 'Add New Student';
        document.getElementById('student-id').value = '';
    }

    modal?.classList.remove('hidden');
}

function closeStudentModal() {
    document.getElementById('student-modal')?.classList.add('hidden');
}

async function saveStudent(e) {
    e.preventDefault();

    const id = document.getElementById('student-id').value;
    const name = document.getElementById('student-name').value;
    const email = document.getElementById('student-email').value;
    const age = parseInt(document.getElementById('student-age').value);
    const course = document.getElementById('student-course').value;

    const payload = { name, email, age, course };
    const isEdit = Boolean(id);
    const url = isEdit ? `${API_BASE}/students/${id}` : `${API_BASE}/students`;
    const method = isEdit ? 'PUT' : 'POST';

    try {
        const headers = { 'Content-Type': 'application/json' };
        if (jwtToken) headers['Authorization'] = `Bearer ${jwtToken}`;

        const res = await fetch(url, {
            method,
            headers,
            body: JSON.stringify(payload)
        });

        const data = await res.json();
        if (res.ok && data.success) {
            showToast(isEdit ? 'Student updated successfully' : 'Student created successfully');
            closeStudentModal();
            fetchStudents();
        } else {
            showToast(data.message || 'Operation failed', 'error');
        }
    } catch (err) {
        showToast('Error saving student', 'error');
    }
}

async function deleteStudent(id, name) {
    if (!confirm(`Are you sure you want to delete student '${name}'?`)) return;

    try {
        const headers = {};
        if (jwtToken) headers['Authorization'] = `Bearer ${jwtToken}`;

        const res = await fetch(`${API_BASE}/students/${id}`, {
            method: 'DELETE',
            headers
        });

        const data = await res.json();
        if (res.ok && data.success) {
            showToast(`Student #${id} deleted`);
            fetchStudents();
        } else {
            showToast(data.message || 'Failed to delete student', 'error');
        }
    } catch (err) {
        showToast('Error deleting student', 'error');
    }
}

function showToast(message) {
    const toast = document.getElementById('toast-message');
    if (!toast) return;

    toast.textContent = message;
    toast.classList.remove('hidden');

    setTimeout(() => {
        toast.classList.add('hidden');
    }, 3000);
}

function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/[&<>"']/g, function(m) {
        return {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        }[m];
    });
}
