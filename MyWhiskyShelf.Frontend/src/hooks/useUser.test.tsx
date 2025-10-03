import { describe, it, expect, vi } from "vitest";
import { renderHook, waitFor } from "@testing-library/react";

vi.mock("@react-keycloak/web", () => {
    const fakeTokenParsed = {
        given_name: "Glen",
        family_name: "Matlock",
        preferred_username: "GlenMatlock",
        email: "glen.matlock@example.com",
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
            given_name: "Glen",
            family_name: "Matlock",
            email: "GlenMatlock@example.com",
            preferred_username: "GlenMatlcok",
        }),
    };

    return {
        useKeycloak: () => ({ keycloak, initialized: true }),
    };
});

import { useUser } from "./useUser";

describe("useUser", () => {
    it("returns merged user fields from token + userinfo", async () => {
        const { result } = renderHook(() => useUser());
        
        await waitFor(() => {
            expect(result.current.initialized).toBe(true);
            expect(result.current.authenticated).toBe(true);
            expect(result.current.firstName).toBe("Glen");
            expect(result.current.lastName).toBe("Matlock");
            expect(result.current.email).toBe("glen.matlock@example.com");
            expect(result.current.username).toBe("GlenMatlock");
            expect(result.current.sub).toBe("sub-123");
        });
    });
});