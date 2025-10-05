import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";

vi.mock("@/components/header/HeaderBar", () => ({
    __esModule: true,
    default: () => <div data-testid="header-bar">HEADER</div>,
}));
vi.mock("@/components/MainContent", () => ({
    __esModule: true,
    default: () => <div data-testid="main-content">MAIN</div>,
}));

import App from "./App";

beforeEach(() => {
    globalThis.location.hash = "";
});

describe("App", () => {
    it("renders the skip link, header, and main region", () => {
        render(<App />);

        const skip = screen.getByRole("link", { name: /skip to content/i });
        expect(skip).toBeInTheDocument();
        expect(skip).toHaveAttribute("href", "#main");

        expect(screen.getByTestId("header-bar")).toBeInTheDocument();

        const main = screen.getByRole("main");
        expect(main).toBeInTheDocument();
        expect(main).toHaveAttribute("id", "main");

        expect(screen.getByTestId("main-content")).toBeInTheDocument();
    });

    it("clicking the skip link navigates to #main", async () => {
        const user = userEvent.setup();
        render(<App />);

        const skip = screen.getByRole("link", { name: /skip to content/i });
        await user.click(skip);

        expect(window.location.hash).toBe("#main");
    });

    it("layout container fills viewport height", () => {
        const { container } = render(<App />);
        const outer = container.firstElementChild as HTMLElement;
        expect(outer).toHaveStyle({ display: "flex", minHeight: "100vh", flexDirection: "column" });
    });
});
