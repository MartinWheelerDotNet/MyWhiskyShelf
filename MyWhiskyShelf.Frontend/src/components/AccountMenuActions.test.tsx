import { describe, it, expect, vi, beforeEach, type Mock } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";

const useUserMock = useUser as unknown as Mock;

vi.mock("./header/LoadingAction", () => ({
    default: () => <div data-testid="loading-action">Loading…</div>,
}));

vi.mock("./header/SignInAction", () => ({
    default: (props: any) => (
        <button data-testid="sign-in-action" onClick={props.onLogin}>Sign in</button>
    ),
}));

vi.mock("./header/AccountMenu", () => ({
    default: (props: any) => (
        <div data-testid="account-menu">
            <span data-testid="account-username">{props.username}</span>
            <span data-testid="account-initials">{props.initials}</span>
            <button data-testid="logout-button" onClick={props.onLogout}>Logout</button>
        </div>
    ),
}));

vi.mock("@/hooks/useUser", () => ({ useUser: vi.fn() }));
// @ts-ignore

import { useUser } from "@/hooks/useUser";
import AccountMenuActions from "./AccountMenuActions";

describe("AccountMenuActions", () => {
    beforeEach(() => vi.clearAllMocks());

    it("renders LoadingAction when not initialized", () => {
        useUserMock.mockReturnValue({ initialized: false, authenticated: false });
        render(<AccountMenuActions />);
        expect(screen.getByTestId("loading-action")).toBeInTheDocument();
    });

    it("renders AccountMenu when authenticated with correct username and initials", () => {
        const logout = vi.fn();
        useUserMock.mockReturnValue({
            initialized: true, authenticated: true,
            username: "whiskylover", email: "slainte@example.com",
            firstName: "jane", lastName: "doe", logout,
        });
        render(<AccountMenuActions />);
        expect(screen.getByTestId("account-menu")).toBeInTheDocument();
        expect(screen.getByTestId("account-username").textContent).toBe("whiskylover");
        expect(screen.getByTestId("account-initials").textContent).toBe("JD");
        fireEvent.click(screen.getByTestId("logout-button"));
        expect(logout).toHaveBeenCalledTimes(1);
    });

    it("falls back to email when username missing", () => {
        useUserMock.mockReturnValue({
            initialized: true, authenticated: true,
            username: undefined, email: "fallback@example.com",
            firstName: "John", lastName: "Smith", logout: vi.fn(),
        });
        render(<AccountMenuActions />);
        expect(screen.getByTestId("account-username").textContent).toBe("fallback@example.com");
        expect(screen.getByTestId("account-initials").textContent).toBe("JS");
    });

    it('falls back to "Account" when username and email missing', () => {
        useUserMock.mockReturnValue({
            initialized: true, authenticated: true,
            username: undefined, email: undefined,
            firstName: "Islay", lastName: "Peat", logout: vi.fn(),
        });
        render(<AccountMenuActions />);
        expect(screen.getByTestId("account-username").textContent).toBe("Account");
        expect(screen.getByTestId("account-initials").textContent).toBe("IP");
    });

    it('uses default initials "WS" when both names missing', () => {
        useUserMock.mockReturnValue({
            initialized: true, authenticated: true,
            username: undefined, email: undefined,
            firstName: undefined, lastName: undefined, logout: vi.fn(),
        });
        render(<AccountMenuActions />);
        expect(screen.getByTestId("account-initials").textContent).toBe("WS");
    });

    it("renders SignInAction when not authenticated and triggers login", () => {
        const login = vi.fn();
        useUserMock.mockReturnValue({ initialized: true, authenticated: false, login });
        render(<AccountMenuActions />);
        fireEvent.click(screen.getByTestId("sign-in-action"));
        expect(login).toHaveBeenCalledTimes(1);
    });
});
