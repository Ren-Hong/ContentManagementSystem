export async function getDepartmentOptions() {
    return await fetch('/api/departments', {
        method: 'GET'
    }).then(r => r.json());
}
