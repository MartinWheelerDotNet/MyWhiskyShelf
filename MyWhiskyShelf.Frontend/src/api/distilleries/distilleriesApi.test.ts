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

    it("calls axiosClient.get with default page/amount and returns the paged response", async () => {
        const payload = {
            items: [{ id: "1", name: "A" }],
            page: 1,
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
                params: { page: 1, amount: 10 },
            })
        );

        expect(res).toEqual(payload);
    });

    it("passes through explicit page and amount (and signal) to axios", async () => {
        const ac = new AbortController();
        const payload = { items: [], page: 2, amount: 20 };
        getMock.mockResolvedValue({ data: payload });

        const { getAllDistilleries } = await import("./distilleriesApi");
        const res = await getAllDistilleries(2, 20, ac.signal);

        const [url, opts] = getMock.mock.calls[0];
        expect(url).toBe("/distilleries");
        expect(opts).toEqual(
            expect.objectContaining({
                params: { page: 2, amount: 20 },
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
                params: { page: 1, amount: 10 },
            })
        );
    });
});