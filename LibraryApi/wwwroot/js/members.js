let editTargetId = null;

document.addEventListener('DOMContentLoaded', () => {
    loadMembers();
    document.getElementById('searchInput').addEventListener('input', e => {
        const q = e.target.value.toLowerCase();
        document.querySelectorAll('#membersBody tr').forEach(row => {
            row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
        });
    });
});

async function loadMembers() {
    const tbody = document.getElementById('membersBody');
    try {
        const members = await apiFetch('Uyeler');
        if (!members.length) {
            tbody.innerHTML = '<tr><td colspan="7" class="loading-cell">Kayıt bulunamadı</td></tr>';
            return;
        }
        tbody.innerHTML = members.map(u => `
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
            <button class="btn-icon btn-loan" title="Ödünç Ver" onclick="goToLoan(${u.uyeID})">⊕</button>
          </td>
        </tr>`).join('');
    } catch (err) {
        tbody.innerHTML = `<tr><td colspan="7" class="loading-cell">Hata: ${esc(err.message)}</td></tr>`;
    }
}

function openAddMember() {
    editTargetId = null;
    document.getElementById('modalTitle').textContent = 'Yeni Üye Ekle';
    document.getElementById('modalBody').innerHTML = `
    <div class="form-grid">
      <div class="form-group"><label class="form-label">Ad</label><input class="form-input" id="f-ad" placeholder="Ad"></div>
      <div class="form-group"><label class="form-label">Soyad</label><input class="form-input" id="f-soyad" placeholder="Soyad"></div>
    </div>
    <div class="form-group"><label class="form-label">Telefon</label><input class="form-input" id="f-telefon" placeholder="05xx xxx xx xx"></div>
    <div class="form-group"><label class="form-label">E-posta</label><input class="form-input" id="f-email" type="email" placeholder="ornek@mail.com"></div>`;
    showModal();
}

async function openEditMember(id) {
    editTargetId = id;
    document.getElementById('modalTitle').textContent = 'Üye Düzenle';
    document.getElementById('modalBody').innerHTML = 'Yükleniyor...';
    showModal();

    try {
        const u = await apiFetch(`Uyeler/${id}`);
        document.getElementById('modalBody').innerHTML = `
      <div class="form-grid">
        <div class="form-group"><label class="form-label">Ad</label><input class="form-input" id="f-ad" value="${esc(u.ad)}"></div>
        <div class="form-group"><label class="form-label">Soyad</label><input class="form-input" id="f-soyad" value="${esc(u.soyad)}"></div>
      </div>
      <div class="form-group"><label class="form-label">Telefon</label><input class="form-input" id="f-telefon" value="${esc(u.telefon)}"></div>
      <div class="form-group"><label class="form-label">E-posta</label><input class="form-input" id="f-email" type="email" value="${esc(u.email)}"></div>`;
    } catch (err) {
        showToast('Üye yüklenemedi', 'error');
        closeModal();
    }
}

async function saveMember() {
    const btn = document.getElementById('modalSaveBtn');
    btn.disabled = true; btn.textContent = 'Kaydediliyor...';
    try {
        const body = { ad: val('ad'), soyad: val('soyad'), telefon: val('telefon'), email: val('email') };
        if (editTargetId) {
            await apiPut(`Uyeler/${editTargetId}`, body);
            showToast('Üye güncellendi', 'success');
        } else {
            await apiPost('Uyeler', body);
            showToast('Üye eklendi', 'success');
        }
        closeModal();
        loadMembers();
    } catch (err) {
        showToast('Hata: ' + err.message, 'error');
    } finally {
        btn.disabled = false; btn.textContent = 'Kaydet';
    }
}

async function deleteMember(id, name) {
    if (!confirm(`"${name}" silinsin mi?`)) return;
    try {
        await apiDelete(`Uyeler/${id}`);
        showToast('Üye silindi', 'success');
        loadMembers();
    } catch (err) { showToast('Hata: ' + err.message, 'error'); }
}

function goToLoan(uyeId) {
    // Ödünç sayfasına parametre ile yönlendir
    window.location.href = `loans.html?uyeId=${uyeId}`;
}