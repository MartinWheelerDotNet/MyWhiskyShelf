import { describe, it, expect, beforeEach, vi } from "vitest";

// Spies to confirm interceptor registration
const requestUseSpy = vi.fn();
const responseUseSpy = vi.fn();

// We capture the installed handlers so we can invoke them directly
const requestHandlerRef: { onFulfilled?: (cfg: any) => any } = {};
const responseHandlerRef: {
    onRejected?: (err: any) => Promise<any>,
    onFulfilled?: (resp: any) => any
} = {};

// Minimal AxiosInstance double
const mockAxiosInstance = {
    interceptors: {
        request: {
            use: (...args: any[]) => {
                requestUseSpy(...args);
                requestHandlerRef.onFulfilled = args[0];
            },
        },
        response: {
            use: (...args: any[]) => {
                responseUseSpy(...args);
                responseHandlerRef.onFulfilled = args[0];
                responseHandlerRef.onRejected = args[1];
            },
        },
    },
    request: vi.fn((cfg: any) => Promise.resolve({ data: "retried-ok", config: cfg })),
} as any;

// Mock axios.create to return our instance
vi.mock("axios", () => {
    const create = vi.fn(() => mockAxiosInstance);
    return {
        default: { create },
        create,
    };
});

// Mock the Keycloak auth adapter
const authProvider = {
    getToken: vi.fn<() => string | undefined>(),
    refreshToken: vi.fn<(minValiditySeconds?: number) => Promise<boolean>>(),
    clearStorage: vi.fn<() => void>(),
    loginRedirect: vi.fn<() => void>(),
};

vi.mock("@/auth/keycloakAdapter", () => ({
    keycloakAuthProvider: authProvider,
}));

beforeEach(async () => {
    vi.resetModules();

    // reset spies & handlers
    requestUseSpy.mockClear();
    responseUseSpy.mockClear();
    mockAxiosInstance.request.mockClear();

    authProvider.getToken.mockReset();
    authProvider.refreshToken.mockReset();
    authProvider.clearStorage.mockReset();
    authProvider.loginRedirect.mockReset();

    // Import the module under test for side-effects (installs interceptors)
    await import("./axiosClient"); // NOTE: no unused variable assignment
});

describe("axiosClient interceptors", () => {
    it("adds Authorization header when auth.getToken() returns a token", async () => {
        authProvider.getToken.mockReturnValue("abc.def.ghi");

        const cfg = await requestHandlerRef.onFulfilled!({ headers: {} });

        expect(requestUseSpy).toHaveBeenCalledTimes(1);
        expect((cfg.headers as any).Authorization).toBe("Bearer abc.def.ghi");
    });

    it("does not add Authorization header when auth.getToken() is empty", async () => {
        authProvider.getToken.mockReturnValue(undefined);

        const cfg = await requestHandlerRef.onFulfilled!({});

        expect((cfg.headers as any)?.Authorization).toBeUndefined();
    });

    it("on 401 triggers refresh; if refreshed, retries the original request once", async () => {
        authProvider.refreshToken.mockResolvedValue(true);

        const error = {
            isAxiosError: true,
            response: { status: 401 },
            config: { url: "/needs-auth" },
            message: "Unauthorized",
        } as any;

        const result = await responseHandlerRef.onRejected!(error);

        expect(authProvider.refreshToken).toHaveBeenCalledWith(10);
        expect(mockAxiosInstance.request).toHaveBeenCalledTimes(1);
        expect(result).toEqual({ data: "retried-ok", config: { url: "/needs-auth", _retry: true } });
    });

    it("on 403 refresh fails → clears storage, redirects, and rejects", async () => {
        authProvider.refreshToken.mockResolvedValue(false);

        const error = {
            isAxiosError: true,
            response: { status: 403 },
            config: { url: "/forbidden" },
            message: "Forbidden",
        } as any;

        await expect(responseHandlerRef.onRejected!(error)).rejects.toBe(error);
        expect(authProvider.refreshToken).toHaveBeenCalledWith(10);
        expect(authProvider.clearStorage).toHaveBeenCalledTimes(1);
        expect(authProvider.loginRedirect).toHaveBeenCalledTimes(1);
        expect(mockAxiosInstance.request).not.toHaveBeenCalled();
    });

    it("for non-401/403 errors, rejects without side effects", async () => {
        const error = {
            isAxiosError: true,
            response: { status: 500 },
            config: { url: "/server-error" },
            message: "Internal Server Error",
        } as any;

        await expect(responseHandlerRef.onRejected!(error)).rejects.toBe(error);
        expect(authProvider.refreshToken).not.toHaveBeenCalled();
        expect(authProvider.clearStorage).not.toHaveBeenCalled();
        expect(authProvider.loginRedirect).not.toHaveBeenCalled();
    });
});
