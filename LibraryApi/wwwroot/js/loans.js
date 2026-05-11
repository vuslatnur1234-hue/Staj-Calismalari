let allMembers = [];
let allBooks = [];

document.addEventListener('DOMContentLoaded', () => {
    loadLoans();

    // Arama işlevi
    document.getElementById('searchInput').addEventListener('input', e => {
        const q = e.target.value.toLowerCase();
        document.querySelectorAll('#loansBody tr').forEach(row => {
            row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
        });
    });

    // Eğer üyeler sayfasından "?uyeId=X" parametresiyle gelindiyse modalı otomatik aç
    const urlParams = new URLSearchParams(window.location.search);
    const preUyeId = urlParams.get('uyeId');
    if (preUyeId) {
        openAddLoan(preUyeId);
        // Sayfa yenilendiğinde tekrar açılmasın diye URL'yi temizle
        window.history.replaceState({}, document.title, window.location.pathname);
    }
});

async function loadLoans() {
    const tbody = document.getElementById('loansBody');
    try {
        const loans = await apiFetch('OduncIslemleri');
        if (!loans.length) {
            tbody.innerHTML = '<tr><td colspan="7" class="loading-cell">Kayıt bulunamadı</td></tr>';
            return;
        }
        tbody.innerHTML = loans.map(l => {
            const teslim = l.teslimTarihi && l.teslimTarihi !== 'Henüz teslim edilmedi' ? fmtDate(l.teslimTarihi) : '<span style="color:var(--text-dim)">—</span>';
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
                ${aktif ? `<button class="btn-icon btn-loan" title="Teslim Al" onclick="teslimAl(${l.islemID})">✓</button>` : '<span style="font-size:11px;color:var(--text-dim)">—</span>'}
              </td>
            </tr>`;
        }).join('');
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="7" class="loading-cell">Hata: ${esc(err.message)}</td></tr>`;
    }
}

async function openAddLoan(preUyeId = null) {
    document.getElementById('modalBody').innerHTML = 'Yükleniyor...';
    showModal();

    try {
        if (!allMembers.length) allMembers = await apiFetch('Uyeler');
        if (!allBooks.length) allBooks = await apiFetch('Books');

        const memberOptions = allMembers.map(u => `<option value="${u.uyeID}" ${u.uyeID == preUyeId ? 'selected' : ''}>${esc(u.ad)} ${esc(u.soyad)}</option>`).join('');
        const bookOptions = allBooks.map(b => `<option value="${b.kitapID}">${esc(b.kitapAd)}</option>`).join('');

        document.getElementById('modalBody').innerHTML = `
      <div class="form-group"><label class="form-label">Üye</label><select class="form-select" id="f-uyeID">${memberOptions}</select></div>
      <div class="form-group"><label class="form-label">Kitap</label><select class="form-select" id="f-kitapID">${bookOptions}</select></div>`;
    } catch (err) {
        showToast('Veriler yüklenemedi', 'error');
        closeModal();
    }
}

async function saveLoan() {
    const btn = document.getElementById('modalSaveBtn');
    btn.disabled = true; btn.textContent = 'Kaydediliyor...';
    try {
        const body = { uyeID: num('uyeID'), kitapID: num('kitapID') };
        await apiPost('OduncIslemleri', body);
        showToast('Ödünç işlemi oluşturuldu', 'success');
        closeModal();
        loadLoans();
    } catch (err) {
        showToast('Hata: ' + err.message, 'error');
    } finally {
        btn.disabled = false; btn.textContent = 'Kaydet';
    }
}

async function teslimAl(id) {
    if (!confirm(`${id} numaralı işlem teslim alınsın mı?`)) return;
    try {
        await apiPut(`OduncIslemleri/${id}/teslim`, {});
        showToast('Teslim alındı ✓', 'success');
        loadLoans();
    } catch (err) { showToast('Hata: ' + err.message, 'error'); }
}