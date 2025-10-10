import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";
import { render, screen, fireEvent, cleanup } from "@testing-library/react";
import { act } from "react";

const hoisted = vi.hoisted(() => {
    const getAll = vi.fn<
        (page: number, amount: number, signal?: AbortSignal) => Promise<any>
    >();
    const getId = (x: any) => String(x?.id ?? "");

    return { getAll, getId };
});

vi.mock("@/api/distilleries/distilleriesApi", () => ({
    getAllDistilleries: hoisted.getAll,
}));

vi.mock("@/features/distilleries/getDistilleryId", () => ({
    getDistilleryId: hoisted.getId,
}));

// @ts-ignore
import { useInfiniteDistilleries } from "@/hooks/distilleries/useInfiniteDistilleries";

type IOEntry = { isIntersecting: boolean; target: Element };
let lastObserver: {
    cb: (entries: IOEntry[]) => void;
    observe: (el: Element) => void;
    disconnect: () => void;
} | null = null;

beforeEach(() => {
    (global as any).IntersectionObserver = class {
        cb: (entries: IOEntry[]) => void;
        constructor(cb: (entries: IOEntry[]) => void) {
            this.cb = cb;
            lastObserver = {
                cb,
                observe: () => {
                },
                disconnect: () => {
                },
            };
        }
        observe() { }
        disconnect() {}
    } as any;
});

afterEach(() => {
    lastObserver = null;
    cleanup();
    vi.clearAllMocks();
});

const flush = async () => {
    await act(async () => {
        await Promise.resolve();
        await Promise.resolve();
    });
};

function pagePayload(page: number, amount: number, ids: (string | number)[]) {
    return {
        items: ids.map((id) => ({ id: String(id), name: `D${id}` })),
        page,
        amount,
    };
}

function Host({ amount = 10 }: { amount?: number }) {
    const {
        items,
        page,
        amount: hookAmount,
        loading,
        isPageLoading,
        hasMore,
        endReached,
        error,
        loadNext,
        refresh,
        setSentinel,
    } = useInfiniteDistilleries(amount);

    return (
        <div>
            <div data-testid="page">{String(page)}</div>
            <div data-testid="amount">{String(hookAmount)}</div>
            <div data-testid="loading">{String(loading)}</div>
            <div data-testid="isPageLoading">{String(isPageLoading)}</div>
            <div data-testid="hasMore">{String(hasMore)}</div>
            <div data-testid="endReached">{String(endReached)}</div>
            <div data-testid="error">{String(!!error)}</div>
            <div data-testid="ids">{JSON.stringify(items.map((x: any) => x.id))}</div>
            <button data-testid="next" onClick={loadNext} />
            <button data-testid="refresh" onClick={refresh} />
            <div data-testid="sentinel" ref={setSentinel} />
        </div>
    );
}

const read = (id: string) => screen.getByTestId(id).textContent!;
const readJSON = (id: string) => JSON.parse(read(id) || "null");

