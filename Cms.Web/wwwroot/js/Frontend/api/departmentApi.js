export async function getAccessibleDepartments() {
    return await fetch('/api/departments/sidebar', {
        method: 'GET'
    }).then(response => response.json());
}
