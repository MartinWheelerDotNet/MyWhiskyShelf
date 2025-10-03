import * as React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import { hexToRgb } from "@mui/material";

let toggleModeMock = vi.fn();

// noinspection JSUnusedGlobalSymbols
vi.mock("../theme/ThemeModeProvider", () => ({
    __esModule: true,
    useThemeMode: () => ({ toggleMode: toggleModeMock }),
}));

import ThemeToggle from "./ThemeToggle";

function renderWithMode(ui: React.ReactNode, mode: "light" | "dark") {
    const theme = createTheme({ palette: { mode } });
    return render(<ThemeProvider theme={theme}>{ui}</ThemeProvider>);
}

beforeEach(() => {
    toggleModeMock = vi.fn();
});

describe("ThemeToggle", () => {
    it("renders unchecked in light mode, shows correct tooltip, and thumb is yellow", async () => {
        const user = userEvent.setup();
        const { container } = renderWithMode(<ThemeToggle />, "light");

        const checkbox = screen.getByRole("checkbox", { name: "Toggle color scheme" });
        expect(checkbox).not.toBeChecked();

        await user.hover(checkbox);
        expect(await screen.findByText("Switch to dark mode")).toBeInTheDocument();

        const thumb = container.querySelector(".MuiSwitch-thumb") as HTMLElement;
        expect(getComputedStyle(thumb).backgroundColor).toBe(hexToRgb("#ebe713"));
    });

    it("renders checked in dark mode, shows correct tooltip, and thumb is dark blue", async () => {
        const user = userEvent.setup();
        const { container } = renderWithMode(<ThemeToggle />, "dark");

        const checkbox = screen.getByRole("checkbox", { name: "Toggle color scheme" });
        expect(checkbox).toBeChecked();

        await user.hover(checkbox);
        expect(await screen.findByText("Switch to light mode")).toBeInTheDocument();

        const thumb = container.querySelector(".MuiSwitch-thumb") as HTMLElement;
        expect(getComputedStyle(thumb).backgroundColor).toBe(hexToRgb("#1a1e38"));
    });

    it("calls toggleMode when clicked", async () => {
        const user = userEvent.setup();
        renderWithMode(<ThemeToggle />, "light");

        const checkbox = screen.getByRole("checkbox", { name: "Toggle color scheme" });
        await user.click(checkbox);

        expect(toggleModeMock).toHaveBeenCalledTimes(1);
    });
});