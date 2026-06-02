function getCanvas(id) {
    const canvas = document.getElementById(id);

    if (!canvas) {
        console.error(`Canvas ${id} не найден`);
        return null;
    }

    return canvas;
}

function getData(sourceName) {
    const data = window[sourceName];

    if (!data) {
        console.error(`${sourceName} не найден`);
        return null;
    }

    return data;
}