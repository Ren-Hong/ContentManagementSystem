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

async function toApiJson(response) {
    const raw = await response.text();

    if (raw) {
        try {
            return JSON.parse(raw);
        } catch {
            return {
                success: false,
                errorCode: `HTTP_${response.status}`
            };
        }
    }

    return {
        success: response.ok,
        errorCode: response.ok ? undefined : `HTTP_${response.status}`
    };
}

export async function getContentTypeLookups(payload) {
    return await fetch(`/api/content-types${buildQueryString(payload)}`, {
        method: 'GET'
    }).then(toApiJson);
}

export async function createContentType(payload) {
    return await fetch('/api/content-types', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(toApiJson);
}

export async function updateContentType(typeId, payload) {
    return await fetch(`/api/content-types/${typeId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(toApiJson);
}

export async function deleteContentType(typeId) {
    return await fetch(`/api/content-types/${typeId}`, {
        method: 'DELETE'
    }).then(toApiJson);
}
