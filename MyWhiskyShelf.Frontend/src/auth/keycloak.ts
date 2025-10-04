import Keycloak, {type KeycloakConfig, type KeycloakInitOptions } from "keycloak-js";

export const KC_INIT_OPTIONS: KeycloakInitOptions = {
    onLoad: "check-sso",
    pkceMethod: "S256",
    silentCheckSsoRedirectUri:
        typeof window !== "undefined"
            ? `${window.location.origin}/silent-check-sso.html`
            : undefined,
};

function createKeycloak(): Keycloak {
    const url = import.meta.env?.VITE_KEYCLOAK_URL as string | undefined;
    const realm = import.meta.env?.VITE_KEYCLOAK_REALM as string | undefined;
    const clientId = import.meta.env?.VITE_KEYCLOAK_CLIENT_ID as string | undefined;

    if (!url || !realm || !clientId) {
        const stub = {
            authenticated: false,
            token: undefined as string | undefined,
            init: async (_opts?: KeycloakInitOptions) => true,
            login: async (_opts?: { redirectUri?: string }) => {},
            logout: async (_opts?: { redirectUri?: string }) => {},
            updateToken: async (_minValidity?: number) => true,
            onTokenExpired: undefined as undefined | (() => void),
        };
        return stub as unknown as Keycloak;
    }

    const config: KeycloakConfig = { url, realm, clientId };
    return new Keycloak(config);
}

export const keycloak: Keycloak = createKeycloak();
export default keycloak;
