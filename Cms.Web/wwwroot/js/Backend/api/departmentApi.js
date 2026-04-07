export async function getDepartmentOptions() {
    return await fetch('/api/departments/options', {
        method: 'GET'
    }).then(response => response.json());
}

