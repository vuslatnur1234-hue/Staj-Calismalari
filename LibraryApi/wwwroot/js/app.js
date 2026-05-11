// --- STATE / DEĞİŞKENLER ---
let currentPage = 'dashboard';
let currentModal = null;   // 'addBook' | 'editBook' | 'addMember' | 'editMember' | 'addLoan'
let editTarget = null;   // { type, id }

let allBooks = [];
let allMembers = [];
let allLoans = [];

// --- INIT (Sayfa Yüklendiğinde) ---
document.addEventListener('DOMContentLoaded', () => {
    // Nav tıklamaları
    document.querySelectorAll('.nav-item').forEach(btn => {
        btn.addEventListener('click', () => navigate(btn.dataset.page));
    });

    // Arama
    document.getElementById('searchInput').addEventListener('input', e => {
        filterTable(e.target.value.toLowerCase());
    });

    loadDashboard();
});

// --- NAVIGATION ---
function navigate(page) {
    currentPage = page;

    document.querySelectorAll('.nav-item').forEach(b => b.classList.remove('active'));
    document.querySelector(`.nav-item[data-page="${page}"]`).classList.add('active');

    document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
    document.getElementById(`page-${page}`).classList.add('active');

    const titles = { dashboard: 'Dashboard', books: 'Kitaplar', members: 'Üyeler', loans: 'Ödünç İşlemleri' };
    document.getElementById('pageTitle').textContent = titles[page];

    const addBtn = document.getElementById('addBtn');
    addBtn.style.display = page === 'dashboard' ? 'none' : 'inline-block';

    document.getElementById('searchInput').value = '';

    if (page === 'books') loadBooks();
    if (page === 'members') loadMembers();
    if (page === 'loans') loadLoans();
}

// --- SEARCH / FILTER ---
function filterTable(q) {
    const tableMap = { books: 'booksBody', members: 'membersBody', loans: 'loansBody' };
    const tbodyId = tableMap[currentPage];
    if (!tbodyId) return;

    document.querySelectorAll(`#${tbodyId} tr`).forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(q) ? '' : 'none';
    });
}

// --- DASHBOARD ---
async function loadDashboard() {
    try {
        const [books, members, loans] = await Promise.all([
            apiFetch('Books'),
            apiFetch('Uyeler'),
            apiFetch('OduncIslemleri'),
        ]);

        allBooks = books;
        allMembers = members;
        allLoans = loans;

        document.getElementById('stat-books').textContent = books.length;
        document.getElementById('stat-members').textContent = members.length;

        const active = loans.filter(l => l.durum !== 'Teslim Edildi');
        const returned = loans.filter(l => l.durum === 'Teslim Edildi');
        document.getElementById('stat-loans').textContent = active.length;
        document.getElementById('stat-returned').textContent = returned.length;

        // Son 5 kitap
        const recentBooks = [...books].slice(-5).reverse();
        document.getElementById('recentBooks').innerHTML = recentBooks.length
            ? recentBooks.map(b => `
          <div class="mini-item">
            <span class="mini-item-name">${esc(b.kitapAd)}</span>
            <span class="mini-item-meta">${esc(b.yazarAd)}</span>
          </div>`).join('')
            : '<div class="mini-item"><span class="mini-item-meta">Kayıt yok</span></div>';

        // Son 5 ödünç
        const recentLoans = [...loans].slice(-5).reverse();
        document.getElementById('recentLoans').innerHTML = recentLoans.length
            ? recentLoans.map(l => `
          <div class="mini-item">
            <span class="mini-item-name">${esc(l.kitapAd)}</span>
            <span class="mini-item-meta">${esc(l.uyeAdSoyad)}</span>
          </div>`).join('')
            : '<div class="mini-item"><span class="mini-item-meta">Kayıt yok</span></div>';

    } catch (err) {
        showToast('Dashboard yüklenemedi: ' + err.message, 'error');
    }
}

