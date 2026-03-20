export async function getScopeOptions() {
    return await fetch('/api/scopes', {
        method: 'GET'
    }).then(r => r.json());
}
