import { describe, it, expect, beforeEach, vi, afterEach } from "vitest";
import { render, screen, act, fireEvent } from "@testing-library/react";
// @ts-ignore
import { useInfiniteDistilleries } from "@/hooks/distilleries/useInfiniteDistilleries";

const hoisted = vi.hoisted(() => {
    const getAll = vi.fn<
        (args: {
            cursor?: string | null;
            amount?: number;
            countryId?: string | null;
            regionId?: string | null;
            pattern?: string | null;
            signal?: AbortSignal;
        }) => Promise<{ items: Array<{ id: string }>; nextCursor: string | null }>
    >();
    return { getAll };
});

vi.mock("@/api/distilleries/distilleriesApi", () => ({
    getAllDistilleries: hoisted.getAll,
}));

vi.mock("@/lib/domain/types", () => ({} as any));

let lastObserver:
    | { observe: (el: Element) => void; disconnect: () => void; cb: (entries: any[]) => void }
    | null = null;

class IO {
    _cb: (entries: any[]) => void;
    constructor(cb: any) {
        this._cb = cb;
        lastObserver = { observe: () => {}, disconnect: () => {}, cb };
    }
    observe() {}
    disconnect() {}
}
(globalThis as any).IntersectionObserver = IO as any;

function flush() {
    return act(() => Promise.resolve());
}

function TestBed(props: {
    initialAmount?: number;
    countryId?: string | null;
    regionId?: string | null;
    pattern?: string | null;
}) {
    const hook = (useInfiniteDistilleries as any)(props);
    return (
        <div>
            <button onClick={hook.loadNext}>loadNext</button>
            <button onClick={hook.refresh}>refresh</button>
            <div data-testid="sentinel" ref={hook.setSentinel} />
            <pre data-testid="state">{JSON.stringify(hook)}</pre>
            <pre data-testid="ids">{JSON.stringify(hook.items.map((x: any) => x.id))}</pre>
            <span data-testid="loading">{String(hook.loading)}</span>
            <span data-testid="isPageLoading">{String(hook.isPageLoading)}</span>
            <span data-testid="hasMore">{String(hook.hasMore)}</span>
            <span data-testid="error">{hook.error ? "ERR" : ""}</span>
        </div>
    );
}

const read = (id: string) => (screen.getByTestId(id) as HTMLElement).textContent!;
const readJSON = (id: string) => JSON.parse(read(id) || "null");

function makePage(ids: string[], nextCursor: string | null) {
    return { items: ids.map((id) => ({ id })), nextCursor };
}

