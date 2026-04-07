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
