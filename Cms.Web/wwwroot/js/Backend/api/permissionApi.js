export async function getPermissionOptions() {
    return await fetch('/api/permissions', {
        method: 'GET'
    }).then(r => r.json());
}
