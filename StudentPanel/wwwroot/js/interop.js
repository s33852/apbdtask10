// Copies the given text to the clipboard using the browser Clipboard API.
export async function copyToClipboard(text) {
    try {
        await navigator.clipboard.writeText(text);
        return true;
    } catch {
        return false;
    }
}

// Shows a native browser confirm dialog and returns true/false.
export function confirmDialog(message) {
    return confirm(message);
}

// Saves a value to localStorage.
export function saveToLocalStorage(key, value) {
    localStorage.setItem(key, value);
}

// Reads a value from localStorage (null if not present).
export function loadFromLocalStorage(key) {
    return localStorage.getItem(key);
}
