import * as React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import { hexToRgb } from "@mui/material";

let toggleModeMock = vi.fn();

vi.mock("../theme/ThemeModeProvider", () => ({
    __esModule: true,
    useThemeMode: () => ({ toggleMode: toggleModeMock }),
}));

import ThemeToggleMobile from "./ThemeToggleMobile";

const LIGHT_THUMB = "#ebe713";
const DARK_THUMB  = "#1a1e38";
type Mode = "light" | "dark";

function renderWithMode(
    ui: React.ReactNode,
    mode: Mode,
    opts?: {
        withAppTokens?: boolean;
        withPaletteOnly?: boolean; 
        withThumbOnly?: boolean;
    }
) {
    const withAppTokens = opts?.withAppTokens ?? true;

    const appTokens =
        !withAppTokens
            ? undefined
            : opts?.withThumbOnly
                ? {
                    controls: {
                        modeToggle: {
                            thumb: "#ff00aa"
                        },
                    },
                }
                : opts?.withPaletteOnly
                    ? {
                        controls: {
                            modeToggle: {
                                palette: { lightThumb: LIGHT_THUMB, darkThumb: DARK_THUMB },
                            },
                        },
                    }
                    : {
                        controls: {
                            modeToggle: {
                                thumb: mode === "dark" ? DARK_THUMB : LIGHT_THUMB,
                                palette: { lightThumb: LIGHT_THUMB, darkThumb: DARK_THUMB },
                            },
                        },
                    };

    const theme = createTheme({
        palette: { mode },
        ...(withAppTokens ? { app: appTokens } : {}),
    } as any);

    return render(<ThemeProvider theme={theme}>{ui}</ThemeProvider>);
}

beforeEach(() => {
    toggleModeMock = vi.fn();
});

describe("ThemeToggleMobile", () => {
    it("light mode: shows DarkMode icon, correct colour & tooltip, triggers on click", async () => {
        const user = userEvent.setup();
        const { container } = renderWithMode(<ThemeToggleMobile />, "light", {
            withPaletteOnly: true
        });

        const button = screen.getByRole("button", { name: "Toggle color scheme" });
        expect(button).toBeInTheDocument();

        await user.hover(button);
        expect(await screen.findByText("Switch to dark mode")).toBeInTheDocument();

        const darkIcon = container.querySelector('[data-testid="DarkModeRoundedIcon"]') as SVGElement;
        expect(darkIcon).toBeTruthy();
        expect(getComputedStyle(darkIcon).color).toBe(hexToRgb(DARK_THUMB));

        await user.click(button);
        expect(toggleModeMock).toHaveBeenCalledTimes(1);
    });

    it("dark mode: shows LightMode icon, correct colour & tooltip, triggers on click", async () => {
        const user = userEvent.setup();
        const { container } = renderWithMode(<ThemeToggleMobile />, "dark", {
            withPaletteOnly: true
        });

        const button = screen.getByRole("button", { name: "Toggle color scheme" });

        await user.hover(button);
        expect(await screen.findByText("Switch to light mode")).toBeInTheDocument();

        const lightIcon = container.querySelector('[data-testid="LightModeRoundedIcon"]') as SVGElement;
        expect(lightIcon).toBeTruthy();
        expect(getComputedStyle(lightIcon).color).toBe(hexToRgb(LIGHT_THUMB));

        await user.click(button);
        expect(toggleModeMock).toHaveBeenCalledTimes(1);
    });

    it("falls back to modeToggle.thumb when palette.{lightThumb,darkThumb} not provided", async () => {
        const { container } = renderWithMode(<ThemeToggleMobile />, "light", {
            withThumbOnly: true
        });

        const darkIcon = container.querySelector('[data-testid="DarkModeRoundedIcon"]') as SVGElement;
        expect(darkIcon).toBeTruthy();
        expect(getComputedStyle(darkIcon).color).toBe(hexToRgb("#ff00aa"));
    });

    it('falls back to "#ffffff" when neither palette nor thumb are provided', async () => {
        const { container } = renderWithMode(<ThemeToggleMobile />, "dark", {
            withAppTokens: false
        });

        const lightIcon = container.querySelector('[data-testid="LightModeRoundedIcon"]') as SVGElement;
        expect(lightIcon).toBeTruthy();
        expect(getComputedStyle(lightIcon).color).toBe(hexToRgb("#ffffff"));
    });

    it("a11y: supports keyboard activation (Enter & Space)", async () => {
        const user = userEvent.setup();
        renderWithMode(<ThemeToggleMobile />, "light");

        const button = screen.getByRole("button", { name: "Toggle color scheme" });
        button.focus();

        await user.keyboard("{Enter}");
        await user.keyboard(" ");

        expect(toggleModeMock).toHaveBeenCalledTimes(2);
    });
});
