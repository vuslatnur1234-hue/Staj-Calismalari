// js/common.js

// --- MODAL İŞLEMLERİ ---
function showModal() {
    document.getElementById('modalOverlay').classList.add('open');
}

function closeModal(e) {
    if (e && e.target !== document.getElementById('modalOverlay')) return;
    document.getElementById('modalOverlay').classList.remove('open');
}

// --- BİLDİRİM (TOAST) ---
function showToast(msg, type = '') {
    const t = document.getElementById('toast');
    t.textContent = msg;
    t.className = `toast${type ? ' ' + type : ''}`;
    t.classList.add('show');
    setTimeout(() => t.classList.remove('show'), 3000);
}

// --- YARDIMCI FONKSİYONLAR ---
function val(id) { return (document.getElementById('f-' + id)?.value || '').trim(); }
function num(id) { return parseInt(document.getElementById('f-' + id)?.value || '0', 10) || 0; }

function esc(str) {
    if (!str) return '';
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

function fmtDate(str) {
    if (!str || str === 'Henüz teslim edilmedi') return str || '—';
    try {
        const d = new Date(str);
        return d.toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' });
    } catch { return str; }
}