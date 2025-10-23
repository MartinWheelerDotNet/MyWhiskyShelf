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
    countryName: "Scotland",
    regionName: "Islay",
    founded: 1815,
    type: "Malt",
    isFavorite: false,
    description: "Renowned for intensely peated single malts.",
    tastingNotes: "Smoke, iodine, sea spray, citrus."
};

describe("DistilleryCard", () => {
    it("renders title and avatar fallback initial when no logoUrl", () => {
        renderWithTheme(<DistilleryCard {...baseProps} logoUrl={undefined} />);

        expect(screen.getByText("Ardbeg")).toBeInTheDocument();
        expect(screen.getByText("A")).toBeInTheDocument();
        expect(screen.getByText("Scotland")).toBeInTheDocument();
        expect(screen.getByText("Islay")).toBeInTheDocument();
        
    });

    it("does not render region chip when omitted", () => {
        renderWithTheme(
            <DistilleryCard
                {...baseProps}
                regionName={undefined}
            />
        );

        expect(screen.queryByText("Islay")).not.toBeInTheDocument();
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
