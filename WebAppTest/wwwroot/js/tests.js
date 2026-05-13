document.addEventListener("DOMContentLoaded", () => {

    const canvas =
        document.getElementById("variantChart");

    if (!canvas) {
        console.error("variantChart не найден");
        return;
    }

    if (!window.variantChartData) {
        console.error("Нет данных графика");
        return;
    }

    const labels =
        window.variantChartData.labels || [];

    const values =
        window.variantChartData.values || [];

    if (labels.length === 0 || values.length === 0) {
        console.error("Пустые данные графика");
        return;
    }

    const ctx = canvas.getContext("2d");

    new Chart(ctx, {

        type: "bar",

        data: {

            labels: labels,

            datasets: [
                {
                    label: "Количество пользователей",

                    data: values,

                    borderWidth: 1
                }
            ]
        },

        options: {

            responsive: true,

            maintainAspectRatio: false,

            plugins: {

                legend: {
                    display: true
                },

                title: {
                    display: true,
                    text: "Распределение пользователей по вариантам"
                }
            },

            scales: {

                y: {
                    beginAtZero: true
                }
            }
        }
    });

});