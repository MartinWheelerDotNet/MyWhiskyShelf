import Keycloak, {type KeycloakInitOptions } from "keycloak-js";

const keycloak = new Keycloak({
    url: import.meta.env.VITE_KEYCLOAK_URL,
    realm: import.meta.env.VITE_KEYCLOAK_REALM,
    clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID
});

export const initOptions: KeycloakInitOptions = {
    onLoad: "check-sso",
    pkceMethod: "S256",
    checkLoginIframe: true,
    silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`
};

export default keycloak;