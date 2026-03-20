export async function getScopeLookups() {
    return await fetch('/api/scopes', {
        method: 'GET'
    }).then(response => response.json());
}