describe("useInfiniteDistilleries", () => {
    beforeEach(() => {
        hoisted.getAll.mockReset();
    });

    afterEach(() => {
        vi.restoreAllMocks();
    });

    it("loads first page on mount with cursor=null and provided amount", async () => {
        hoisted.getAll.mockResolvedValueOnce(makePage(["1", "2", "3"], "CUR1"));

        render(<TestBed initialAmount={10} />);
        await flush();

        expect(hoisted.getAll).toHaveBeenCalledTimes(1);
        const args = hoisted.getAll.mock.calls[0][0];
        expect(args).toEqual(expect.objectContaining({ cursor: null, amount: 10 }));
        expect(readJSON("ids")).toEqual(["1", "2", "3"]);
        expect(read("hasMore")).toBe("true");
        expect(read("loading")).toBe("false");
        expect(read("isPageLoading")).toBe("false");
    });

    it("loadNext fetches next cursor and appends (no duplicates) then sets hasMore=false at end", async () => {
        hoisted.getAll
            .mockResolvedValueOnce(makePage(["1", "2", "3"], "CUR1"))
            .mockResolvedValueOnce(makePage(["3", "4"], null)); // note duplicate "3"

        render(<TestBed initialAmount={10} />);
        await flush();

        fireEvent.click(screen.getByText("loadNext"));
        await flush();

        expect(hoisted.getAll).toHaveBeenCalledTimes(2);
        const lastArgs = hoisted.getAll.mock.calls.at(-1)![0];
        expect(lastArgs).toEqual(expect.objectContaining({ cursor: "CUR1", amount: 10 }));

        expect(readJSON("ids")).toEqual(["1", "2", "3", "4"]); // deduped
        expect(read("hasMore")).toBe("false");
    });

    it("guards against concurrent page loads", async () => {
        let resolveSecond!: (v: any) => void;

        hoisted.getAll
            .mockResolvedValueOnce(makePage(["1", "2"], "CUR1"))
            .mockImplementationOnce(
                () =>
                    new Promise((res) => {
                        resolveSecond = res;
                    })
            );

        render(<TestBed initialAmount={5} />);
        await flush();

        fireEvent.click(screen.getByText("loadNext"));
        fireEvent.click(screen.getByText("loadNext"));

        expect(hoisted.getAll).toHaveBeenCalledTimes(2);
        resolveSecond!(makePage(["3"], null));
        await flush();

        expect(readJSON("ids")).toEqual(["1", "2", "3"]);
    });

    it("refresh resets to first page and clears error/loading flags", async () => {
        hoisted.getAll
            .mockResolvedValueOnce(makePage(["1"], "CUR1"))
            .mockRejectedValueOnce(new Error("boom"))
            .mockResolvedValueOnce(makePage(["A", "B"], "CURX"));

        render(<TestBed initialAmount={10} />);
        await flush();

        fireEvent.click(screen.getByText("loadNext"));
        await flush();

        expect(read("error")).toBe("ERR");

        fireEvent.click(screen.getByText("refresh"));
        await flush();

        const lastArgs = hoisted.getAll.mock.calls.at(-1)![0];
        expect(lastArgs).toEqual(expect.objectContaining({ cursor: null, amount: 10 }));
        expect(readJSON("ids")).toEqual(["A", "B"]);
        expect(read("error")).toBe("");
        expect(read("loading")).toBe("false");
    });

    it("setSentinel triggers loadNext when intersecting", async () => {
        hoisted.getAll
            .mockResolvedValueOnce(makePage(["1", "2", "3"], "CUR1"))
            .mockResolvedValueOnce(makePage(["4"], null));

        render(<TestBed initialAmount={10} />);
        await flush();

        expect(lastObserver).not.toBeNull();
        
        act(() => {
            lastObserver!.cb([{ isIntersecting: true } as any]);
        });
        await flush();

        expect(hoisted.getAll).toHaveBeenCalledTimes(2);
        expect(readJSON("ids")).toEqual(["1", "2", "3", "4"]);
    });

    it("applies filters (countryId/regionId/pattern) and refetches on change", async () => {
        hoisted.getAll
            .mockResolvedValueOnce(makePage(["1"], "CUR1"))
            .mockResolvedValueOnce(makePage(["2"], "CUR2"))
            .mockResolvedValueOnce(makePage(["X"], "CURX"));

        const { rerender } = render(
            <TestBed initialAmount={10} countryId={null} regionId={null} pattern={null} />
        );
        await flush();
        
        fireEvent.click(screen.getByText("loadNext"));
        await flush();
        
        rerender(<TestBed initialAmount={10} countryId={"SCO"} regionId={"HIGHLAND"} pattern={"glen"} />);
        await flush();
        
        const lastArgs = hoisted.getAll.mock.calls.at(-1)![0];
        expect(lastArgs).toEqual(
            expect.objectContaining({
                cursor: null,
                amount: 10,
                countryId: "SCO",
                regionId: "HIGHLAND",
                pattern: "glen",
            })
        );

        expect(readJSON("ids")).toEqual(["X"]);
    });

    it("sets error when page load fails but leaves previous items intact", async () => {
        hoisted.getAll.mockResolvedValueOnce(makePage(["1", "2"], "CUR1")).mockRejectedValueOnce(
            new Error("nope")
        );

        render(<TestBed initialAmount={10} />);
        await flush();

        fireEvent.click(screen.getByText("loadNext"));
        await flush();

        expect(read("error")).toBe("ERR");
        expect(readJSON("ids")).toEqual(["1", "2"]);
        expect(read("isPageLoading")).toBe("false");
    });
});
