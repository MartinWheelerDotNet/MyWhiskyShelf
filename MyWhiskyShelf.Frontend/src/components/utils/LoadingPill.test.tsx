import { describe, it, expect, beforeEach, vi } from "vitest";
import { render, screen, cleanup } from "@testing-library/react";

vi.mock("./Delayed", () => ({
    default: ({ show, children }: { show: boolean; children: React.ReactNode }) =>
        show ? <>{children}</> : null,
}));

import LoadingPill from "./LoadingPill";
import React from "react";

describe("LoadingPill", () => {
    beforeEach(() => {
        cleanup();
        vi.clearAllMocks();
    });

    it("has polite live region and status role", () => {
        render(<LoadingPill loading={false} hasMore={false} />);
        const wrapper = screen.getByRole("status");
        expect(wrapper).toHaveAttribute("aria-live", "polite");
    });

    it("shows spinner + 'Loading more…' when loading && hasMore", () => {
        render(<LoadingPill loading={true} hasMore={true} />);

        // CircularProgress renders with role="progressbar"
        expect(screen.getByRole("progressbar")).toBeInTheDocument();
        expect(screen.getByText("Loading more…")).toBeInTheDocument();

        // Should not show the end-of-list pill at the same time
        expect(screen.queryByText("No more results")).toBeNull();
    });

    it("shows 'No more results' when !loading && !hasMore", () => {
        render(<LoadingPill loading={false} hasMore={false} />);
        expect(screen.getByText("No more results")).toBeInTheDocument();
        // No spinner in this state
        expect(screen.queryByRole("progressbar")).toBeNull();
    });

    it("shows nothing when loading && !hasMore (no spinner, no end pill)", () => {
        render(<LoadingPill loading={true} hasMore={false} />);
        expect(screen.queryByRole("progressbar")).toBeNull();
        expect(screen.queryByText("No more results")).toBeNull();
    });

    it("shows nothing when !loading && hasMore (idle between loads)", () => {
        render(<LoadingPill loading={false} hasMore={true} />);
        expect(screen.queryByRole("progressbar")).toBeNull();
        expect(screen.queryByText("No more results")).toBeNull();
    });
});
