export async function runABTest(appId) {
    const response = await fetch("/api/ab/run", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ appId })
    });

    if (!response.ok) {
        throw new Error("Ошибка API runABTest");
    }

    return await response.json();
}