// --- BOOKS ---
async function loadBooks() {
    const tbody = document.getElementById('booksBody');
    tbody.innerHTML = '<tr><td colspan="6" class="loading-cell">Yükleniyor...</td></tr>';

    try {
        allBooks = await apiFetch('Books');
        renderBooks(allBooks);
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="6" class="loading-cell">Hata: ${esc(err.message)}</td></tr>`;
    }
}

function renderBooks(list) {
    const tbody = document.getElementById('booksBody');
    if (!list.length) {
        tbody.innerHTML = '<tr><td colspan="6" class="loading-cell">Kayıt bulunamadı</td></tr>';
        return;
    }
    tbody.innerHTML = list.map(b => `
    <tr>
      <td><span style="font-family:'DM Mono',monospace;font-size:11px;color:var(--text-dim)">${b.kitapID}</span></td>
      <td style="color:var(--cream)">${esc(b.kitapAd)}</td>
      <td>${esc(b.yazarAd)}</td>
      <td><span class="badge badge-returned">${esc(b.turAd)}</span></td>
      <td><span style="font-family:'DM Mono',monospace;font-size:12px">${esc(b.rafNo)}</span></td>
      <td>
        <button class="btn-icon" title="Düzenle" onclick="openEditBook(${b.kitapID})">✎</button>
        <button class="btn-icon btn-del" title="Sil" onclick="deleteBook(${b.kitapID}, '${esc(b.kitapAd)}')">✕</button>
      </td>
    </tr>`).join('');
}

// --- MEMBERS ---
async function loadMembers() {
    const tbody = document.getElementById('membersBody');
    tbody.innerHTML = '<tr><td colspan="7" class="loading-cell">Yükleniyor...</td></tr>';

    try {
        allMembers = await apiFetch('Uyeler');
        renderMembers(allMembers);
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="7" class="loading-cell">Hata: ${esc(err.message)}</td></tr>`;
    }
}

function renderMembers(list) {
    const tbody = document.getElementById('membersBody');
    if (!list.length) {
        tbody.innerHTML = '<tr><td colspan="7" class="loading-cell">Kayıt bulunamadı</td></tr>';
        return;
    }
    tbody.innerHTML = list.map(u => `
    <tr>
      <td><span style="font-family:'DM Mono',monospace;font-size:11px;color:var(--text-dim)">${u.uyeID}</span></td>
      <td style="color:var(--cream)">${esc(u.ad)}</td>
      <td>${esc(u.soyad)}</td>
      <td><span style="font-family:'DM Mono',monospace;font-size:12px">${esc(u.telefon)}</span></td>
      <td>${esc(u.email)}</td>
      <td><span style="font-family:'DM Mono',monospace;font-size:11px;color:var(--text-dim)">${fmtDate(u.kayitTarihi)}</span></td>
      <td>
        <button class="btn-icon" title="Düzenle" onclick="openEditMember(${u.uyeID})">✎</button>
        <button class="btn-icon btn-del" title="Sil" onclick="deleteMember(${u.uyeID}, '${esc(u.ad)} ${esc(u.soyad)}')">✕</button>
        <button class="btn-icon btn-loan" title="Ödünç Ver" onclick="openLoanForMember(${u.uyeID})">⊕</button>
      </td>
    </tr>`).join('');
}

