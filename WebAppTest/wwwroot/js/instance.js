document.addEventListener("DOMContentLoaded", function () {

    const table = document.getElementById("metricsTable");

    if (!table) {
        console.warn("Таблица metricsTable не найдена");
        return;
    }

    // получаем заголовки (пропускаем первый столбец "Установка")
    const headers = Array.from(table.querySelectorAll("thead th"))
        .slice(1)
        .map(function (th) { return th.innerText; });

    const metricSelect = document.getElementById("metricSelect");
    const chartTypeSelect = document.getElementById("chartType");
    const canvas = document.getElementById("metricsChart");
    const updateBtn = document.getElementById("updateChartBtn");

    if (!metricSelect || !chartTypeSelect || !canvas) {
        console.error("Элементы управления не найдены");
        return;
    }

    const ctx = canvas.getContext("2d");
    let currentChart = null;

    // заполняем выпадающий список метрик
    headers.forEach(function (header) {
        const option = document.createElement("option");
        option.value = header;
        option.textContent = header;
        metricSelect.appendChild(option);
    });

    function renderChart() {
        const selectedMetric = metricSelect.value;
        const chartType = chartTypeSelect.value;

        const rows = Array.from(table.querySelectorAll("tbody tr"));

        const labels = [];
        const values = [];

        // находим индекс выбранной метрики
        const metricIndex = headers.indexOf(selectedMetric);

        rows.forEach(function (row) {
            const cols = row.querySelectorAll("td");
            if (cols.length > metricIndex + 1) {
                const label = cols[0]?.innerText || "Unknown";
                const valueCell = cols[metricIndex + 1];
                const value = parseFloat(valueCell?.dataset?.value || valueCell?.innerText || 0);

                labels.push(label);
                values.push(value);
            }
        });

        if (currentChart) {
            currentChart.destroy();
        }

        currentChart = new Chart(ctx, {
            type: chartType,
            data: {
                labels: labels,
                datasets: [{
                    label: selectedMetric,
                    data: values,
                    backgroundColor: 'rgba(54, 162, 235, 0.5)',
                    borderColor: 'rgb(54, 162, 235)',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: (chartType === 'pie' || chartType === 'doughnut') ? {} : {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    // рендерим начальный график
    if (headers.length > 0) {
        renderChart();
    }

    // обработчики событий
    if (updateBtn) {
        updateBtn.addEventListener("click", renderChart);
    }
    chartTypeSelect.addEventListener("change", renderChart);
    metricSelect.addEventListener("change", renderChart);
});