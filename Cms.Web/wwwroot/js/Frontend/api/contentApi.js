export async function createContent(typeId, payload) {
    return await fetch(`/api/content-types/${typeId}/contents`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(response => response.json());
}

export async function getContents(typeId) {
    return await fetch(`/api/content-types/${typeId}/contents`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    }).then(response => response.json());
}

export async function updateContent(contentId, payload) {
    return await fetch(`/api/contents/${contentId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(response => response.json());
}

export async function deleteContent(contentId) {
    return await fetch(`/api/contents/${contentId}`, {
        method: 'DELETE'
    }).then(response => response.json());
}
