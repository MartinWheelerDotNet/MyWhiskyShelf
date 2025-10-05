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

import ThemeToggle from "./ThemeToggle";

function renderWithMode(ui: React.ReactNode, mode: "light" | "dark") {
    const thumb = mode === "dark" ? "#1a1e38" : "#ebe713";

    const theme = createTheme({
        palette: { mode },
        app: {
            controls: {
                modeToggle: {
                    thumb,
                    icon: "#ffffff",
                    track: {
                        light: "linear-gradient(90deg, #f0b429 0%, #ff8ba7 100%)",
                        dark: "linear-gradient(90deg, #34b1eb 0%, #0e4f6e 100%)",
                    },
                    width: 70,
                    height: 46,
                    padding: 10,
                    thumbSize: 20,
                    leftThumbPos: 12,
                },
            },
        },
    } as any);

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