// @ts-ignore
import keycloak from "@/auth/keycloak";

export interface AuthProvider {
    getToken(): string | undefined;
    refreshToken(minValiditySeconds?: number): Promise<boolean>;
    loginRedirect(): void;
    clearStorage(): void;
}

export const keycloakAuthProvider: AuthProvider = {
    getToken() {
        return keycloak.token ?? undefined;
    },

    async refreshToken(minValiditySeconds = 10) {
        try {
            const ok = await keycloak.updateToken(minValiditySeconds);
            return ok || !!keycloak.token;
        } catch {
            return false;
        }
    },

    loginRedirect() {
        globalThis.location.assign("/login");
    },

    clearStorage() {
        try {
            globalThis.localStorage.clear();
        } catch {}
    },
};