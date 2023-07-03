function showToast(text, type) {
    Toastify({
        text,
        duration: 3000,
        gravity: "top",
        position: "right",
        className: `toast ${type ?? ""}`
    }).showToast();
}

async function makeRequest(url, processMessage, successMessage, method = "get", body = undefined, reload = false, button = undefined, buttons = undefined) {
    if (button?.classList.contains("off"))
        return;
    
    if (button !== undefined && buttons === undefined)
        buttons = [button];
    
    buttons?.forEach(btn => btn.classList.add("off"));
    
    showToast(processMessage);
    
    const settings = { method, credentials: "same-origin" };
    if (body !== undefined) {
        if (body instanceof FormData)
            settings.body = body;
        else {
            settings.body = JSON.stringify(body);
            
            settings.headers = { "Content-Type": "application/json" };
        }
    }
    
    try {
        const response = await fetch(url, settings);

        if (response.ok) {
            showToast(successMessage, "success");

            if (reload)
                location.reload();
        } else
            showToast("An error occurred", "error");
    } catch (e) {
        showToast("An error occurred", "error");
    } finally {
        if (!reload)
            buttons?.forEach(btn => btn.classList.remove("off"));
    }
}