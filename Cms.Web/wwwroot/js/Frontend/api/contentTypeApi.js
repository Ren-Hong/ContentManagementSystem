function buildQueryString(payload) {
    const params = new URLSearchParams();

    Object.entries(payload || {}).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
            params.set(key, value);
        }
    });

    const query = params.toString();
    return query ? `?${query}` : '';
}

export async function getContentTypeLookups(payload) {
    return await fetch(`/api/content-types${buildQueryString(payload)}`, {
        method: 'GET'
    }).then(response => response.json());
}

export async function createContentType(payload) {
    return await fetch('/api/content-types', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(response => response.json());
}

export async function updateContentType(typeId, payload) {
    return await fetch(`/api/content-types/${typeId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(response => response.json());
}

export async function deleteContentType(typeId) {
    return await fetch(`/api/content-types/${typeId}`, {
        method: 'DELETE'
    }).then(response => response.json());
}
