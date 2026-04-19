// Базовый URL backend API
const apiBase = "https://localhost:5001/api";


// После загрузки страницы сразу подгружаем список A/B тестов
window.addEventListener("DOMContentLoaded", async () => {
    await loadTests();
});


// Загрузка тестов

// Получает список A/B тестов с backend и заполняет dropdown
async function loadTests() {
    try {
        // Запрос к API за всеми A/B тестами
        const tests = await fetchData(`${apiBase}/AbTest`);

        // Получение select элемент
        const select = document.getElementById("testSelect");

        // Очистка старых значений
        select.innerHTML = "";

        // Добавление каждого теста как option в select
        tests.forEach(t => {
            const opt = document.createElement("option");

            // Поддержка разных регистров (Id / id)
            opt.value = t.id || t.Id;

            // Отображение имени теста
            opt.textContent = t.name || t.Name;

            select.appendChild(opt);
        });

    } catch (e) {
        console.error(e);
        alert("Ошибка загрузки тестов");
    }
}


// Загрузка данных для графика
async function loadChart() {
    try {
        // Получение выбранного теста
        const testId = document.getElementById("testSelect").value;

        // Запрос результатов по тесту
        // ожидается структура: [{ variant: "A", count: 120 }, { variant: "B", count: 150 }]
        const data = await fetchData(`${apiBase}/ab/results/${testId}`);

        // Названия вариантов (оси X)
        const labels = data.map(x => x.variant || x.Variant);

        // Значения (оси Y)
        const values = data.map(x => x.count || x.Count);

        // Построение графика
        renderChart(labels, values);

    } catch (e) {
        console.error(e);
        alert("Ошибка загрузки графика");
    }
}

// Отображение графика
let chartInstance = null; // Хранение текущего графика

function renderChart(labels, values) {
    const ctx = document.getElementById("chart").getContext("2d");

    // Если график уже есть — минусуем ему жизнь
    if (chartInstance) {
        chartInstance.destroy();
    }

    // Создание нового графика через Chart.js
    chartInstance = new Chart(ctx, {
        type: "bar", // Тип графика: столбчатый

        data: {
            labels: labels, // Подписи оси X
            datasets: [{
                label: "Конверсии", // Название набора данных
                data: values        // Значения оси Y
            }]
        },

        options: {
            responsive: true, // Адаптивный размер
            plugins: {
                legend: { display: true } // Показывать легенду
            }
        }
    });
}


// API

// GET 
async function fetchData(url) {
    const res = await fetch(url);

    // Исключение в случае ошибки
    if (!res.ok) throw new Error("HTTP error " + res.status);

    // Чтение ответа как текста
    const text = await res.text();

    // Парс JSON или возврат пустого массива
    return text ? JSON.parse(text) : [];
}