// useUser.test.ts
import { describe, it, expect, vi } from "vitest";
import { renderHook, waitFor } from "@testing-library/react";

// 👇 mock first, before importing useUser
vi.mock("@react-keycloak/web", () => {
    const fakeTokenParsed = {
        given_name: "Grace",
        family_name: "Hopper",
        preferred_username: "grace",
        email: "grace@example.com",
        sub: "sub-123",
    };

    const keycloak = {
        authenticated: true,
        token: "token-abc",
        idTokenParsed: fakeTokenParsed,
        tokenParsed: fakeTokenParsed,
        login: vi.fn(),
        logout: vi.fn(),
        register: vi.fn(),
        loadUserInfo: vi.fn().mockResolvedValue({
            given_name: "Grace",
            family_name: "Hopper",
            email: "grace@example.com",
            preferred_username: "grace",
        }),
    };

    // 👇 this replaces the real hook so ReactKeycloakProvider is never needed
    return {
        useKeycloak: () => ({ keycloak, initialized: true }),
    };
});

// 👇 import AFTER the mock
import { useUser } from "./useUser";

describe("useUser", () => {
    it("returns merged user fields from token + userinfo", async () => {
        const { result } = renderHook(() => useUser());

        expect(result.current.initialized).toBe(true);
        expect(result.current.authenticated).toBe(true);
        expect(result.current.firstName).toBe("Grace");
        expect(result.current.lastName).toBe("Hopper");
        expect(result.current.email).toBe("grace@example.com");
        expect(result.current.username).toBe("grace");
        expect(result.current.sub).toBe("sub-123");

        await waitFor(() => {
            expect(result.current.firstName).toBe("Grace");
        });
    });
});