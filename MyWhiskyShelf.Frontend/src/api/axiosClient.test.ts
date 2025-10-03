import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";

const kc = vi.hoisted(() => ({
    instance: {
        init: vi.fn().mockResolvedValue(true),
        updateToken: vi.fn().mockResolvedValue(true),
        login: vi.fn().mockResolvedValue(undefined),
        token: "test.jwt.token",
        onTokenExpired: undefined as undefined | (() => void),
        authenticated: true,
    },
}));

vi.mock("keycloak-js", () => {
    const Ctor = vi.fn(function MockKeycloak(_config?: any) {
        return kc.instance;
    });
    return { default: Ctor };
});

vi.mock("../auth/keycloak", () => {
    const keycloak = kc.instance;
    return {
        default: keycloak,
        keycloak,
    };
});

const ax = vi.hoisted(() => ({
    state: {
        reqHandler: undefined as ((config: any) => any) | undefined,
        resFulfilled: undefined as ((resp: any) => any) | undefined,
        resRejected: undefined as ((err: any) => any) | undefined,
    },
}));

vi.mock("axios", () => {
    const { state } = ax;

    const create = vi.fn(() => ({
        interceptors: {
            request: {
                use: (onFulfilled: (config: any) => any) => {
                    state.reqHandler = onFulfilled;
                },
            },
            response: {
                use: (
                    onFulfilled?: (resp: any) => any,
                    onRejected?: (err: any) => any
                ) => {
                    state.resFulfilled = onFulfilled;
                    state.resRejected = onRejected;
                },
            },
        },
        get: vi.fn(),
        post: vi.fn(),
        put: vi.fn(),
        delete: vi.fn(),
    }));

    return { default: { create }, create };
});

import "./axiosClient";

function getInterceptors() {
    return ax.state;
}

const realLocation = window.location;

function makeMockLocalStorage() {
    let store = new Map<string, string>();
    return {
        getItem: vi.fn((k: string) => (store.has(k) ? store.get(k)! : null)),
        setItem: vi.fn((k: string, v: string) => void store.set(k, v)),
        removeItem: vi.fn((k: string) => void store.delete(k)),
        clear: vi.fn(() => void store.clear()),
    };
}

let mockStorage: ReturnType<typeof makeMockLocalStorage>;

beforeEach(() => {
    mockStorage = makeMockLocalStorage();
    // @ts-expect-error override for test
    globalThis.localStorage = mockStorage;

    delete (window as any).location;
    // @ts-expect-error minimal test double
    window.location = {
        ...realLocation,
        href: "",
        assign: vi.fn(),
        replace: vi.fn(),
    };
});

afterEach(() => {
    vi.clearAllMocks();
    delete (window as any).location;
    // @ts-expect-error restore
    window.location = realLocation;
});

describe("axiosClient interceptors", () => {
    it("adds Authorization header when a token exists in localStorage", async () => {
        const { reqHandler } = getInterceptors();
        expect(reqHandler).toBeTypeOf("function");

        const token = "test.jwt.token";
        localStorage.setItem("token", token);

        const initialConfig = { url: "/test", headers: {} as Record<string, string> };
        const result = await reqHandler!(initialConfig);

        const auth =
            (result.headers as Record<string, string>)["Authorization"] ??
            (result.headers as Record<string, string>)["authorization"];

        expect(auth).toBeTruthy();
        expect(String(auth)).toContain(token);
    });

    it("does not add Authorization header when no token is present", async () => {
        const { reqHandler } = getInterceptors();
        expect(reqHandler).toBeTypeOf("function");

        kc.instance.token = undefined as unknown as string;
        kc.instance.authenticated = false as unknown as boolean;

        const initialConfig = { url: "/test", headers: {} as Record<string, string> };
        const result = await reqHandler!(initialConfig);

        const hasAuth =
            "Authorization" in result.headers || "authorization" in result.headers;

        expect(hasAuth).toBe(false);
    });

    it("passes successful responses through unchanged", async () => {
        const { resFulfilled } = getInterceptors();
        const response = { data: { ok: true }, status: 200 };
        const pass = resFulfilled ?? ((r: any) => r);
        expect(await pass(response)).toBe(response);
    });

    it("on 401: triggers Keycloak login with redirectUri=current href", async () => {
        const { resRejected } = getInterceptors();
        expect(resRejected).toBeTypeOf("function");

        const currentHref = String((window.location as any).href ?? "");

        const error = {
            isAxiosError: true,
            response: { status: 401 },
            config: { url: "/protected", autoLoginOn401: true },
        };

        resRejected!(error);

        await Promise.resolve();

        expect(kc.instance.login).toHaveBeenCalledTimes(1);
        expect(kc.instance.login).toHaveBeenCalledWith({ redirectUri: currentHref });
    });

    it("non-401 errors are rethrown", async () => {
        const { resRejected } = getInterceptors();
        expect(resRejected).toBeTypeOf("function");

        const error = {
            isAxiosError: true,
            response: { status: 500 },
            config: { url: "/boom" },
            message: "Internal Server Error",
        };

        await expect(resRejected!(error)).rejects.toBe(error);
    });
});
