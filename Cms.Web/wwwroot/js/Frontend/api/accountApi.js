export async function logout() {
    const response = await fetch('/api/sessions/current', {
        method: 'DELETE'
    });

    if (!response.ok) {
        throw new Error('Logout failed');
    }
}
