<script>
    async function exportData(format) {
        var testId = getTestId();

    if (!testId) {
        alert('ID теста не найден');
    return;
        }

    var url = format === 'excel'
    ? `/api/export/excel/${testId}`
    : `/api/export/txt/${testId}`;

    try {
            const response = await fetch(url);

    if (!response.ok) {
                const error = await response.text();
    alert(`Ошибка: ${error}`);
    return;
            }

    // Получаем имя файла из заголовка Content-Disposition
    const contentDisposition = response.headers.get('Content-Disposition');
    let filename = `export_${format}.${format === 'excel' ? 'xlsx' : 'txt'}`;
    if (contentDisposition) {
                const match = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
    if (match && match[1]) {
        filename = match[1].replace(/['"]/g, '');
                }
            }

    // Скачиваем файл
    const blob = await response.blob();
    const link = document.createElement('a');
    const objectUrl = URL.createObjectURL(blob);
    link.href = objectUrl;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(objectUrl);

        } catch (error) {
        console.error('Ошибка:', error);
    alert('Произошла ошибка при экспорте');
        }
    }

    function getTestId() {
        // Получаем ID из URL
        const path = window.location.pathname;
    const match = path.match(/\/Tests\/Details\/([a-fA-F0-9]{24})/);
    return match ? match[1] : null;
    }
</script>