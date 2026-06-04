document.addEventListener("DOMContentLoaded", function () {

    if (!window.chartData) {
        console.error("Нет данных для графика");
        return;
    }

    const chartTypeSelect = document.getElementById("mainChartType");
    const metricSelect = document.getElementById("metricSelect");

    if (!chartTypeSelect) {
        console.error("Элемент mainChartType не найден");
        return;
    }

    let currentChart = null;

    // Функция для форматирования даты
    function formatDate(dateString) {
        if (!dateString) return "Нет данных";
        const date = new Date(dateString);
        return date.toLocaleDateString('ru-RU', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    }

    // Функция для получения метрики
    function getMetricData() {
        if (!metricSelect) return window.chartData.values || [];

        switch (metricSelect.value) {
            case "variants":
                return window.chartData.variantsValues || [];
            case "applications":
                return window.chartData.applicationsValues || [];
            case "tests":
            default:
                return window.chartData.values || [];
        }
    }

    // Функция для получения названия метрики
    function getMetricLabel() {
        if (!metricSelect) return "Количество тестов";

        switch (metricSelect.value) {
            case "variants":
                return "Количество вариантов";
            case "applications":
                return "Количество приложений";
            case "tests":
            default:
                return "Количество тестов";
        }
    }

    // Функция для получения заголовка графика
    function getChartTitle() {
        if (!metricSelect) return "Динамика A/B тестов";

        switch (metricSelect.value) {
            case "variants":
                return "Динамика создания вариантов";
            case "applications":
                return "Динамика добавления приложений";
            case "tests":
            default:
                return "Динамика создания A/B тестов";
        }
    }

    function renderChart() {
        const labels = window.chartData.dates || window.chartData.labels || [];
        const values = getMetricData();

        if (!labels || !values || labels.length === 0) {
            console.warn("Нет данных для отображения");
            return;
        }

        const canvas = document.getElementById("testsChart");
        if (!canvas) return;

        const ctx = canvas.getContext('2d');

        if (currentChart) {
            currentChart.destroy();
        }

        // Форматируем подписи для оси X (показываем только дату)
        const formattedLabels = labels.map(label => {
            // Если это дата в формате ISO или другой
            if (label.includes('-') && (label.length === 10 || label.includes('T'))) {
                return formatDate(label);
            }
            return label;
        });

        const chartConfig = {
            type: chartTypeSelect.value,
            data: {
                labels: formattedLabels,
                datasets: [{
                    label: getMetricLabel(),
                    data: values,
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgb(54, 162, 235)',
                    borderWidth: 2,
                    borderRadius: 8,
                    tension: 0.3, // для сглаживания линий
                    fill: chartTypeSelect.value === 'line' ? false : undefined
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        position: 'top',
                        labels: {
                            font: { size: 12 }
                        }
                    },
                    title: {
                        display: true,
                        text: getChartTitle(),
                        font: { size: 16, weight: 'bold' }
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                let label = context.dataset.label || '';
                                if (label) {
                                    label += ': ';
                                }
                                label += context.raw;
                                return label;
                            }
                        }
                    }
                },
                scales: chartTypeSelect.value !== 'pie' && chartTypeSelect.value !== 'doughnut' ? {
                    x: {
                        title: {
                            display: true,
                            text: 'Дата создания',
                            font: { size: 12 }
                        },
                        ticks: {
                            maxRotation: 45,
                            minRotation: 45,
                            autoSkip: true,
                            maxTicksLimit: 10
                        }
                    },
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: getMetricLabel(),
                            font: { size: 12 }
                        },
                        ticks: {
                            stepSize: 1,
                            precision: 0
                        }
                    }
                } : {}
            }
        };

        currentChart = new Chart(ctx, chartConfig);
    }

    // Загружаем данные с сервера при выборе метрики
    async function fetchAndRender() {
        const metric = metricSelect ? metricSelect.value : 'tests';

        try {
            // Получаем данные с сервера
            const response = await fetch(`/api/charts/data?metric=${metric}`);
            if (response.ok) {
                const data = await response.json();
                window.chartData = {
                    dates: data.dates,
                    values: data.testsValues,
                    variantsValues: data.variantsValues,
                    applicationsValues: data.applicationsValues
                };
            }
        } catch (error) {
            console.error("Ошибка загрузки данных:", error);
        }

        renderChart();
    }

    // Первоначальный рендеринг
    setTimeout(renderChart, 100);

    // Обработчики событий
    chartTypeSelect.addEventListener("change", renderChart);

    if (metricSelect) {
        metricSelect.addEventListener("change", function () {
            renderChart();
        });
    }
});