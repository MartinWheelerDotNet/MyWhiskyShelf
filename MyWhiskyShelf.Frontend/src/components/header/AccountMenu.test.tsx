import * as React from "react";
import { describe, it, expect, vi } from "vitest";
import {render, screen, within} from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import AccountMenu from "./AccountMenu";
import {COLOURS} from "../../theme/tokens.ts";
import {hexToRgb} from "@mui/material";

function renderWithTheme(ui: React.ReactNode, mode: "light" | "dark" = "light") {
    const theme = createTheme({ palette: { mode } });
    return render(<ThemeProvider theme={theme}>{ui}</ThemeProvider>);
}

describe("AccountMenu", () => {
    it("opens menu and logs out", async () => {
        const user = userEvent.setup();
        const onLogout = vi.fn();
        const { container } = renderWithTheme(<AccountMenu username="AdaLovelace" initials="AL" onLogout={onLogout} />);
        const chip = within(container).getByRole("button", { name: "Account"});
        await user.click(chip);
        
        const accountMenu = await screen.findByRole("menu", { name: "Account" });
        expect(within(accountMenu).getByText("AdaLovelace")).toBeInTheDocument();
        
        await user.click(within(accountMenu).getByRole("menuitem", { name: "Logout" }));
        expect(onLogout).toHaveBeenCalled();
    });

    it("uses light chip in light mode", () => {
        const { container } = renderWithTheme(
            <AccountMenu username="AdaLovelace" initials="AL" onLogout={() => {}} />,
            "light"
        );
        
        const chip = within(container).getByRole('button', { name: "Account" });
        const avatar = within(chip).getByRole('generic', { name: "Account Avatar" });
        
        expect(getComputedStyle(chip).backgroundColor)
            .toBe(hexToRgb(COLOURS.light.border));
        expect(getComputedStyle(avatar as HTMLElement).backgroundColor)
            .toBe(hexToRgb(COLOURS.dark.text));
    });

    it("uses dark chip in dark mode", () => {
        const { container } = renderWithTheme(
            <AccountMenu username="AdaLovelace" initials="AL" onLogout={() => {}} />,
            "dark"
        );

        const chip = within(container).getByRole('button', { name: "Account" });
        const avatar = within(chip).getByRole('generic', { name: "Account Avatar" });

        expect(getComputedStyle(chip).backgroundColor)
            .toBe(hexToRgb(COLOURS.dark.border));
        expect(getComputedStyle(avatar as HTMLElement).backgroundColor)
            .toBe(hexToRgb(COLOURS.light.text));
    });
});