// Базовый URL API
const apiBase = "https://localhost:5001/api";

// Загрузка списка записей выбранной модели
async function loadList() {
    const model = document.getElementById("modelSelect").value;
    const response = await fetch(`${apiBase}/${model}`);
    if (!response.ok) return alert("Ошибка загрузки данных");

    const data = await response.json();

    const tableHeader = document.getElementById("tableHeader");
    const tableBody = document.getElementById("tableBody");
    tableHeader.innerHTML = "";
    tableBody.innerHTML = "";

    if (data.length === 0) return;

    // Заголовки таблицы
    Object.keys(data[0]).forEach(key => {
        const th = document.createElement("th");
        th.innerText = key;
        tableHeader.appendChild(th);
    });

    // Строки таблицы
    data.forEach(item => {
        const tr = document.createElement("tr");
        Object.values(item).forEach(value => {
            const td = document.createElement("td");
            td.innerText = typeof value === 'object' && value !== null ? JSON.stringify(value) : value;
            tr.appendChild(td);
        });
        tableBody.appendChild(tr);
    });
}

// Создание новой записи
async function createRecord() {
    const model = document.getElementById("modelSelect").value;
    const data = JSON.parse(document.getElementById("recordData").value);

    const response = await fetch(`${apiBase}/${model}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    if (!response.ok) return alert("Ошибка создания");
    alert("Создано успешно");
    loadList();
}

// Обновление записи по Id
async function updateRecord() {
    const model = document.getElementById("modelSelect").value;
    const id = document.getElementById("recordId").value;
    const data = JSON.parse(document.getElementById("recordData").value);

    const response = await fetch(`${apiBase}/${model}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    if (!response.ok) return alert("Ошибка обновления");
    alert("Обновлено успешно");
    loadList();
}

// Удаление записи по Id
async function deleteRecord() {
    const model = document.getElementById("modelSelect").value;
    const id = document.getElementById("recordId").value;

    const response = await fetch(`${apiBase}/${model}/${id}`, { method: "DELETE" });
    if (!response.ok) return alert("Ошибка удаления");

    alert("Удалено успешно");
    loadList();
}

// Инициализация: при изменении модели автоматически загружается список
document.getElementById("modelSelect").addEventListener("change", loadList);

// Автозагрузка первой модели при старте
window.addEventListener("DOMContentLoaded", loadList);