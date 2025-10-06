import { describe, it, expect, beforeEach, vi } from "vitest";

const getMock = vi.fn();

vi.mock("./axiosClient", () => ({
    axiosClient: {
        get: getMock,
    },
}));

describe("distilleriesApi - getAllDistilleries", () => {
    beforeEach(() => {
        vi.resetModules();
        getMock.mockReset();
    });

    it("calls axiosClient.get with the correct path and returns the data", async () => {
        const sample = [
            {
                id: "1",
                name: "Glen Example",
                country: "Scotland",
                region: "Speyside",
                founded: 1890,
                owner: "Example Spirits",
                type: "Single Malt",
                description: "A classic Speyside profile.",
                tastingNotes: "Honey, apple, vanilla",
                flavourProfile: { sweet: 7, smoky: 2, fruity: 6, spicy: 3, malty: 5 },
                active: true,
            },
        ] as const;

        getMock.mockResolvedValue({ data: sample });

        const { getAllDistilleries } = await import("./distilleriesApi");
        const result = await getAllDistilleries();

        expect(getMock).toHaveBeenCalledTimes(1);
        expect(getMock).toHaveBeenCalledWith("/distilleries");
        expect(result).toEqual(sample);
    });

    it("returns an empty array when the API responds with an empty list", async () => {
        getMock.mockResolvedValue({ data: [] });

        const { getAllDistilleries } = await import("./distilleriesApi");
        const result = await getAllDistilleries();

        expect(Array.isArray(result)).toBe(true);
        expect(result).toHaveLength(0);
    });

    it("propagates errors from axiosClient.get", async () => {
        const error = new Error("Network down");
        getMock.mockRejectedValue(error);

        const { getAllDistilleries } = await import("./distilleriesApi");

        await expect(getAllDistilleries()).rejects.toBe(error);
        expect(getMock).toHaveBeenCalledWith("/distilleries");
    });
});
