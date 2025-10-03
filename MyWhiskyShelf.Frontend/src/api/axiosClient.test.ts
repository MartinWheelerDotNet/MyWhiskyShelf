import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";
import { installInterceptors } from "./axiosClient";

// helpers from global setup
declare const TEST_KEYCLOAK: {
    login: ReturnType<typeof vi.fn>;
    token?: string;
    authenticated?: boolean;
};
declare function makeMockLocalStorage(): Storage;
declare function stubLocation(): () => void;

let restoreLocation: () => void;

beforeEach(() => {
    const ls = makeMockLocalStorage();
    globalThis.localStorage = ls;

    restoreLocation = stubLocation();

    TEST_KEYCLOAK.login.mockClear();
    TEST_KEYCLOAK.token = "test.jwt.token";
    TEST_KEYCLOAK.authenticated = true;
});

afterEach(() => {
    restoreLocation();
});

describe("axiosClient interceptors (with DI)", () => {
    it("adds Authorization header when a token exists in localStorage", async () => {
        const token = "from-local-storage";
        localStorage.setItem("token", token);

        const { request } = installInterceptors({
            getToken: () => TEST_KEYCLOAK.token,
            login: (uri) => TEST_KEYCLOAK.login({ redirectUri: uri }),
        });

        const result = await request({ url: "/a-test-url", headers: {} } as any);
        const auth = (result.headers as any).Authorization ?? (result.headers as any).authorization;

        expect(auth).toBe(`Bearer ${token}`);
    });

    it("does not add Authorization header when no token is present anywhere", async () => {
        TEST_KEYCLOAK.token = undefined;
        TEST_KEYCLOAK.authenticated = false;

        const { request } = installInterceptors({
            getToken: () => TEST_KEYCLOAK.token,
            login: (uri) => TEST_KEYCLOAK.login({ redirectUri: uri }),
        });

        const result = await request({ url: "/a-test-url", headers: {} } as any);
        const hasAuth = "Authorization" in result.headers || "authorization" in result.headers;
        expect(hasAuth).toBe(false);
    });

    it("passes successful responses through unchanged (identity fulfilled)", async () => {
        const { response } = installInterceptors({
            getToken: () => TEST_KEYCLOAK.token,
            login: (uri) => TEST_KEYCLOAK.login({ redirectUri: uri }),
        });

        const pass = response.onFulfilled ?? ((r: any) => r);
        const responseObj = { data: { ok: true }, status: 200 };
        expect(await pass(responseObj)).toBe(responseObj);
    });

    it("on 401 & autoLogin: triggers Keycloak login with redirectUri=current href", async () => {
        const { response } = installInterceptors({
            getToken: () => TEST_KEYCLOAK.token,
            login: (uri) => TEST_KEYCLOAK.login({ redirectUri: uri }),
            shouldAutoLogin401: (err) => Boolean((err as any)?.config?.autoLoginOn401),
        });

        const currentHref = String((window.location as any).href);

        response.onRejected({
            isAxiosError: true,
            response: { status: 401 },
            config: { url: "/a-protected-url", autoLoginOn401: true },
        } as any);

        await Promise.resolve();

        expect(TEST_KEYCLOAK.login).toHaveBeenCalledTimes(1);
        expect(TEST_KEYCLOAK.login).toHaveBeenCalledWith({ redirectUri: currentHref });
    });

    it("non-401 errors are rethrown", async () => {
        const { response } = installInterceptors({
            getToken: () => TEST_KEYCLOAK.token,
            login: (uri) => TEST_KEYCLOAK.login({ redirectUri: uri }),
        });

        const error = {
            isAxiosError: true,
            response: { status: 500 },
            config: { url: "/a-test-url" },
            message: "Internal Server Error",
        } as any;

        await expect(response.onRejected(error)).rejects.toBe(error);
    });
});