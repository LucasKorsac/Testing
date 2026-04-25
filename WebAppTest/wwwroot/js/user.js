import { runABTest } from "./api.js";
import { renderUserResults } from "./graph.js";

document.addEventListener("DOMContentLoaded", () => {
    const btn = document.getElementById("runBtn");

    btn.addEventListener("click", async () => {
        const appId = document.getElementById("appId").value;

        try {
            const result = await runABTest(appId);

            console.log("AB result:", result);

            renderUserResults(result);
        } catch (err) {
            console.error(err);
            alert("Ошибка запуска A/B теста");
        }
    });
});