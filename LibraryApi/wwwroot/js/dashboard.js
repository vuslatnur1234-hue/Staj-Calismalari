document.addEventListener('DOMContentLoaded', () => {
    loadDashboard();
});

async function loadDashboard() {
    try {
        const [books, members, loans] = await Promise.all([
            apiFetch('Books'),
            apiFetch('Uyeler'),
            apiFetch('OduncIslemleri'),
        ]);

        document.getElementById('stat-books').textContent = books.length;
        document.getElementById('stat-members').textContent = members.length;

        const active = loans.filter(l => l.durum !== 'Teslim Edildi');
        const returned = loans.filter(l => l.durum === 'Teslim Edildi');

        document.getElementById('stat-loans').textContent = active.length;
        document.getElementById('stat-returned').textContent = returned.length;

        // Son 5 kitap
        const recentBooks = [...books].slice(-5).reverse();
        document.getElementById('recentBooks').innerHTML = recentBooks.length
            ? recentBooks.map(b => `<div class="mini-item"><span class="mini-item-name">${esc(b.kitapAd)}</span><span class="mini-item-meta">${esc(b.yazarAd)}</span></div>`).join('')
            : '<div class="mini-item"><span class="mini-item-meta">Kayıt yok</span></div>';

        // Son 5 ödünç
        const recentLoans = [...loans].slice(-5).reverse();
        document.getElementById('recentLoans').innerHTML = recentLoans.length
            ? recentLoans.map(l => `<div class="mini-item"><span class="mini-item-name">${esc(l.kitapAd)}</span><span class="mini-item-meta">${esc(l.uyeAdSoyad)}</span></div>`).join('')
            : '<div class="mini-item"><span class="mini-item-meta">Kayıt yok</span></div>';

    } catch (err) {
        showToast('Dashboard verileri alınamadı: ' + err.message, 'error');
    }
}