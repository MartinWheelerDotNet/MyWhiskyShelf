import { describe, it, expect, beforeEach, vi } from "vitest";

const keycloakMock = {
    token: undefined as string | undefined,
    updateToken: vi.fn<(minValidity?: number) => Promise<boolean>>(),
};

vi.mock("@/auth/keycloak", () => ({
    default: keycloakMock,
}));

const importAdapter = async () => await import("./keycloakAdapter");

describe("keycloakAdapter (AuthProvider)", () => {
    beforeEach(() => {
        vi.resetModules();
        vi.resetAllMocks();
        keycloakMock.token = undefined;
        keycloakMock.updateToken.mockReset();
    });

    it("getToken returns the current keycloak.token or undefined", async () => {
        const { keycloakAuthProvider } = await importAdapter();

        expect(keycloakAuthProvider.getToken()).toBeUndefined();

        keycloakMock.token = "abc.def.ghi";
        expect(keycloakAuthProvider.getToken()).toBe("abc.def.ghi");
    });

    it("refreshToken calls keycloak.updateToken with default minValidity=10 and returns true when resolved true", async () => {
        keycloakMock.token = "t0";
        keycloakMock.updateToken.mockResolvedValueOnce(true);

        const { keycloakAuthProvider } = await importAdapter();

        await expect(keycloakAuthProvider.refreshToken()).resolves.toBe(true);
        expect(keycloakMock.updateToken).toHaveBeenCalledTimes(1);
        expect(keycloakMock.updateToken).toHaveBeenCalledWith(10);
    });

    it("refreshToken returns true when updateToken resolves false (still valid, not refreshed)", async () => {
        keycloakMock.token = "t0";
        keycloakMock.updateToken.mockResolvedValueOnce(false);

        const { keycloakAuthProvider } = await importAdapter();

        await expect(keycloakAuthProvider.refreshToken(30)).resolves.toBe(true);
        expect(keycloakMock.updateToken).toHaveBeenCalledWith(30);
    });

    it("refreshToken returns false when updateToken throws (e.g., expired / not authenticated)", async () => {
        keycloakMock.token = "t0";
        keycloakMock.updateToken.mockRejectedValueOnce(new Error("boom"));

        const { keycloakAuthProvider } = await importAdapter();

        await expect(keycloakAuthProvider.refreshToken()).resolves.toBe(false);
        expect(keycloakMock.updateToken).toHaveBeenCalledWith(10);
    });

    it("loginRedirect navigates to /login via location.assign", async () => {
        const originalLocation = globalThis.location;

        const mockAssign = vi.fn();
        Object.defineProperty(globalThis, "location", {
            configurable: true,
            value: {
                ...originalLocation,
                assign: mockAssign,
            },
        });

        const { keycloakAuthProvider } = await importAdapter();
        keycloakAuthProvider.loginRedirect();

        expect(mockAssign).toHaveBeenCalledWith("/login");
        
        Object.defineProperty(globalThis, "location", {
            configurable: true,
            value: originalLocation,
        });
    });

    it("clearStorage clears localStorage and swallows errors", async () => {
        const originalLocalStorage = globalThis.localStorage;

        const clear = vi.fn()
            .mockImplementationOnce(() => { throw new Error("error"); })
            .mockImplementationOnce(() => {});

        vi.stubGlobal("localStorage", { clear } as unknown as Storage);

        const { keycloakAuthProvider } = await importAdapter();

        expect(() => keycloakAuthProvider.clearStorage()).not.toThrow();
        expect(() => keycloakAuthProvider.clearStorage()).not.toThrow();

        expect(clear).toHaveBeenCalledTimes(2);

        vi.stubGlobal("localStorage", originalLocalStorage);
    });
});