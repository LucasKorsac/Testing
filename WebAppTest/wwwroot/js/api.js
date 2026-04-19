// Базовый URL твоего backend API (ASP.NET / другой сервер)
const apiBase = "https://localhost:5001/api";


// Универсальная функция для HTTP запросов (GET, POST, PUT, DELETE)
async function apiRequest(url, options = {}) {

    // Выполнение запроса через fetch
    const response = await fetch(url, {

        // Стандартные заголовки (JSON по умолчанию)
        headers: {
            "Content-Type": "application/json",

            // Если переданы дополнительные заголовки
            ...(options.headers || {})
        },

        // Остальные настройки
        ...options
    });

    // Если сервер вернул ошибку
    if (!response.ok)
        throw new Error(`HTTP Error: ${response.status}`);

    // Чтение ответа как текст
    const text = await response.text();

    // Если ответ не пустой — парс JSON, иначе - возврат null
    return text ? JSON.parse(text) : null;
}


// Генерирует URL для API запросов
function getUrl(model, id = "") {

    // Если передан id - обращение к конкретному объекту
    return id
        ? `${apiBase}/${model}/${id}`

        // Если id нет - работа со списком
        : `${apiBase}/${model}`;
}