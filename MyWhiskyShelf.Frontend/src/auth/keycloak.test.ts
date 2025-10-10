import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";


const KeycloakCtor = vi.fn();

vi.mock("keycloak-js", () => ({
    default: KeycloakCtor,
}));

const importModule = async () => await import("./keycloak");

describe("auth/keycloak", () => {
    beforeEach(() => {
        vi.resetModules();
        vi.resetAllMocks();
        vi.unstubAllEnvs();
    });

    afterEach(() => {
        vi.unstubAllEnvs();
    });

    it("exports KC_INIT_OPTIONS with the expected defaults", async () => {
        const origin =
            typeof globalThis !== "undefined" && (globalThis as any)?.location?.origin
                ? (globalThis as any).location.origin
                : "http://localhost";

        const { KC_INIT_OPTIONS } = await importModule();

        expect(KC_INIT_OPTIONS.onLoad).toBe("check-sso");
        expect(KC_INIT_OPTIONS.pkceMethod).toBe("S256");
        
        expect(KC_INIT_OPTIONS.silentCheckSsoRedirectUri).toBe(
            `${origin}/silent-check-sso.html`
        );
    });

    it("creates a real Keycloak instance when all env vars are present", async () => {
        vi.stubEnv("VITE_KEYCLOAK_URL", "https://kc.example.com/auth");
        vi.stubEnv("VITE_KEYCLOAK_REALM", "my-realm");
        vi.stubEnv("VITE_KEYCLOAK_CLIENT_ID", "my-client");

        const realInstance = { __type: "Keycloak", init: vi.fn() } as any;
        KeycloakCtor.mockReturnValue(realInstance);

        const { keycloak } = await importModule();

        expect(KeycloakCtor).toHaveBeenCalledTimes(1);
        expect(KeycloakCtor).toHaveBeenCalledWith({
            url: "https://kc.example.com/auth",
            realm: "my-realm",
            clientId: "my-client",
        });

        expect(keycloak).toBe(realInstance);
    });

    it("returns a stubbed Keycloak object when env vars are missing", async () => {
        vi.stubEnv("VITE_KEYCLOAK_URL", "");
        vi.stubEnv("VITE_KEYCLOAK_REALM", "");
        vi.stubEnv("VITE_KEYCLOAK_CLIENT_ID", "");

        const { keycloak } = await importModule();

        expect(KeycloakCtor).not.toHaveBeenCalled();

        const k = keycloak as any;
        expect(k.authenticated).toBe(false);
        expect(k.token).toBeUndefined();
        expect(typeof k.init).toBe("function");
        expect(typeof k.login).toBe("function");
        expect(typeof k.logout).toBe("function");
        expect(typeof k.updateToken).toBe("function");
        expect(k.onTokenExpired).toBeUndefined();

        await expect(k.init()).resolves.toBe(true);
        await expect(k.login()).resolves.toBeUndefined();
        await expect(k.logout()).resolves.toBeUndefined();
        await expect(k.updateToken(5)).resolves.toBe(true);
    });
});