describe("useInfiniteDistilleries", () => {
    beforeEach(() => {
        vi.useRealTimers();
    });

    it("loads page 1 on mount (default amount=10) and populates state", async () => {
        hoisted.getAll.mockResolvedValueOnce(pagePayload(1, 10, [1,2,3]));

        render(<Host amount={10} />);
        expect(read("loading")).toBe("true");

        await flush();

        expect(hoisted.getAll).toHaveBeenCalledTimes(1);
        const [p1, a1] = hoisted.getAll.mock.calls[0];
        expect(p1).toBe(1);
        expect(a1).toBe(10);
        
        expect(read("loading")).toBe("false");
        expect(read("isPageLoading")).toBe("false");
        expect(read("page")).toBe("1");
        expect(read("amount")).toBe("10");
        expect(read("hasMore")).toBe("false");
        expect(readJSON("ids")).toEqual(["1","2","3"]);
    });

    it("loadNext appends next page and updates page; merges by id without duplicates", async () => {
        hoisted.getAll
            .mockResolvedValueOnce(pagePayload(1, 10, [1,2,3,4,5,6,7,8,9,10]))
            .mockResolvedValueOnce(pagePayload(2, 10, [10,11,12]));

        render(<Host amount={10} />);
        await flush();

        fireEvent.click(screen.getByTestId("next"));
        await flush();

        const c1 = hoisted.getAll.mock.calls[0];
        const c2 = hoisted.getAll.mock.calls[1];
        expect(c1[0]).toBe(1); expect(c1[1]).toBe(10);
        expect(c2[0]).toBe(2); expect(c2[1]).toBe(10);

        expect(read("page")).toBe("2");
        expect(readJSON("ids")).toEqual([
            "1","2","3","4","5","6","7","8","9","10","11","12"
        ]);
    });

    it("sets hasMore=false and endReached=true when a smaller page is returned", async () => {
        hoisted.getAll
            .mockResolvedValueOnce(pagePayload(1, 10, [1,2,3,4,5,6,7,8,9,10]))
            .mockResolvedValueOnce(pagePayload(2, 10, [11,12,13,14]));

        render(<Host amount={10} />);
        await flush();

        fireEvent.click(screen.getByTestId("next"));
        await flush();

        expect(read("hasMore")).toBe("false");
        expect(read("endReached")).toBe("true");
        
        fireEvent.click(screen.getByTestId("next"));
        await flush();
        expect(hoisted.getAll).toHaveBeenCalledTimes(2);
    });

    it("guards against duplicate in-flight loads (only 1 network call for a given next page)", async () => {

        hoisted.getAll.mockResolvedValueOnce(pagePayload(1, 10, [1,2,3,4,5,6,7,8,9,10]));

        let resolveP2!: (v: any) => void;
        const p2 = new Promise((res) => { resolveP2 = res; });
        hoisted.getAll.mockImplementationOnce((page) => {
            if (page === 2) return p2 as Promise<any>;
            return Promise.reject(new Error("unexpected"));
        });

        render(<Host amount={10} />);
        await flush();

        fireEvent.click(screen.getByTestId("next"));
        fireEvent.click(screen.getByTestId("next"));
        

        expect(hoisted.getAll).toHaveBeenCalledTimes(2);

        resolveP2(pagePayload(2, 10, [11,12]));
        await flush();

        expect(read("page")).toBe("2");
        expect(readJSON("ids")).toEqual(["1","2","3","4","5","6","7","8","9","10","11","12"]);
    });

    it("refresh resets state and loads page 1 again", async () => {
        hoisted.getAll
            .mockResolvedValueOnce(pagePayload(1, 10, [1,2,3,4,5,6,7,8,9,10]))
            .mockResolvedValueOnce(pagePayload(2, 10, [11,12]))
            .mockResolvedValueOnce(pagePayload(1, 10, [1]));

        render(<Host amount={10} />);
        await flush();
        fireEvent.click(screen.getByTestId("next"));
        await flush();

        fireEvent.click(screen.getByTestId("refresh"));
        await flush();
        await flush();

        expect(hoisted.getAll).toHaveBeenCalledTimes(3);
        const last = hoisted.getAll.mock.calls.at(-1)!;
        expect(last[0]).toBe(1);
        expect(last[1]).toBe(10);

        expect(read("page")).toBe("1");
        expect(readJSON("ids")).toEqual(["1"]);
        expect(read("hasMore")).toBe("false");
    });

    it("IntersectionObserver callback triggers loadNext when sentinel intersects", async () => {
        hoisted.getAll
            .mockResolvedValueOnce(pagePayload(1, 10, [1,2,3,4,5,6,7,8,9,10]))
            .mockResolvedValueOnce(pagePayload(2, 10, [11]));

        render(<Host amount={10} />);
        await flush();

        act(() => {
            lastObserver?.cb([{ isIntersecting: true, target: document.body } as any]);
        });
        await flush();

        expect(hoisted.getAll).toHaveBeenCalledTimes(2);
        const last = hoisted.getAll.mock.calls.at(-1)!;
        expect(last[0]).toBe(2);
        expect(last[1]).toBe(10);
        expect(readJSON("ids")).toEqual(["1","2","3","4","5","6","7","8","9","10","11"]);
        expect(read("page")).toBe("2");
    });
});
