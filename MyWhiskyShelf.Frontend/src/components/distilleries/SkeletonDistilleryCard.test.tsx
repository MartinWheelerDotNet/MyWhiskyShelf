import { describe, it, expect, beforeEach, vi } from "vitest";
import { render, screen, cleanup } from "@testing-library/react";

const mui = vi.hoisted(() => {
    const alpha = (c: string) => c;
    const useMediaQuery = vi.fn().mockReturnValue(false);
    const Skeleton = (props: any) => (
        <div
            data-testid="sk"
            data-variant={props.variant}
            data-width={props.width != null ? String(props.width) : undefined}
            data-height={props.height != null ? String(props.height) : undefined}
            data-animation={props.animation != null ? String(props.animation) : undefined}
        />
    );
    const Card = (p: any) => <div data-testid="card">{p.children}</div>;
    const Stack = (p: any) => <div data-testid="stack">{p.children}</div>;
    const CardHeader = ({ avatar, title, subheader, action }: any) => (
        <div data-testid="card-header">
            <div data-testid="slot-avatar">{avatar}</div>
            <div data-testid="slot-title">{title}</div>
            <div data-testid="slot-subheader">{subheader}</div>
            <div data-testid="slot-action">{action}</div>
        </div>
    );

    return { alpha, useMediaQuery, Skeleton, Card, Stack, CardHeader };
});

vi.mock("@mui/material", () => ({
    alpha: mui.alpha,
    useMediaQuery: mui.useMediaQuery,
    Skeleton: mui.Skeleton,
    Card: mui.Card,
    Stack: mui.Stack,
    CardHeader: mui.CardHeader,
}));

import SkeletonDistilleryCard from "./SkeletonDistilleryCard";

const byVariant = (variant: string) =>
    screen.getAllByTestId("sk").filter((n) => n.getAttribute("data-variant") === variant);

describe("SkeletonDistilleryCard", () => {
    beforeEach(() => {
        cleanup();
        vi.clearAllMocks();
        mui.useMediaQuery.mockReturnValue(false);
    });

    it("renders a Card with a CardHeader and seven skeleton parts", () => {
        render(<SkeletonDistilleryCard />);

        expect(screen.getByTestId("card")).toBeInTheDocument();
        expect(screen.getByTestId("card-header")).toBeInTheDocument();

        const all = screen.getAllByTestId("sk");
        expect(all).toHaveLength(7);

        expect(byVariant("circular")).toHaveLength(3);
        expect(byVariant("text")).toHaveLength(1);
        expect(byVariant("rounded")).toHaveLength(3);
    });

    it("uses expected sizes for avatar/action/chip skeletons", () => {
        render(<SkeletonDistilleryCard />);

        const circular = byVariant("circular");
        const sizes = circular.map((n) => ({
            w: n.getAttribute("data-width"),
            h: n.getAttribute("data-height"),
        }));
        
        expect(sizes).toContainEqual({ w: "40", h: "40" });
        expect(sizes.filter((s) => s.w === "36" && s.h === "36")).toHaveLength(2);

        const rounded = byVariant("rounded");
        const roundedWidths = rounded.map((n) => n.getAttribute("data-width"));
        const roundedHeights = new Set(rounded.map((n) => n.getAttribute("data-height")));
        expect(roundedWidths.sort()).toEqual(["110", "80", "90"].sort());
        expect(roundedHeights).toEqual(new Set(["24"]));
    });

    it("sets animation='wave' by default (reduced motion OFF)", () => {
        mui.useMediaQuery.mockReturnValue(false);
        render(<SkeletonDistilleryCard />);

        for (const el of screen.getAllByTestId("sk")) {
            expect(el.getAttribute("data-animation")).toBe("wave");
        }
    });

    it("disables animation when prefers-reduced-motion is true", () => {
        mui.useMediaQuery.mockReturnValue(true);
        render(<SkeletonDistilleryCard />);

        for (const el of screen.getAllByTestId("sk")) {
            expect(el.getAttribute("data-animation")).toBe("false");
        }
    });
});
