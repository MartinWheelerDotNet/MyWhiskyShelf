import * as React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import { hexToRgb } from "@mui/material";
import { COLOURS } from "../theme/tokens";

let toggleModeMock = vi.fn();
vi.mock("../theme/ThemeModeProvider", () => ({
    __esModule: true,
    useThemeMode: () => ({ toggleMode: toggleModeMock }),
}));

import ThemeToggleMobile from "./ThemeToggleMobile";

function renderWithMode(ui: React.ReactNode, mode: "light" | "dark") {
    const theme = createTheme({ palette: { mode } });
    return render(<ThemeProvider theme={theme}>{ui}</ThemeProvider>);
}

beforeEach(() => {
    toggleModeMock = vi.fn();
});

describe("ThemeToggleMobile", () => {
    it("light mode: shows DarkMode icon, correct colour & tooltip, triggers toggle on click", async () => {
        const user = userEvent.setup();
        const { container } = renderWithMode(<ThemeToggleMobile />, "light");

        const button = screen.getByRole("button", { name: "Toggle color scheme" });
        expect(button).toBeInTheDocument();

        await user.hover(button);
        expect(await screen.findByText("Switch to dark mode")).toBeInTheDocument();

        const darkIcon = container.querySelector('[data-testid="LightModeRoundedIcon"]') as SVGElement;
        expect(darkIcon).toBeTruthy();
        expect(getComputedStyle(darkIcon).color).toBe(hexToRgb(COLOURS.dark.modeToggle));

        await user.click(button);
        expect(toggleModeMock).toHaveBeenCalledTimes(1);
    });

    it("dark mode: shows LightMode icon, correct colour & tooltip, triggers toggle on click", async () => {
        const user = userEvent.setup();
        const { container } = renderWithMode(<ThemeToggleMobile />, "dark");

        const button = screen.getByRole("button", { name: "Toggle color scheme" });
        await user.hover(button);
        expect(await screen.findByText("Switch to light mode")).toBeInTheDocument();

        const lightIcon = container.querySelector('[data-testid="LightModeRoundedIcon"]') as SVGElement;
        expect(lightIcon).toBeTruthy();
        expect(getComputedStyle(lightIcon).color).toBe(hexToRgb(COLOURS.light.modeToggle));

        await user.click(button);
        expect(toggleModeMock).toHaveBeenCalledTimes(1);
    });

    it("desktop a11y: supports keyboard activation (Enter & Space)", async () => {
        const user = userEvent.setup();
        renderWithMode(<ThemeToggleMobile />, "light");
        const button = screen.getByRole("button", { name: "Toggle color scheme" });

        button.focus();
        await user.keyboard("{Enter}");
        await user.keyboard(" ");
        expect(toggleModeMock).toHaveBeenCalledTimes(2);
    });
});