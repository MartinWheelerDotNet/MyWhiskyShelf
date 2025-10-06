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
    description: "Renowned for intensely peated single malts.",
    tastingNotes: "Smoke, iodine, sea spray, citrus.",
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

    it("pluralizes 'whiskies' correctly when count is 0", () => {
        renderWithTheme(<DistilleryCard {...baseProps} whiskiesCount={0} />);
        expect(screen.getByText(/0 whiskies/i)).toBeInTheDocument();
    });

    it("does not render country/region/whiskies chips when omitted", () => {
        renderWithTheme(
            <DistilleryCard
                {...baseProps}
                country={undefined}
                region={undefined}
                whiskiesCount={undefined}
            />
        );

        expect(screen.queryByText("Scotland")).not.toBeInTheDocument();
        expect(screen.queryByText("Islay")).not.toBeInTheDocument();
        expect(screen.queryByText(/whisky|whiskies/i)).not.toBeInTheDocument();
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

    it("expand button toggles details and aria-expanded then collapses again", async () => {
        const user = userEvent.setup();
        renderWithTheme(<DistilleryCard {...baseProps} />);

        expect(screen.queryByRole("heading", { name: /about/i })).not.toBeInTheDocument();
        expect(screen.queryByRole("heading", { name: /tasting notes/i })).not.toBeInTheDocument();
        expect(screen.queryByRole("heading", { name: /my bottles/i })).not.toBeInTheDocument();

        const expandBtn = screen.getByRole("button", { name: /show more/i });

        await user.click(expandBtn);
        expect(expandBtn).toHaveAttribute("aria-expanded", "true");

        const aboutHeading = await screen.findByRole("heading", { name: /about/i });
        const notesHeading = await screen.findByRole("heading", { name: /tasting notes/i });
        const bottlesHeading = await screen.findByRole("heading", { name: /my bottles/i });

        expect(aboutHeading).toBeInTheDocument();
        expect(notesHeading).toBeInTheDocument();
        expect(bottlesHeading).toBeInTheDocument();

        await user.click(expandBtn);
        expect(expandBtn).toHaveAttribute("aria-expanded", "false");
        expect(screen.queryByRole("heading", { name: /about/i })).not.toBeInTheDocument();
    });

    it("shows founded fallback '-' when no founded provided", async () => {
        renderWithTheme(<DistilleryCard {...baseProps} founded={undefined} />);

        const expandBtn = screen.getByRole("button", { name: /show more/i });
        await userEvent.click(expandBtn);

        expect(screen.getByText(/^\s*Founded:\s*-\s*$/i)).toBeInTheDocument();
    });

    it("shows 'Owned By: -' fallback when no owner provided", async () => {
        renderWithTheme(<DistilleryCard {...baseProps} owner={undefined} />);

        const expandBtn = screen.getByRole("button", { name: /show more/i });
        await userEvent.click(expandBtn);

        expect(screen.getByText(/^\s*Owned By:\s*-\s*$/i)).toBeInTheDocument();
    });

    it("shows em dash fallback for description and tasting notes", async () => {
        renderWithTheme(
            <DistilleryCard
                {...baseProps}
                description={undefined}
                tastingNotes={undefined}
            />
        );

        const expandBtn = screen.getByRole("button", { name: /show more/i });
        await userEvent.click(expandBtn);

        // Both About->description and Tasting Notes body should show "—"
        const dashes = screen.getAllByText("—");
        expect(dashes.length).toBeGreaterThanOrEqual(2);
    });

    it("renders non-numeric founded value when provided as string", async () => {
        renderWithTheme(<DistilleryCard {...baseProps} founded="Pre-1800s" />);

        const expandBtn = screen.getByRole("button", { name: /show more/i });
        await userEvent.click(expandBtn);

        expect(screen.getByText(/^\s*Founded:\s*Pre-1800s\s*$/i)).toBeInTheDocument();
    });

    it("uses provided logoUrl when present", () => {
        renderWithTheme(
            <DistilleryCard {...baseProps} logoUrl="/media/images/distilleries/ardbeg-logo.png" />
        );

        const img = screen.getByRole("img", { name: /ardbeg/i }) as HTMLImageElement;
        expect(img).toBeInTheDocument();
        expect(img.src).toContain("/media/images/distilleries/ardbeg-logo.png");
    });

    it("clicking favorite without a handler does not throw", async () => {
        const user = userEvent.setup();
        renderWithTheme(<DistilleryCard {...baseProps} onToggleFavorite={undefined} />);
        await user.click(screen.getByRole("button", { name: /toggle favorite/i }));
        // If it reaches here without error, it's fine.
        expect(true).toBe(true);
    });
});
