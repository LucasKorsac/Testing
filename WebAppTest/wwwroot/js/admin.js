document.addEventListener("DOMContentLoaded", () => {
    const btn = document.getElementById("refreshBtn");

    btn.addEventListener("click", () => {
        alert("Admin panel ready (backend уже отвечает за логику)");
    });
});