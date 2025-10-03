import * as React from "react";
import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import SignInAction from "./SignInAction";

function renderWithTheme(ui: React.ReactNode) {
    return render(<ThemeProvider theme={createTheme()}>{ui}</ThemeProvider>);
}

const originalMatchMedia = window.matchMedia;

function setMatchMedia(matches: boolean) {
    window.matchMedia = vi.fn().mockImplementation((query: string) => ({
        matches,
        media: query,
        onchange: null,
        addListener: vi.fn(),
        removeListener: vi.fn(),
        addEventListener: vi.fn(),
        removeEventListener: vi.fn(),
        dispatchEvent: vi.fn(),
    }));
}

afterEach(() => {
    window.matchMedia = originalMatchMedia;
    vi.clearAllMocks();
});

describe("SignInAction (mobile ≤ sm)", () => {
    beforeEach(() => setMatchMedia(true));

    it("renders an IconButton with accessible name 'Sign in'", () => {
        const onLogin = vi.fn();
        renderWithTheme(<SignInAction onLogin={onLogin} />);
        const btn = screen.getByRole("button", { name: "Sign in" });
        expect(btn).toBeInTheDocument();
    });

    it("calls onLogin when clicked", async () => {
        const user = userEvent.setup();
        const onLogin = vi.fn();
        renderWithTheme(<SignInAction onLogin={onLogin} />);
        await user.click(screen.getByRole("button", { name: "Sign in" }));
        expect(onLogin).toHaveBeenCalledTimes(1);
    });
});

describe("SignInAction (desktop > sm)", () => {
    beforeEach(() => setMatchMedia(false));

    it("renders a text button 'Sign in / Register' and not the icon-only button", () => {
        const onLogin = vi.fn();
        renderWithTheme(<SignInAction onLogin={onLogin} />);

        const textBtn = screen.getByRole("button", { name: "Sign in / Register" });
        expect(textBtn).toBeInTheDocument();

        expect(screen.queryByRole("button", { name: "Sign in" })).toBeNull();
    });

    it("calls onLogin when the text button is clicked", async () => {
        const user = userEvent.setup();
        const onLogin = vi.fn();
        renderWithTheme(<SignInAction onLogin={onLogin} />);

        await user.click(screen.getByRole("button", { name: "Sign in / Register" }));
        expect(onLogin).toHaveBeenCalledTimes(1);
    });

    it("supports keyboard activation (Enter & Space)", async () => {
        const user = userEvent.setup();
        const onLogin = vi.fn();
        renderWithTheme(<SignInAction onLogin={onLogin} />);

        const btn = screen.getByRole("button", { name: "Sign in / Register" });
        btn.focus();
        await user.keyboard("{Enter}");
        await user.keyboard(" ");
        expect(onLogin).toHaveBeenCalledTimes(2);
    });
});