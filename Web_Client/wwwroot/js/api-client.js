const OsyApi = {
    baseUrl: (document.querySelector('meta[name="api-base-url"]')?.content || '').replace(/\/$/, ''),

    async request(path, options = {}) {
        const headers = { 'Accept': 'application/json', ...(options.headers || {}) };
        const token = localStorage.getItem('osy_access_token');

        if (!options.anonymous && token) {
            headers.Authorization = `Bearer ${token}`;
        }

        let body = options.body;
        if (body !== undefined && body !== null && !(body instanceof FormData)) {
            headers['Content-Type'] = 'application/json';
            body = JSON.stringify(body);
        }

        OsyUi.loading(true);
        try {
            const response = await fetch(`${this.baseUrl}${path}`, {
                method: options.method || 'GET',
                headers,
                body
            });

            const text = await response.text();
            let payload = null;
            if (text) {
                try { payload = JSON.parse(text); } catch { payload = text; }
            }

            if (!response.ok) {
                if (response.status === 401 && !options.anonymous) {
                    OsyAuth.logout(false);
                }
                const message = payload?.message || payload?.title || (typeof payload === 'string' ? payload : `HTTP ${response.status}`);
                const error = new Error(message);
                error.status = response.status;
                error.payload = payload;
                throw error;
            }

            return payload;
        } catch (error) {
            if (error instanceof TypeError) {
                throw new Error(`Không kết nối được Web API tại ${this.baseUrl}. Hãy kiểm tra API đã chạy và CORS đã bật.`);
            }
            throw error;
        } finally {
            OsyUi.loading(false);
        }
    }
};

const OsyAuth = {
    saveSession(loginResponse) {
        localStorage.setItem('osy_access_token', loginResponse.accessToken);
        localStorage.setItem('osy_user', JSON.stringify({
            accountId: loginResponse.accountId,
            username: loginResponse.username,
            fullName: loginResponse.fullName,
            role: loginResponse.role,
            kitchenId: loginResponse.kitchenId,
            kitchenName: loginResponse.kitchenName,
            expiresAt: loginResponse.expiresAt
        }));
    },
    getUser() {
        try { return JSON.parse(localStorage.getItem('osy_user') || 'null'); } catch { return null; }
    },
    getRoleRedirect() {
        const roleMap = {
            SystemAdmin: '/Admin/Dashboard',
            KitchenManager: '/KitchenManager/Dashboard',
            BranchManager: '/BranchManager/Dashboard',
            Staff: '/Staff/Orders'
        };
        const user = this.getUser();
        return roleMap[user?.role] || '/Auth/Login';
    },
    requireLogin() {
        if (!localStorage.getItem('osy_access_token')) window.location.href = '/Auth/Login';
        const redirect = this.getRoleRedirect();
        if (location.pathname === '/' || location.pathname === '/Auth/Login') {
            window.location.href = redirect;
        }
    },
    renderCurrentUser() {
        const user = this.getUser();
        if (!user) return;
        const name = document.getElementById('currentUserName');
        const role = document.getElementById('currentUserRole');
        const initial = document.getElementById('currentUserInitial');
        if (name) name.textContent = user.fullName || user.username;
        if (role) role.textContent = `${user.role}${user.kitchenName ? ` · ${user.kitchenName}` : ''}`;
        if (initial) initial.textContent = (user.fullName || user.username || 'O').charAt(0).toUpperCase();
        const parts = location.pathname.split('/').filter(Boolean);
        const navKey = parts.length >= 2 ? parts[1] : (parts[0] || 'Dashboard');
        document.querySelector(`[data-nav="${navKey}"]`)?.classList.add('active');
    },
    logout(redirect = true) {
        localStorage.removeItem('osy_access_token');
        localStorage.removeItem('osy_user');
        if (redirect) window.location.href = '/Auth/Login';
    }
};

