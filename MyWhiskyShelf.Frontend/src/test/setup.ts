import { vi } from "vitest";
import '@testing-library/jest-dom/vitest';

export const TEST_KEYCLOAK = {
    login: vi.fn().mockResolvedValue(undefined),
    token: "test.jwt.token" as string | undefined,
    authenticated: true as boolean | undefined,
};

export function makeMockLocalStorage() {
    let store = new Map<string, string>();
    return {
        getItem: vi.fn((k: string) => (store.has(k) ? store.get(k)! : null)),
        setItem: vi.fn((k: string, v: string) => void store.set(k, v)),
        removeItem: vi.fn((k: string) => void store.delete(k)),
        clear: vi.fn(() => void store.clear()),
    };
}

export function stubLocation() {
    const realLocation = window.location;
    delete (window as any).location;
    // @ts-expect-error minimal test double
    window.location = {
        ...realLocation,
        href: "http://localhost/",
        assign: vi.fn(),
        replace: vi.fn(),
    };
    return () => {
        delete (window as any).location;
        // @ts-expect-error restore
        window.location = realLocation;
    };
}

Object.assign(globalThis, {
    TEST_KEYCLOAK,
    makeMockLocalStorage,
    stubLocation,
});
