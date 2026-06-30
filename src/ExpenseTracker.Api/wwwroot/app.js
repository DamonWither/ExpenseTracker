const api = '/api/expenses';
const categories = [
  ['Food', 'Еда'], ['Transport', 'Транспорт'], ['Housing', 'Жилье'],
  ['Entertainment', 'Развлечения'], ['Health', 'Здоровье'], ['Other', 'Другое']
];
const money = new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' });
let chart;

const $ = id => document.getElementById(id);

function fillCategories() {
  $('category').innerHTML = categories.map(([v,t]) => `<option value="${v}">${t}</option>`).join('');
  $('filterCategory').innerHTML = '<option value="">Все категории</option>' + categories.map(([v,t]) => `<option value="${v}">${t}</option>`).join('');
  $('date').valueAsDate = new Date();
}

function categoryTitle(value) { return categories.find(([v]) => v === value)?.[1] ?? value; }

function queryString(includeCategory = true) {
  const params = new URLSearchParams();
  if ($('dateFrom').value) params.set('dateFrom', $('dateFrom').value);
  if ($('dateTo').value) params.set('dateTo', $('dateTo').value);
  if (includeCategory && $('filterCategory').value) params.set('category', $('filterCategory').value);
  if (includeCategory && $('search').value.trim()) params.set('search', $('search').value.trim());
  return params.toString();
}

async function request(url, options) {
  const res = await fetch(url, options);
  if (!res.ok) {
    const body = await res.json().catch(() => ({ error: 'Ошибка запроса' }));
    throw new Error(body.error ?? 'Ошибка запроса');
  }
  return res.status === 204 ? null : res.json();
}

async function loadExpenses() {
  const data = await request(`${api}?${queryString(true)}`);
  $('itemsCount').textContent = `Найдено: ${data.totalCount}`;
  $('expensesBody').innerHTML = data.items.map(e => `
    <tr>
      <td>${e.date}</td>
      <td>${escapeHtml(e.description)}</td>
      <td>${categoryTitle(e.category)}</td>
      <td class="amount">${money.format(e.amount)}</td>
      <td><button class="danger" onclick="deleteExpense('${e.id}')">Удалить</button></td>
    </tr>`).join('') || '<tr><td colspan="5">Расходов пока нет</td></tr>';
}

async function loadSummary() {
  const summary = await request(`${api}/summary?${queryString(false)}`);
  $('totalAmount').textContent = money.format(summary.totalAmount);
  $('categorySummary').innerHTML = summary.byCategory.map(x => `<li><span>${categoryTitle(x.category)}</span><strong>${money.format(x.totalAmount)}</strong></li>`).join('') || '<li>Нет данных</li>';
  renderChart(summary.byDay);
}

function renderChart(days) {
  const ctx = $('expensesChart');
  if (chart) chart.destroy();
  chart = new Chart(ctx, {
    type: 'bar',
    data: { labels: days.map(x => x.date), datasets: [{ label: 'Траты по дням', data: days.map(x => x.totalAmount) }] },
    options: { responsive: true, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } }
  });
}

async function refresh() {
  $('formMessage').textContent = '';
  try { await Promise.all([loadExpenses(), loadSummary()]); }
  catch (e) { showMessage(e.message, false); }
}

async function deleteExpense(id) {
  if (!confirm('Удалить расход?')) return;
  try { await request(`${api}/${id}`, { method: 'DELETE' }); await refresh(); }
  catch (e) { showMessage(e.message, false); }
}

function showMessage(text, ok) {
  const el = $('formMessage'); el.textContent = text; el.className = ok ? 'message ok' : 'message';
}

function escapeHtml(text) {
  return text.replace(/[&<>'"]/g, ch => ({ '&':'&amp;', '<':'&lt;', '>':'&gt;', "'":'&#039;', '"':'&quot;' }[ch]));
}

$('expenseForm').addEventListener('submit', async (e) => {
  e.preventDefault();
  const payload = {
    description: $('description').value.trim(),
    amount: Number($('amount').value),
    date: $('date').value,
    category: $('category').value
  };
  try {
    await request(api, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
    e.target.reset(); $('date').valueAsDate = new Date(); showMessage('Расход добавлен', true); await refresh();
  } catch (err) { showMessage(err.message, false); }
});

$('applyFilters').addEventListener('click', refresh);
$('resetFilters').addEventListener('click', () => { $('dateFrom').value=''; $('dateTo').value=''; $('filterCategory').value=''; $('search').value=''; refresh(); });

fillCategories();
refresh();
