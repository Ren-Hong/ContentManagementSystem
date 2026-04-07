export async function getPermissionOptions() {
    return await fetch('/api/permissions/options', {
        method: 'GET'
    }).then(response => response.json());
}

