import keycloak from "../auth/keycloak";

export async function apiFetch(input: RequestInfo, init: RequestInit = {}) {
    // refresh if expiring in next 30s
    await keycloak.updateToken(30).catch(() => keycloak.login());
    const token = keycloak.token;

    return fetch(input, {
        ...init,
        headers: {
            ...(init.headers || {}),
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
        },
        credentials: "include",
    });
}