export async function getContentFields(typeId) {
    return await fetch(`/api/content-types/${typeId}/fields`, {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    }).then(response => response.json());
}
