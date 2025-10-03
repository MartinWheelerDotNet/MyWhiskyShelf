import { describe, it, expect, vi } from "vitest";
import { renderHook, waitFor } from "@testing-library/react";

describe("useUser", () => {
    it("returns merged user fields from token + userinfo", async () => {
        // 1️⃣ mock BEFORE importing the hook
        vi.doMock("@react-keycloak/web", () => {
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

            return {
                useKeycloak: () => ({ keycloak, initialized: true }),
            };
        });

        // 2️⃣ fresh import AFTER the mock
        const { useUser } = await import("./useUser");

        // 3️⃣ now run the hook
        const { result } = renderHook(() => useUser());

        // immediate token-derived values
        expect(result.current.initialized).toBe(true);
        expect(result.current.authenticated).toBe(true);
        expect(result.current.firstName).toBe("Grace");
        expect(result.current.lastName).toBe("Hopper");
        expect(result.current.email).toBe("grace@example.com");
        expect(result.current.username).toBe("grace");
        expect(result.current.sub).toBe("sub-123");

        // after loadUserInfo resolves
        await waitFor(() => {
            expect(result.current.firstName).toBe("Grace");
        });
    });
});