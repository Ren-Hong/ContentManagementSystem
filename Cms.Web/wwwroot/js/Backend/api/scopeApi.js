export async function getScopeOptions() {
    return await fetch('/api/scopes/options', {
        method: 'GET'
    }).then(response => response.json());
}

