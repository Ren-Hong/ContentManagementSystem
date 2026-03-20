export async function getRoleOptions() {
    return await fetch('/api/roles/options', {
        method: 'GET'
    }).then(r => r.json());
}

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

export async function getRoleSummaries(payload) {
    return await fetch(`/api/roles${buildQueryString(payload)}`, {
        method: 'GET'
    }).then(r => r.json());
}

export async function createRole(payload) {
    return await fetch('/api/roles', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function updateRole(roleName, payload) {
    return await fetch(`/api/roles/${encodeURIComponent(roleName)}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function deleteRole(roleName) {
    return await fetch(`/api/roles/${encodeURIComponent(roleName)}`, {
        method: 'DELETE'
    }).then(r => r.json());
}
