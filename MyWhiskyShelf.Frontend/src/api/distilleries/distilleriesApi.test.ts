import { describe, it, expect, beforeEach, vi } from "vitest";

const getMock = vi.fn();

vi.mock("../axiosClient", () => ({
    axiosClient: { get: getMock },
}));

describe("distilleriesApi.getAllDistilleries", () => {
    beforeEach(() => {
        vi.clearAllMocks();
        getMock.mockReset();
    });

    it("calls axiosClient.get with default amount and returns the cursor paged response", async () => {
        const payload = {
            items: [{ id: "1", name: "A" }],
            nextCursor: null,
            amount: 10,
        };
        getMock.mockResolvedValue({ data: payload });

        const { getAllDistilleries } = await import("./distilleriesApi");
        const res = await getAllDistilleries();

        expect(getMock).toHaveBeenCalledTimes(1);
        const [url, opts] = getMock.mock.calls[0];
        expect(url).toBe("/distilleries");
        expect(opts).toEqual(
            expect.objectContaining({
                // default request now uses { amount } only (no page)
                params: { amount: 10 },
            })
        );

        expect(res).toEqual(payload);
    });

    it("passes through explicit cursor and amount (and signal) to axios", async () => {
        const ac = new AbortController();
        const payload = { items: [], nextCursor: "NEXT123", amount: 20 };
        getMock.mockResolvedValue({ data: payload });

        const { getAllDistilleries } = await import("./distilleriesApi");
        // new signature: pass an options object with cursor/amount/signal
        const res = await getAllDistilleries({ cursor: "CURSOR123", amount: 20, signal: ac.signal });

        const [url, opts] = getMock.mock.calls[0];
        expect(url).toBe("/distilleries");
        expect(opts).toEqual(
            expect.objectContaining({
                params: { cursor: "CURSOR123", amount: 20 },
                signal: ac.signal,
            })
        );
        expect(res).toEqual(payload);
    });

    it("propagates axios errors", async () => {
        const boom = new Error("Network down");
        getMock.mockRejectedValue(boom);

        const { getAllDistilleries } = await import("./distilleriesApi");
        await expect(getAllDistilleries()).rejects.toBe(boom);

        expect(getMock).toHaveBeenCalledTimes(1);
        const [url, opts] = getMock.mock.calls[0];
        expect(url).toBe("/distilleries");
        expect(opts).toEqual(
            expect.objectContaining({
                // default request: { amount } only
                params: { amount: 10 },
            })
        );
    });
});
