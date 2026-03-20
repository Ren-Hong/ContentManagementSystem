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

export async function getPermissionAssignments(payload) {
    return await fetch(`/api/permission-assignments${buildQueryString(payload)}`, {
        method: 'GET'
    }).then(response => response.json());
}

export async function createPermissionAssignment(payload) {
    return await fetch('/api/permission-assignments', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(response => response.json());
}
