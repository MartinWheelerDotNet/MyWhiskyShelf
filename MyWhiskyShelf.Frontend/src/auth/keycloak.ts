import Keycloak, {type KeycloakInitOptions} from "keycloak-js";

export const keycloak = new Keycloak({
    url: import.meta.env.VITE_KEYCLOAK_URL,
    realm: import.meta.env.VITE_KEYCLOAK_REALM,
    clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID
});

export const KC_INIT_OPTIONS = Object.freeze({
    onLoad: "check-sso",
    pkceMethod: "S256",
    checkLoginIframe: false,
    scope: "openid profile email",
});

let _initPromise: Promise<boolean> | null = null;
const _origInit = keycloak.init.bind(keycloak);

(keycloak as any).init = (opts?: KeycloakInitOptions) => {
    if (_initPromise) return _initPromise;
    _initPromise = _origInit(opts);
    return _initPromise;
};