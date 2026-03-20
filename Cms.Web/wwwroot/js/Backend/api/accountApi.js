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

export async function getAccountSummaries(payload) {
    return await fetch(`/api/accounts${buildQueryString(payload)}`, {
        method: 'GET'
    }).then(r => r.json());
}

export async function createAccount(payload) {
    return await fetch('/api/accounts', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function updateAccount(username, payload) {
    return await fetch(`/api/accounts/${encodeURIComponent(username)}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function resetPassword(username, payload) {
    return await fetch(`/api/accounts/${encodeURIComponent(username)}/password`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    }).then(r => r.json());
}

export async function deleteAccount(username) {
    return await fetch(`/api/accounts/${encodeURIComponent(username)}`, {
        method: 'DELETE'
    }).then(r => r.json());
}

export async function logout() {
    const res = await fetch('/api/sessions/current', {
        method: 'DELETE'
    });

    if (!res.ok) {
        throw new Error('Logout failed');
    }
}
