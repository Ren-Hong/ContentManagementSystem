export async function getDepartmentLookups() {
    return await fetch('/api/departments', {
        method: 'GET'
    }).then(response => response.json());
}
