import { describe, it, expect, beforeEach, vi } from "vitest";
import { render, screen, cleanup } from "@testing-library/react";

const hoisted = vi.hoisted(() => ({
    mapToDistilleryCardProps: vi.fn((d: any) => ({ __mappedFrom: d?.id ?? "unknown" })),
}));

vi.mock("@/components/distilleries/SkeletonDistilleryCard", () => ({
    default: () => <div data-testid="skeleton-card" />,
}));

vi.mock("@/components/distilleries/DistilleryCard", () => ({
    default: (props: any) => (
        <div data-testid="distillery-card" data-props={JSON.stringify(props)} />
    ),
}));

vi.mock("@/lib/mappers/distillery", () => ({
    mapToDistilleryCardProps: hoisted.mapToDistilleryCardProps,
}));

import DistilleriesList from "./DistilleriesList";
// @ts-ignore
import {Distillery} from "@/lib/domain/types";

describe("DistilleriesList", () => {
    beforeEach(() => {
        cleanup();
        vi.clearAllMocks();
    });

    it("renders initial skeletons under initialLoading (default 6)", () => {
        render(
            <DistilleriesList
                items={[]}
                initialLoading={true}
                setSentinel={() => {}}
            />
        );

        const skeletons = screen.getAllByTestId("skeleton-card");
        expect(skeletons).toHaveLength(6);
        expect(screen.queryByTestId("distillery-card")).toBeNull();
    });

    it("renders a custom number of initial skeletons when initialSkeletonCount is provided", () => {
        render(
            <DistilleriesList
                items={[]}
                initialLoading={true}
                initialSkeletonCount={4}
                setSentinel={() => {}}
            />
        );

        const skeletons = screen.getAllByTestId("skeleton-card");
        expect(skeletons).toHaveLength(4);
    });

    it("renders DistilleryCard for items with a valid id and skips items with empty id", () => {
        const items: Distillery[] = [
            { id: "1", name: "A" },
            { id: "", name: "B" },
            { id: "2", name: "C" },
        ];

        render(
            <DistilleriesList
                items={items as any}
                initialLoading={false}
                showMoreSkeletons={false}
                setSentinel={() => {}}
            />
        );
        
        const cards = screen.getAllByTestId("distillery-card");
        expect(cards).toHaveLength(2);
        
        expect(hoisted.mapToDistilleryCardProps).toHaveBeenCalledTimes(2);
        expect(hoisted.mapToDistilleryCardProps).toHaveBeenNthCalledWith(1, expect.objectContaining({ id: "1" }));
        expect(hoisted.mapToDistilleryCardProps).toHaveBeenNthCalledWith(2, expect.objectContaining({ id: "2" }));
    });

    it("renders 'loading more' skeletons by default when showMoreSkeletons=true (default count 3)", () => {
        const items: Distillery[] = [{ id: "1", name: "A" }];

        render(
            <DistilleriesList
                items={items as any}
                initialLoading={false}
                setSentinel={() => {}}
            />
        );

        const appendedSkeletons = screen.getAllByTestId("skeleton-card");
        expect(appendedSkeletons).toHaveLength(3);
    });

    it("renders custom number of 'loading more' skeletons when moreSkeletonCount provided", () => {
        const items: Distillery[] = [{ id: "1", name: "A" }];

        render(
            <DistilleriesList
                items={items as any}
                initialLoading={false}
                showMoreSkeletons={true}
                moreSkeletonCount={5}
                setSentinel={() => {}}
            />
        );

        const appendedSkeletons = screen.getAllByTestId("skeleton-card");
        expect(appendedSkeletons).toHaveLength(5);
    });

    it("invokes setSentinel with a DOM element via callback ref", () => {
        const setSentinel = vi.fn();

        render(
            <DistilleriesList
                items={[] as any}
                initialLoading={false}
                showMoreSkeletons={false}
                setSentinel={setSentinel}
            />
        );
        
        const calls = setSentinel.mock.calls;
        const firstElArg = calls.find(([arg]) => arg instanceof HTMLElement)?.[0];

        expect(firstElArg).toBeInstanceOf(HTMLElement);
    });
});
