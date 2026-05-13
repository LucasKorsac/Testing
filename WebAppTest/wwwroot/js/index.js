document.addEventListener("DOMContentLoaded", () => {

    const canvas = document.getElementById("testsChart");

    if (!canvas) {
        console.error("Canvas testsChart не найден");
        return;
    }

    if (!window.testsChartData) {
        console.error("testsChartData не загружен");
        return;
    }

    const labels = window.testsChartData.labels || [];
    const values = window.testsChartData.values || [];

    const ctx = canvas.getContext("2d");

    new Chart(ctx, {
        type: "line",

        data: {
            labels: labels,

            datasets: [
                {
                    label: "Активность тестов",

                    data: values,

                    fill: true,

                    borderColor: "rgba(59, 130, 246, 1)",
                    backgroundColor: "rgba(59, 130, 246, 0.15)",

                    tension: 0.4,   // плавность линии
                    pointRadius: 5,
                    pointHoverRadius: 7,
                    pointBackgroundColor: "rgba(59, 130, 246, 1)"
                }
            ]
        },

        options: {
            responsive: true,
            maintainAspectRatio: false,

            interaction: {
                mode: "index",
                intersect: false
            },

            plugins: {
                title: {
                    display: true,
                    text: "Динамика A/B тестов"
                },

                legend: {
                    display: true
                }
            },

            scales: {
                x: {
                    grid: {
                        display: false
                    }
                },

                y: {
                    beginAtZero: true
                }
            }
        }
    });
});