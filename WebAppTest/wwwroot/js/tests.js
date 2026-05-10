function submitAction(mode) {
    const form = document.getElementById("actionForm");
    document.getElementById("actionMode").value = mode;
    form.submit();
}

function editTest() {
    submitAction("edit");
}

function stopTest() {
    if (confirm("Остановить тест?")) {
        submitAction("stop");
    }
}

function resumeTest() {
    if (confirm("Возобновить тест?")) {
        submitAction("resume");
    }
}

function deleteTest() {
    if (confirm("Удалить тест?")) {
        submitAction("delete");
    }
}