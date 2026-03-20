export async function getDepartmentsForSidebar() {
    return await fetch('/api/departments/sidebar', {
        method: 'GET'
    }).then(r => r.json());
}
