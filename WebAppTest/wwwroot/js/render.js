document.addEventListener("DOMContentLoaded", () => {

    const charts = window.__charts;

    if (!charts || !Array.isArray(charts)) {
        console.warn("Нет графиков для отрисовки");
        return;
    }

    charts.forEach(renderChart);
});

function renderChart(chart) {

    const canvas = document.getElementById(chart.id);

    if (!canvas) {
        console.warn(`Canvas ${chart.id} не найден`);
        return;
    }

    const ctx = canvas.getContext("2d");

    new Chart(ctx, {
        type: chart.type || "line",

        data: {
            labels: chart.labels || [],
            datasets: [{
                label: chart.title || "",
                data: chart.values || [],
                borderWidth: 2
            }]
        },

        options: {
            responsive: true,
            maintainAspectRatio: false,

            plugins: {
                title: {
                    display: true,
                    text: chart.title || ""
                }
            },

            scales: chart.type === "bar" || chart.type === "line"
                ? {
                    y: { beginAtZero: true }
                }
                : {}
        }
    });
}