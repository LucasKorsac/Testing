// Текущая страница пагинации
let page = 1;

// Кол-во элементов на одной странице
const pageSize = 10;


// Инициализация

// Срабатывает после загрузки HTML страницы
window.addEventListener("DOMContentLoaded", () => {

    // Заполнение списка моделей
    initModels();

    // Отррисовка формы создания для выбранной модели
    renderForm();

    // Изменение выбранной модели
    document.getElementById("modelSelect")
        .addEventListener("change", () => {

            // Сброс страницы при смене модели
            page = 1;

            // Перестройка формы под новую модель
            renderForm();

            // Загрузка данных для новой модели
            loadList();
        });

    // Первичная загрузка таблицы
    loadList();
});


// Список моделей

// Заполнение списком моделей из schemas
function initModels() {
    const select = document.getElementById("modelSelect");

    // schemas — это объект с описанием всех моделей
    Object.keys(schemas).forEach(m => {

        // создание option для select
        const opt = document.createElement("option");
        opt.value = m;
        opt.textContent = m;

        // добавление в список
        select.appendChild(opt);
    });
}

// Получение выбранной модели из select
function getModel() {
    return document.getElementById("modelSelect").value;
}


// Форма создания

// Генерация ввод полей на основе схемы модели
function renderForm() {
    const model = getModel();
    const schema = schemas[model];

    const container = document.getElementById("createForm");

    // Очистка форму
    container.innerHTML = "";

    // Создание ввода под каждое поле модели
    Object.keys(schema).forEach(field => {

        const input = document.createElement("input");
        input.id = "create_" + field;
        input.placeholder = field;

        container.appendChild(input);
    });
}

// Загрузка данных

// Загрузка списка данных с сервера
async function loadList() {
    try {
        const model = getModel();

        // Фильтр поиска
        const filter = document.getElementById("filterInput").value || "";

        // Запрос к API с пагинацией и фильтром
        const data = await apiRequest(
            `${getUrl(model)}?page=${page}&pageSize=${pageSize}&filter=${filter}`
        );

        // Отображение таблицы
        renderTable(data);

        // Обновление страницы в UI
        document.getElementById("pageLabel").textContent = page;

    } catch (e) {
        console.error(e);
        alert("Ошибка загрузки");
    }
}


// Отображение

// Рендер HTML-таблицы из данных API
function renderTable(data) {

    const header = document.getElementById("tableHeader");
    const body = document.getElementById("tableBody");

    // Очистка таблицы
    header.innerHTML = "";
    body.innerHTML = "";

    // Если данных нет — выход
    if (!data || data.length === 0) return;

    // Названия колонок
    const keys = Object.keys(data[0]);

    // Создание заголовков таблицы
    keys.forEach(k => {
        const th = document.createElement("th");
        th.textContent = k;
        header.appendChild(th);
    });

    // Создание строк таблицы
    data.forEach(row => {
        const tr = document.createElement("tr");

        keys.forEach(key => {
            const td = document.createElement("td");

            // Вставка значений поля
            td.textContent = row[key];

            tr.appendChild(td);
        });

        body.appendChild(tr);
    });
}


// Создание

// Отправка POST-запроса на создание новой записи
async function createItem() {
    try {
        const model = getModel();
        const schema = schemas[model];

        const data = {};

        // Сбор данных из input формы
        Object.keys(schema).forEach(field => {
            data[field] = document.getElementById("create_" + field).value;
        });

        // Отправка на сервер
        await apiRequest(getUrl(model), {
            method: "POST",
            body: JSON.stringify(data)
        });

        alert("Создано");

        // Обновление списока после создания
        loadList();

    } catch (e) {
        console.error(e);
        alert("Ошибка создания");
    }
}


// Переход на следующую страницу
function nextPage() {
    page++;
    loadList();
}

// Переход на предыдущую страницу
function prevPage() {
    if (page > 1) page--;
    loadList();
}