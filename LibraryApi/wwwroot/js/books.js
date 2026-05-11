let editTargetId = null;

// Sayfa yüklendiğinde kitapları getir
document.addEventListener('DOMContentLoaded', () => {
    loadBooks();

    // Arama işlevi
    document.getElementById('searchInput').addEventListener('input', e => {
        const q = e.target.value.toLowerCase();
        document.querySelectorAll('#booksBody tr').forEach(row => {
            row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
        });
    });
});

async function loadBooks() {
    const tbody = document.getElementById('booksBody');
    try {
        const books = await apiFetch('Books');
        if (!books.length) {
            tbody.innerHTML = '<tr><td colspan="6" class="loading-cell">Kayıt bulunamadı</td></tr>';
            return;
        }
        tbody.innerHTML = books.map(b => `
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
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="6" class="loading-cell">Hata: ${esc(err.message)}</td></tr>`;
    }
}

function openAddBook() {
    editTargetId = null;
    document.getElementById('modalTitle').textContent = 'Yeni Kitap Ekle';
    document.getElementById('modalBody').innerHTML = `
    <div class="form-group">
      <label class="form-label">Kitap Adı</label>
      <input class="form-input" id="f-kitapAd" placeholder="Kitap adını girin">
    </div>
    <div class="form-grid">
      <div class="form-group"><label class="form-label">Yazar ID</label><input class="form-input" id="f-yazarID" type="number" placeholder="1"></div>
      <div class="form-group"><label class="form-label">Tür ID</label><input class="form-input" id="f-turID" type="number" placeholder="1"></div>
    </div>
    <div class="form-grid">
      <div class="form-group"><label class="form-label">Sayfa</label><input class="form-input" id="f-sayfa" type="number" placeholder="320"></div>
      <div class="form-group"><label class="form-label">Basım Yılı</label><input class="form-input" id="f-yil" placeholder="2024"></div>
    </div>
    <div class="form-grid">
      <div class="form-group"><label class="form-label">Stok</label><input class="form-input" id="f-stok" type="number" placeholder="5"></div>
      <div class="form-group"><label class="form-label">Raf No</label><input class="form-input" id="f-rafNo" placeholder="BG-1"></div>
    </div>`;
    showModal();
}

async function openEditBook(id) {
    editTargetId = id;
    document.getElementById('modalTitle').textContent = 'Kitap Düzenle';
    document.getElementById('modalBody').innerHTML = 'Yükleniyor...';
    showModal();

    try {
        const b = await apiFetch(`Books/${id}`);
        document.getElementById('modalBody').innerHTML = `
      <div class="form-group"><label class="form-label">Kitap Adı</label><input class="form-input" id="f-kitapAd" value="${esc(b.kitapAd)}"></div>
      <div class="form-grid">
        <div class="form-group"><label class="form-label">Stok Adedi</label><input class="form-input" id="f-stok" type="number" value="5"></div>
        <div class="form-group"><label class="form-label">Raf No</label><input class="form-input" id="f-rafNo" value="${esc(b.rafNo)}"></div>
      </div>`;
    } catch (err) {
        showToast('Kitap yüklenemedi', 'error');
        closeModal();
    }
}

async function saveBook() {
    const btn = document.getElementById('modalSaveBtn');
    btn.disabled = true; btn.textContent = 'Kaydediliyor...';

    try {
        if (editTargetId) {
            // Güncelleme
            const body = { kitapAd: val('kitapAd'), stokAdeti: num('stok'), rafNo: val('rafNo'), yazarID: 0, turID: 0, sayfaSayisi: 0, basimYili: '' };
            await apiPut(`Books/${editTargetId}`, body);
            showToast('Kitap güncellendi', 'success');
        } else {
            // Ekleme
            const body = { kitapAd: val('kitapAd'), yazarID: num('yazarID'), turID: num('turID'), sayfaSayisi: num('sayfa'), basimYili: val('yil'), stokAdeti: num('stok'), rafNo: val('rafNo') };
            await apiPost('Books', body);
            showToast('Kitap eklendi', 'success');
        }
        closeModal();
        loadBooks();
    } catch (err) {
        showToast('Hata: ' + err.message, 'error');
    } finally {
        btn.disabled = false; btn.textContent = 'Kaydet';
    }
}

async function deleteBook(id, name) {
    if (!confirm(`"${name}" silinsin mi?`)) return;
    try {
        await apiDelete(`Books/${id}`);
        showToast('Kitap silindi', 'success');
        loadBooks();
    } catch (err) { showToast('Hata: ' + err.message, 'error'); }
}