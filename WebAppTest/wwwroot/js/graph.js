export function renderUserResults(data) {
    const container = document.getElementById("result");

    container.innerHTML = "";

    const list = document.createElement("ul");

    Object.entries(data).forEach(([test, variant]) => {
        const li = document.createElement("li");
        li.textContent = `${test} → ${variant}`;
        list.appendChild(li);
    });

    container.appendChild(list);
}