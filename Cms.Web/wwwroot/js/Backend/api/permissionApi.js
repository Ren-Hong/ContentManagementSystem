export async function getPermissionLookups() {
    return await fetch('/api/permissions', {
        method: 'GET'
    }).then(response => response.json());
}
