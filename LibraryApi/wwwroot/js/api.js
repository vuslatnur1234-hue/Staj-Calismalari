//API 
const API = 'https://localhost:55142/api';

const http = {
  get: (url) => fetch(url).then(r => { if (!r.ok) throw new Error(r.status); return r.json(); }),
  post: (url, body) => fetch(url, { method:'POST', headers:{'Content-Type':'application/json'}, body:JSON.stringify(body) })
            .then(r => { if (!r.ok) throw new Error(r.status); return r; }),
  put:  (url) => fetch(url, { method:'PUT' })
            .then(r => { if (!r.ok) throw new Error(r.status); return r; }),
  del:  (url) => fetch(url, { method:'DELETE' })
            .then(r => { if (!r.ok) throw new Error(r.status); return r; }),
};

const api = {
  // Kitaplar
  getBooks:   () => http.get(`${API}/books`),
  addBook:    (d) => http.post(`${API}/books`, d),
  deleteBook: (id) => http.del(`${API}/books/${id}`),
  updateBook: (id,d) => fetch(`${API}/books/${id}`, { method:'PUT', headers:{'Content-Type':'application/json'}, body:JSON.stringify(d) }).then(r=>{ if(!r.ok) throw new Error(r.status); return r; }),

  // Üyeler
  getUyeler:  () => http.get(`${API}/uyeler`),
  addUye:     (d) => http.post(`${API}/uyeler`, d),
  deleteUye:  (id) => http.del(`${API}/uyeler/${id}`),

  // Ödünç
  getOdunc:   () => http.get(`${API}/OduncIslemleri`),
  addOdunc:   (d) => http.post(`${API}/OduncIslemleri`, d),
  teslimAl:   (id) => http.put(`${API}/OduncIslemleri/${id}/teslim`),
};