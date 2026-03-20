export async function logout() {
    const res = await fetch('/api/sessions/current', {
        method: 'DELETE'
    });

    if (!res.ok) {
        throw new Error('Logout failed');
    }
}
