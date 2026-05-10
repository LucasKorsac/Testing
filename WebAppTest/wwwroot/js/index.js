const ctx = document.getElementById("testsChart");

if (ctx && window.testsChartData) {

    new Chart(ctx, {
        type: "bar",

        data: {
            labels: window.testsChartData.labels,

            datasets: [
                {
                    label: "Количество вариантов",

                    data: window.testsChartData.values,

                    borderWidth: 2,

                    borderRadius: 12,

                    tension: 0.4
                }
            ]
        },

        options: {
            responsive: true,

            plugins: {
                legend: {
                    display: true
                }
            },

            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}