// --- LOANS ---
async function loadLoans() {
    const tbody = document.getElementById('loansBody');
    tbody.innerHTML = '<tr><td colspan="7" class="loading-cell">Yükleniyor...</td></tr>';

    try {
        allLoans = await apiFetch('OduncIslemleri');
        renderLoans(allLoans);
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="7" class="loading-cell">Hata: ${esc(err.message)}</td></tr>`;
    }
}

function renderLoans(list) {
    const tbody = document.getElementById('loansBody');
    if (!list.length) {
        tbody.innerHTML = '<tr><td colspan="7" class="loading-cell">Kayıt bulunamadı</td></tr>';
        return;
    }
    tbody.innerHTML = list.map(l => {
        const teslim = l.teslimTarihi && l.teslimTarihi !== 'Henüz teslim edilmedi'
            ? fmtDate(l.teslimTarihi) : '<span style="color:var(--text-dim)">—</span>';
        const aktif = l.durum !== 'Teslim Edildi';
        return `
    <tr>
      <td><span style="font-family:'DM Mono',monospace;font-size:11px;color:var(--text-dim)">${l.islemID}</span></td>
      <td style="color:var(--cream)">${esc(l.uyeAdSoyad)}</td>
      <td>${esc(l.kitapAd)}</td>
      <td><span style="font-family:'DM Mono',monospace;font-size:12px">${fmtDate(l.verilisTarihi)}</span></td>
      <td><span style="font-family:'DM Mono',monospace;font-size:12px">${teslim}</span></td>
      <td><span class="badge ${aktif ? 'badge-active' : 'badge-returned'}">${esc(l.durum)}</span></td>
      <td>
        ${aktif
                ? `<button class="btn-icon btn-loan" title="Teslim Al" onclick="teslimAl(${l.islemID})">✓</button>`
                : '<span style="font-size:11px;color:var(--text-dim)">—</span>'}
      </td>
    </tr>`;
    }).join('');
}

// --- MODAL — OPEN ---
function openAddModal() {
    if (currentPage === 'books') openAddBook();
    if (currentPage === 'members') openAddMember();
    if (currentPage === 'loans') openAddLoan();
}

// ── ADD BOOK
function openAddBook() {
    currentModal = 'addBook';
    editTarget = null;
    document.getElementById('modalTitle').textContent = 'Yeni Kitap Ekle';
    document.getElementById('modalBody').innerHTML = `
    <div class="form-group">
      <label class="form-label">Kitap Adı</label>
      <input class="form-input" id="f-kitapAd" placeholder="Kitap adını girin">
    </div>
    <div class="form-grid">
      <div class="form-group">
        <label class="form-label">Yazar ID</label>
        <input class="form-input" id="f-yazarID" type="number" placeholder="1">
      </div>
      <div class="form-group">
        <label class="form-label">Tür ID</label>
        <input class="form-input" id="f-turID" type="number" placeholder="1">
      </div>
    </div>
    <div class="form-grid">
      <div class="form-group">
        <label class="form-label">Sayfa Sayısı</label>
        <input class="form-input" id="f-sayfa" type="number" placeholder="320">
      </div>
      <div class="form-group">
        <label class="form-label">Basım Yılı</label>
        <input class="form-input" id="f-yil" placeholder="2024">
      </div>
    </div>
    <div class="form-grid">
      <div class="form-group">
        <label class="form-label">Stok Adedi</label>
        <input class="form-input" id="f-stok" type="number" placeholder="5">
      </div>
      <div class="form-group">
        <label class="form-label">Raf No</label>
        <input class="form-input" id="f-rafNo" placeholder="BG-1">
      </div>
    </div>`;
    showModal();
}

// ── EDIT BOOK
async function openEditBook(id) {
    currentModal = 'editBook';
    editTarget = { type: 'book', id };
    document.getElementById('modalTitle').textContent = 'Kitap Düzenle';
    document.getElementById('modalBody').innerHTML = '<div style="padding:20px;text-align:center;color:var(--text-dim)">Yükleniyor...</div>';
    showModal();

    try {
        const b = await apiFetch(`Books/${id}`);
        document.getElementById('modalBody').innerHTML = `
      <div class="form-group">
        <label class="form-label">Kitap Adı</label>
        <input class="form-input" id="f-kitapAd" value="${esc(b.kitapAd)}">
      </div>
      <div class="form-grid">
        <div class="form-group">
          <label class="form-label">Stok Adedi</label>
          <input class="form-input" id="f-stok" type="number" value="5">
        </div>
        <div class="form-group">
          <label class="form-label">Raf No</label>
          <input class="form-input" id="f-rafNo" value="${esc(b.rafNo)}">
        </div>
      </div>`;
    } catch (err) {
        showToast('Kitap yüklenemedi', 'error');
        closeModal();
    }
}

// ── ADD MEMBER
function openAddMember() {
    currentModal = 'addMember';
    editTarget = null;
    document.getElementById('modalTitle').textContent = 'Yeni Üye Ekle';
    document.getElementById('modalBody').innerHTML = `
    <div class="form-grid">
      <div class="form-group">
        <label class="form-label">Ad</label>
        <input class="form-input" id="f-ad" placeholder="Ad">
      </div>
      <div class="form-group">
        <label class="form-label">Soyad</label>
        <input class="form-input" id="f-soyad" placeholder="Soyad">
      </div>
    </div>
    <div class="form-group">
      <label class="form-label">Telefon</label>
      <input class="form-input" id="f-telefon" placeholder="05xx xxx xx xx">
    </div>
    <div class="form-group">
      <label class="form-label">E-posta</label>
      <input class="form-input" id="f-email" type="email" placeholder="ornek@mail.com">
    </div>`;
    showModal();
}

// ── EDIT MEMBER
async function openEditMember(id) {
    currentModal = 'editMember';
    editTarget = { type: 'member', id };
    document.getElementById('modalTitle').textContent = 'Üye Düzenle';
    document.getElementById('modalBody').innerHTML = '<div style="padding:20px;text-align:center;color:var(--text-dim)">Yükleniyor...</div>';
    showModal();

    try {
        const u = await apiFetch(`Uyeler/${id}`);
        document.getElementById('modalBody').innerHTML = `
      <div class="form-grid">
        <div class="form-group">
          <label class="form-label">Ad</label>
          <input class="form-input" id="f-ad" value="${esc(u.ad)}">
        </div>
        <div class="form-group">
          <label class="form-label">Soyad</label>
          <input class="form-input" id="f-soyad" value="${esc(u.soyad)}">
        </div>
      </div>
      <div class="form-group">
        <label class="form-label">Telefon</label>
        <input class="form-input" id="f-telefon" value="${esc(u.telefon)}">
      </div>
      <div class="form-group">
        <label class="form-label">E-posta</label>
        <input class="form-input" id="f-email" type="email" value="${esc(u.email)}">
      </div>`;
    } catch (err) {
        showToast('Üye yüklenemedi', 'error');
        closeModal();
    }
}

// ── ADD LOAN
async function openAddLoan(preUyeId = null) {
    currentModal = 'addLoan';
    editTarget = null;
    document.getElementById('modalTitle').textContent = 'Ödünç Ver';
    document.getElementById('modalBody').innerHTML = '<div style="padding:20px;text-align:center;color:var(--text-dim)">Yükleniyor...</div>';
    showModal();

    try {
        if (!allMembers.length) allMembers = await apiFetch('Uyeler');
        if (!allBooks.length) allBooks = await apiFetch('Books');

        const memberOptions = allMembers.map(u =>
            `<option value="${u.uyeID}" ${u.uyeID == preUyeId ? 'selected' : ''}>${esc(u.ad)} ${esc(u.soyad)}</option>`
        ).join('');

        const bookOptions = allBooks.map(b =>
            `<option value="${b.kitapID}">${esc(b.kitapAd)}</option>`
        ).join('');

        document.getElementById('modalBody').innerHTML = `
      <div class="form-group">
        <label class="form-label">Üye</label>
        <select class="form-select" id="f-uyeID">${memberOptions}</select>
      </div>
      <div class="form-group">
        <label class="form-label">Kitap</label>
        <select class="form-select" id="f-kitapID">${bookOptions}</select>
      </div>`;
    } catch (err) {
        showToast('Veriler yüklenemedi', 'error');
        closeModal();
    }
}

function openLoanForMember(uyeId) {
    navigate('loans');
    setTimeout(() => openAddLoan(uyeId), 100);
}

// --- MODAL — SAVE ---
async function handleSave() {
    const btn = document.getElementById('modalSaveBtn');
    btn.disabled = true;
    btn.textContent = 'Kaydediliyor...';

    try {
        if (currentModal === 'addBook') await saveAddBook();
        if (currentModal === 'editBook') await saveEditBook();
        if (currentModal === 'addMember') await saveAddMember();
        if (currentModal === 'editMember') await saveEditMember();
        if (currentModal === 'addLoan') await saveAddLoan();
        closeModal();
    } catch (err) {
        showToast('Hata: ' + err.message, 'error');
    } finally {
        btn.disabled = false;
        btn.textContent = 'Kaydet';
    }
}

async function saveAddBook() {
    const body = {
        kitapAd: val('kitapAd'),
        yazarID: num('yazarID'),
        turID: num('turID'),
        sayfaSayisi: num('sayfa'),
        basimYili: val('yil'),
        stokAdeti: num('stok'),
        rafNo: val('rafNo'),
    };
    await apiPost('Books', body);
    showToast('Kitap eklendi ✓', 'success');
    loadBooks();
}

async function saveEditBook() {
    const body = {
        kitapAd: val('kitapAd'),
        stokAdeti: num('stok'),
        rafNo: val('rafNo'),
        yazarID: 0, turID: 0, sayfaSayisi: 0, basimYili: ''
    };
    await apiPut(`Books/${editTarget.id}`, body);
    showToast('Kitap güncellendi ✓', 'success');
    loadBooks();
}

async function saveAddMember() {
    const body = { ad: val('ad'), soyad: val('soyad'), telefon: val('telefon'), email: val('email') };
    await apiPost('Uyeler', body);
    showToast('Üye eklendi ✓', 'success');
    loadMembers();
}

async function saveEditMember() {
    const body = { ad: val('ad'), soyad: val('soyad'), telefon: val('telefon'), email: val('email') };
    await apiPut(`Uyeler/${editTarget.id}`, body);
    showToast('Üye güncellendi ✓', 'success');
    loadMembers();
}

async function saveAddLoan() {
    const body = { uyeID: num('uyeID'), kitapID: num('kitapID') };
    await apiPost('OduncIslemleri', body);
    showToast('Ödünç işlemi oluşturuldu ✓', 'success');
    loadLoans();
}

// --- DELETE / TESLIM ---
async function deleteBook(id, name) {
    if (!confirm(`"${name}" silinsin mi?`)) return;
    try {
        await apiDelete(`Books/${id}`);
        showToast('Kitap silindi', 'success');
        loadBooks();
    } catch (err) { showToast('Silinemedi: ' + err.message, 'error'); }
}

async function deleteMember(id, name) {
    if (!confirm(`"${name}" silinsin mi?`)) return;
    try {
        await apiDelete(`Uyeler/${id}`);
        showToast('Üye silindi', 'success');
        loadMembers();
    } catch (err) { showToast('Silinemedi: ' + err.message, 'error'); }
}

async function teslimAl(id) {
    if (!confirm(`${id} numaralı işlem teslim alınsın mı?`)) return;
    try {
        await apiPut(`OduncIslemleri/${id}/teslim`, {});
        showToast('Teslim alındı ✓', 'success');
        loadLoans();
    } catch (err) { showToast('Hata: ' + err.message, 'error'); }
}

// --- MODAL HELPERS ---
function showModal() { document.getElementById('modalOverlay').classList.add('open'); }

function closeModal(e) {
    if (e && e.target !== document.getElementById('modalOverlay')) return;
    document.getElementById('modalOverlay').classList.remove('open');
    currentModal = null;
    editTarget = null;
}

// --- INPUT HELPERS ---
function val(id) { return (document.getElementById('f-' + id)?.value || '').trim(); }
function num(id) { return parseInt(document.getElementById('f-' + id)?.value || '0', 10) || 0; }

// --- ESCAPE HTML ---
function esc(str) {
    if (!str) return '';
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

// --- DATE FORMAT ---
function fmtDate(str) {
    if (!str || str === 'Henüz teslim edilmedi') return str || '—';
    try {
        const d = new Date(str);
        return d.toLocaleDateString('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' });
    } catch { return str; }
}

// --- TOAST ---
function showToast(msg, type = '') {
    const t = document.getElementById('toast');
    t.textContent = msg;
    t.className = `toast${type ? ' ' + type : ''}`;
    t.classList.add('show');
    setTimeout(() => t.classList.remove('show'), 3000);
}