const OsyUi = {
    loading(show) { document.getElementById('loadingOverlay')?.classList.toggle('show', show); },
    alert(targetId, message, type = 'success') {
        const target = document.getElementById(targetId);
        if (!target) return;
        target.innerHTML = `<div class="alert alert-${type} alert-dismissible fade show" role="alert">${this.escape(message)}<button type="button" class="btn-close" data-bs-dismiss="alert"></button></div>`;
    },
    global(message, type = 'success') { this.alert('globalAlert', message, type); },
    toast(message, type = 'success') {
        const container = document.getElementById('toastContainer');
        if (!container) return;
        const icons = { success: '&#10003;', danger: '&#10007;', warning: '&#9888;', info: '&#8505;' };
        const el = document.createElement('div');
        el.className = `toast-item toast-${type}`;
        el.innerHTML = `<span class="toast-icon">${icons[type] || icons.info}</span><span class="toast-msg">${this.escape(message)}</span>`;
        container.appendChild(el);
        setTimeout(() => { el.classList.add('toast-hide'); setTimeout(() => el.remove(), 400); }, 3500);
    },
    confirm(message) {
        return new Promise(function (resolve) {
            let modal = document.getElementById('confirmModal');
            if (!modal) {
                modal = document.createElement('div');
                modal.className = 'modal fade';
                modal.id = 'confirmModal';
                modal.innerHTML = '<div class="modal-dialog modal-sm modal-dialog-centered"><div class="modal-content"><div class="modal-body text-center py-4"><div class="confirm-icon mb-3">&#9888;</div><p class="confirm-msg mb-4"></p><div class="d-flex justify-content-center gap-3"><button class="btn btn-light btn-cancel" data-bs-dismiss="modal">Huỷ</button><button class="btn btn-brand btn-ok">Xác nhận</button></div></div></div></div>';
                document.body.appendChild(modal);
            }
            modal.querySelector('.confirm-msg').textContent = message;
            const instance = bootstrap.Modal.getOrCreateInstance(modal);
            modal.querySelector('.btn-ok').onclick = function () { instance.hide(); resolve(true); };
            modal.querySelector('.btn-cancel').onclick = function () { instance.hide(); resolve(false); };
            modal.addEventListener('hidden.bs.modal', function () { resolve(false); }, { once: true });
            instance.show();
        });
    },
    showDetail(title, bodyHtml) {
        let modal = document.getElementById('detailModal');
        if (!modal) {
            modal = document.createElement('div');
            modal.className = 'modal fade';
            modal.id = 'detailModal';
            modal.innerHTML = '<div class="modal-dialog modal-lg modal-dialog-scrollable"><div class="modal-content"><div class="modal-header"><h5 class="modal-title"></h5><button type="button" class="btn-close" data-bs-dismiss="modal"></button></div><div class="modal-body"></div><div class="modal-footer"><button class="btn btn-light" data-bs-dismiss="modal">Đóng</button></div></div></div>';
            document.body.appendChild(modal);
        }
        modal.querySelector('.modal-title').innerHTML = this.escape(title);
        modal.querySelector('.modal-body').innerHTML = bodyHtml;
        const instance = bootstrap.Modal.getOrCreateInstance(modal);
        instance.show();
    },
    escape(value) {
        return String(value ?? '')
            .replaceAll('&', '&amp;').replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;').replaceAll('"', '&quot;').replaceAll("'", '&#039;');
    },
    money(value) { return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(Number(value || 0)); },
    number(value) { return new Intl.NumberFormat('vi-VN', { maximumFractionDigits: 3 }).format(Number(value || 0)); },
    date(value) { return value ? new Intl.DateTimeFormat('vi-VN', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value)) : '-'; },
    badge(status) { return `<span class="status-badge status-${String(status).toLowerCase()}">${this.escape(status)}</span>`; },
    modal(id) { return bootstrap.Modal.getOrCreateInstance(document.getElementById(id)); }
};
