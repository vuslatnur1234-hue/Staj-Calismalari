// TEMA 
(function () {
  const saved = localStorage.getItem('kutuphane-tema') || 'light';
  if (saved === 'dark') document.documentElement.setAttribute('data-theme', 'dark');
})();

function temaDegistir() {
  const html = document.documentElement;
  const btn  = document.getElementById('themeBtn');
  const dark = html.getAttribute('data-theme') === 'dark';
  if (dark) {
    html.removeAttribute('data-theme');
    localStorage.setItem('kutuphane-tema', 'light');
    if (btn) btn.textContent = '🌙';
  } else {
    html.setAttribute('data-theme', 'dark');
    localStorage.setItem('kutuphane-tema', 'dark');
    if (btn) btn.textContent = '☀️';
  }
}

document.addEventListener('DOMContentLoaded', () => {
  const btn = document.getElementById('themeBtn');
  if (btn) {
    const dark = document.documentElement.getAttribute('data-theme') === 'dark';
    btn.textContent = dark ? '☀️' : '🌙';
  }

  // Aktif nav link
  const path = location.pathname.split('/').pop() || 'index.html';
  document.querySelectorAll('nav a').forEach(a => {
    const href = a.getAttribute('href');
    if (href === path || (path === '' && href === 'index.html')) a.classList.add('active');
  });
});

// TOAST
function toast(msg, type = 'success') {
  const container = document.getElementById('toast-container');
  if (!container) return;
  const el = document.createElement('div');
  el.className = `toast ${type}`;
  el.innerHTML = `<span>${type === 'success' ? '✓' : '✕'}</span><span>${msg}</span>`;
  container.appendChild(el);
  setTimeout(() => {
    el.classList.add('out');
    setTimeout(() => el.remove(), 350);
  }, 3200);
}

// MODAL
function openModal(id) {
  const m = document.getElementById(id);
  if (m) { m.classList.add('open'); document.body.style.overflow = 'hidden'; }
}
function closeModal(id) {
  const m = document.getElementById(id);
  if (m) { m.classList.remove('open'); document.body.style.overflow = ''; }
}


document.addEventListener('click', e => {
  if (e.target.classList.contains('modal-overlay')) closeModal(e.target.id);
});

// YARDIMCILAR
function formatTarih(str) {
  if (!str || str.length < 10) return '—';
  return str.substring(0, 10);
}