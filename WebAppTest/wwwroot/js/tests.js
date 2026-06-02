document.addEventListener("DOMContentLoaded", function () {

    if (!window.variantData) {
        console.error("Нет данных для графика");
        return;
    }

    const chartTypeSelect = document.getElementById("chartType");
    const chartMetricSelect = document.getElementById("chartMetric");

    if (!chartTypeSelect || !chartMetricSelect) {
        console.error("Элементы управления не найдены");
        return;
    }

    let currentChart = null;

    function getMetricData() {
        switch (chartMetricSelect.value) {
            case "metrics":
                return window.variantData.metrics || [];
            default:
                return window.variantData.installs || [];
        }
    }

    function getMetricLabel() {
        switch (chartMetricSelect.value) {
            case "metrics":
                return "Средние метрики";
            default:
                return "Количество установок";
        }
    }

    function renderChart() {
        const labels = window.variantData.labels;
        const values = getMetricData();

        if (!labels || !values || labels.length === 0) {
            console.warn("Нет данных для графика");
            return;
        }

        const canvas = document.getElementById("variantChart");
        if (!canvas) return;

        const ctx = canvas.getContext('2d');

        if (currentChart) {
            currentChart.destroy();
        }

        currentChart = new Chart(ctx, {
            type: chartTypeSelect.value,
            data: {
                labels: labels,
                datasets: [{
                    label: getMetricLabel(),
                    data: values,
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgb(54, 162, 235)',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { position: 'top' },
                    title: { display: true, text: 'Аналитика вариантов' }
                },
                scales: chartTypeSelect.value !== 'pie' && chartTypeSelect.value !== 'doughnut' ? {
                    y: { beginAtZero: true }
                } : {}
            }
        });
    }

    renderChart();
    chartTypeSelect.addEventListener("change", renderChart);
    chartMetricSelect.addEventListener("change", renderChart);

    // Фильтр таблицы
    const variantFilter = document.getElementById("variantFilter");
    if (variantFilter) {
        variantFilter.addEventListener("change", function () {
            const selectedValue = this.value;
            const rows = document.querySelectorAll(".result-row");
            rows.forEach(row => {
                const variant = row.dataset.variant;
                row.style.display = (selectedValue === "all" || variant === selectedValue) ? "" : "none";
            });
        });
    }
});