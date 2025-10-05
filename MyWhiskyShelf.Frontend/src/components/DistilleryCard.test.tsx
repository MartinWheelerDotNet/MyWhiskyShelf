import * as React from "react";
import { describe, it, expect, beforeEach, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import DistilleryCard, { type DistilleryCardProps } from "./DistilleryCard";

function renderWithTheme(ui: React.ReactNode) {
    const theme = createTheme({ palette: { mode: "light" } });
    return render(<ThemeProvider theme={theme}>{ui}</ThemeProvider>);
}

beforeEach(() => {
    globalThis.location.hash = "";
    vi.clearAllMocks();
});

const baseProps: DistilleryCardProps = {
    id: "ardbeg",
    name: "Ardbeg",
    region: "Islay",
    country: "Scotland",
    founded: 1815,
    isFavorite: false,
    about: "Renowned for intensely peated single malts.",
    notes: "Smoke, iodine, sea spray, citrus.",
    whiskiesCount: 17,
};

describe("DistilleryCard", () => {
    it("renders title and avatar fallback initial when no logoUrl", () => {
        renderWithTheme(<DistilleryCard {...baseProps} logoUrl={undefined} />);

        expect(screen.getByText("Ardbeg")).toBeInTheDocument();

        expect(screen.getByText("A")).toBeInTheDocument();

        expect(screen.getByText("Scotland")).toBeInTheDocument();
        expect(screen.getByText("Islay")).toBeInTheDocument();

        expect(screen.getByText(/17 whiskies/i)).toBeInTheDocument();
    });

    it("pluralizes 'whisky' correctly when count is 1", () => {
        renderWithTheme(<DistilleryCard {...baseProps} whiskiesCount={1} />);
        expect(screen.getByText(/1 whisky/i)).toBeInTheDocument();
    });

    it("favorite button calls onToggleFavorite with id", async () => {
        const user = userEvent.setup();
        const onToggleFavorite = vi.fn();

        renderWithTheme(
            <DistilleryCard {...baseProps} onToggleFavorite={onToggleFavorite} />
        );

        const favBtn = screen.getByRole("button", { name: /toggle favorite/i });
        await user.click(favBtn);

        expect(onToggleFavorite).toHaveBeenCalledTimes(1);
        expect(onToggleFavorite).toHaveBeenCalledWith("ardbeg");
    });

    it("favorite tooltip reflects isFavorite = false/true", async () => {
        const user = userEvent.setup();

        const { rerender } = renderWithTheme(
            <DistilleryCard {...baseProps} isFavorite={false} />
        );

        const favBtn = screen.getByRole("button", { name: /toggle favorite/i });

        await user.hover(favBtn);
        expect(
            await screen.findByText(/add to favorites/i)
        ).toBeInTheDocument();

        await user.unhover(favBtn);

        rerender(
            <ThemeProvider theme={createTheme({ palette: { mode: "light" } })}>
                <DistilleryCard {...baseProps} isFavorite />
            </ThemeProvider>
        );

        await user.hover(screen.getByRole("button", { name: /toggle favorite/i }));
        expect(await screen.findByText(/unfavorite/i)).toBeInTheDocument();
    });

    it("expand button toggles details and aria-expanded", async () => {
        const user = userEvent.setup();
        renderWithTheme(<DistilleryCard {...baseProps} />);

        expect(screen.queryByRole("heading", { name: /about/i })).not.toBeInTheDocument();
        expect(screen.queryByRole("heading", { name: /tasting notes/i })).not.toBeInTheDocument();
        expect(screen.queryByRole("heading", { name: /my bottles/i })).not.toBeInTheDocument();

        const expandBtn = screen.getByRole("button", { name: /show more/i });

        await user.hover(expandBtn);
        expect(await screen.findByText(/expand/i)).toBeInTheDocument();
        await user.unhover(expandBtn);

        await user.click(expandBtn);
        expect(expandBtn).toHaveAttribute("aria-expanded", "true");

        const aboutHeading = await screen.findByRole("heading", { name: /about/i });
        const notesHeading = await screen.findByRole("heading", { name: /tasting notes/i });
        const bottlesHeading = await screen.findByRole("heading", { name: /my bottles/i });

        expect(aboutHeading).toBeInTheDocument();
        expect(notesHeading).toBeInTheDocument();
        expect(bottlesHeading).toBeInTheDocument();

        expect(
            screen.getByText(/This is a list of my bottles which will be retrieved from the API./i)
        ).toBeInTheDocument();

        await user.hover(expandBtn);
        expect(await screen.findByText(/collapse/i)).toBeInTheDocument();
    });

    it("shows founded fallback '-' when no founded provided", async () => {
        renderWithTheme(<DistilleryCard {...baseProps} founded={undefined} />);

        const expandBtn = screen.getByRole("button", { name: /show more/i });
        await userEvent.click(expandBtn);

        expect(screen.getByText(/founded:/i)).toBeInTheDocument();
        expect(screen.getByText("-")).toBeInTheDocument();
    });

    it("uses provided logoUrl when present", () => {
        renderWithTheme(
            <DistilleryCard {...baseProps} logoUrl="/media/images/distilleries/ardbeg-logo.png" />
        );

        const img = screen.getByRole("img", { name: /ardbeg/i }) as HTMLImageElement;
        expect(img).toBeInTheDocument();
        expect(img.src).toContain("/media/images/distilleries/ardbeg-logo.png");
    });
});
