import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import Delayed from "./Delayed";
import { act } from "react";

describe("Delayed", () => {
    beforeEach(() => {
        vi.useFakeTimers();
    });

    afterEach(() => {
        vi.runOnlyPendingTimers();
        vi.useRealTimers();
    });

    it("does not render children until delay elapses", () => {
        render(
            <Delayed show={true} delayMs={100}>
                <div data-testid="child" />
            </Delayed>
        );

        expect(screen.queryByTestId("child")).toBeNull();

        act(() => {
            vi.advanceTimersByTime(99);
        });
        expect(screen.queryByTestId("child")).toBeNull();

        act(() => {
            vi.advanceTimersByTime(1);
        });
        expect(screen.getByTestId("child")).toBeInTheDocument();
    });

    it("hides immediately when show becomes false (cancels pending timer)", () => {
        const { rerender } = render(
            <Delayed show={true} delayMs={100}>
                <div data-testid="child" />
            </Delayed>
        );
        
        expect(screen.queryByTestId("child")).toBeNull();
        
        rerender(
            <Delayed show={false} delayMs={100}>
                <div data-testid="child" />
            </Delayed>
        );

        expect(screen.queryByTestId("child")).toBeNull();
        vi.advanceTimersByTime(200);
        expect(screen.queryByTestId("child")).toBeNull();
    });

    it("re-shows after being hidden, using the new delay", () => {
        const { rerender } = render(
            <Delayed show={false} delayMs={50}>
                <div data-testid="child" />
            </Delayed>
        );

        rerender(
            <Delayed show={true} delayMs={50}>
                <div data-testid="child" />
            </Delayed>
        );

        expect(screen.queryByTestId("child")).toBeNull();

        act(() => {
            vi.advanceTimersByTime(49);
        });
        expect(screen.queryByTestId("child")).toBeNull();

        act(() => {
            vi.advanceTimersByTime(1);
        });
        expect(screen.getByTestId("child")).toBeInTheDocument();
        
        rerender(
            <Delayed show={false} delayMs={50}>
                <div data-testid="child" />
            </Delayed>
        );
        expect(screen.queryByTestId("child")).toBeNull();
    });

    it("cleans up timer on unmount (no stray render after)", () => {
        const { unmount } = render(
            <Delayed show={true} delayMs={100}>
                <div data-testid="child" />
            </Delayed>
        );

        unmount();

        expect(() => vi.advanceTimersByTime(200)).not.toThrow();
    });
});
