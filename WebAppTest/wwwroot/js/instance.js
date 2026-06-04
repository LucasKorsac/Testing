document.addEventListener("DOMContentLoaded", function () {

    if (!window.chartData) {
        console.error("Нет данных для графика");
        return;
    }

    const chartTypeSelect = document.getElementById("chartType");

    if (!chartTypeSelect) {
        console.error("Элемент chartType не найден");
        return;
    }

    let currentChart = null;

    function renderChart() {
        const labels = window.chartData.labels;
        const values = window.chartData.values;

        if (!labels || !values || labels.length === 0) {
            console.warn("Нет данных для отображения");
            return;
        }

        const canvas = document.getElementById("metricsChart");
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
                    label: "Количество значений",
                    data: values,
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgb(54, 162, 235)',
                    borderWidth: 2,
                    borderRadius: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: { position: 'top' },
                    title: { display: true, text: 'Распределение метрик' }
                },
                scales: chartTypeSelect.value !== 'pie' && chartTypeSelect.value !== 'doughnut' ? {
                    y: { beginAtZero: true }
                } : {}
            }
        });
    }

    setTimeout(renderChart, 100);
    chartTypeSelect.addEventListener("change", renderChart);
});