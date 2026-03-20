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

export async function getPermissionAssignmentSummaries(payload) {
    return await fetch(`/api/permission-assignments${buildQueryString(payload)}`, {
        method: 'GET'
    }).then(r => r.json());
}

export async function createPermissionAssignment(payload) {
    return await fetch('/api/permission-assignments', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}
