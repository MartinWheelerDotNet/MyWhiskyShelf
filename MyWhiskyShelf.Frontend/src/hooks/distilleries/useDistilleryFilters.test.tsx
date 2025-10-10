import { describe, it, expect } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";

// @ts-ignore
import { useDistilleryFilters } from "@/hooks/distilleries/useDistilleryFilters";

type D = { id: string; name: string; country?: string; region?: string };

const FIXTURE: D[] = [
    { id: "1", name: "Ardbeg", country: "Scotland", region: "Islay" },
    { id: "2", name: "Lagavulin", country: "Scotland", region: "Islay" },
    { id: "3", name: "Glenmorangie", country: "Scotland", region: "Highland" },
    { id: "4", name: "Redbreast", country: "Ireland", region: "Leinster" },
    { id: "5", name: "Hakushu", country: "Japan", region: "Chūbu" },
    { id: "6", name: "Test A", country: "Scotland", region: "Highland" },
    { id: "7", name: "Test B", country: "", region: "" },
];

function Host({ items }: { items: D[] }) {
    const {
        query, setQuery,
        country, setCountry,
        region, setRegion,
        countryOptions,
        regionOptions,
        filteredItems,
        resetFilters,
    } = useDistilleryFilters(items);

    return (
        <div>
            <div data-testid="query">{query}</div>
            <div data-testid="country">{country}</div>
        <div data-testid="region">{region}</div>

        <div data-testid="countryOptions">
        {JSON.stringify(countryOptions)}
        </div>
        <div data-testid="regionOptions">
        {JSON.stringify(regionOptions)}
        </div>

        <div data-testid="filtered">
        {JSON.stringify(filteredItems.map((x: { name: string; }) => x.name))}
        </div>

        <button data-testid="setQuery-isla" onClick={() => setQuery("isla")} />
        <button data-testid="setQuery-glen" onClick={() => setQuery("glen")} />
        <button data-testid="setCountry-scotland" onClick={() => setCountry("scotland")} />
        <button data-testid="setCountry-ireland" onClick={() => setCountry("ireland")} />
        <button data-testid="setRegion-islay" onClick={() => setRegion("islay")} />
        <button data-testid="setRegion-highland" onClick={() => setRegion("highland")} />
        <button data-testid="setRegion-leinster" onClick={() => setRegion("leinster")} />
        <button data-testid="setRegion-all" onClick={() => setRegion("all")} />
        <button data-testid="reset" onClick={() => resetFilters()} />
    </div>
);
}

const readJson = (testId: string) =>
    JSON.parse(screen.getByTestId(testId).textContent || "null");

describe("useDistilleryFilters", () => {
    it("initial state: empty query, 'all' filters; options are de-duped & sorted with 'All'", () => {
        render(<Host items={FIXTURE} />);

        expect(screen.getByTestId("query").textContent).toBe("");
        expect(screen.getByTestId("country").textContent).toBe("all");
        expect(screen.getByTestId("region").textContent).toBe("all");

        const countryOptions = readJson("countryOptions");
        const regionOptions = readJson("regionOptions");

        // Country options start with All
        expect(countryOptions[0]).toEqual({ value: "all", label: "All" });
        // Labels keep original case; values lowercased
        expect(countryOptions.map((o: any) => o.label)).toEqual(["All", "Ireland", "Japan", "Scotland"]);
        expect(countryOptions.map((o: any) => o.value)).toEqual(["all", "ireland", "japan", "scotland"]);

        // Region options start with All; sorted & de-duped (empty ones removed)
        expect(regionOptions[0]).toEqual({ value: "all", label: "All" });
        expect(regionOptions.map((o: any) => o.label)).toEqual(["All", "Chūbu", "Highland", "Islay", "Leinster"]);
        expect(regionOptions.map((o: any) => o.value)).toEqual(["all", "chūbu", "highland", "islay", "leinster"]);

        // By default filteredItems === all items
        const filteredNames = readJson("filtered");
        expect(filteredNames).toEqual(FIXTURE.map(x => x.name));
    });

    it("query is case-insensitive and matches name/region/country", () => {
        render(<Host items={FIXTURE} />);

        // "isla" matches region "Islay" → Ardbeg + Lagavulin
        fireEvent.click(screen.getByTestId("setQuery-isla"));
        expect(readJson("filtered")).toEqual(["Ardbeg", "Lagavulin"]);

        // "glen" matches name "Glenmorangie"
        fireEvent.click(screen.getByTestId("setQuery-glen"));
        expect(readJson("filtered")).toEqual(["Glenmorangie"]);
    });

    it("filters by country value (lowercased) and region value (lowercased)", () => {
        render(<Host items={FIXTURE} />);


        fireEvent.click(screen.getByTestId("setCountry-scotland"));
        expect(readJson("filtered").sort()).toEqual(
            ["Ardbeg", "Lagavulin", "Glenmorangie", "Test A"].sort()
        );

        fireEvent.click(screen.getByTestId("setRegion-islay"));
        expect(readJson("filtered")).toEqual(["Ardbeg", "Lagavulin"]);

        fireEvent.click(screen.getByTestId("setRegion-highland"));
        expect(readJson("filtered").sort()).toEqual(["Glenmorangie", "Test A"].sort());

        fireEvent.click(screen.getByTestId("setCountry-ireland"));
        fireEvent.click(screen.getByTestId("setRegion-all"));
        expect(readJson("filtered")).toEqual(["Redbreast"]);
    });

    it("resetFilters clears query and sets country/region back to 'all'", () => {
        render(<Host items={FIXTURE} />);

        fireEvent.click(screen.getByTestId("setCountry-scotland"));
        fireEvent.click(screen.getByTestId("setRegion-islay"));
        fireEvent.click(screen.getByTestId("setQuery-glen")); // now query doesn't match any Islay item

        // Currently filtered should be empty
        expect(readJson("filtered")).toEqual([]);

        // Reset
        fireEvent.click(screen.getByTestId("reset"));

        expect(screen.getByTestId("query").textContent).toBe("");
        expect(screen.getByTestId("country").textContent).toBe("all");
        expect(screen.getByTestId("region").textContent).toBe("all");
        expect(readJson("filtered")).toEqual(FIXTURE.map(x => x.name));
    });

    it("recomputes options and filtered items when items prop changes", () => {
        const { rerender } = render(<Host items={FIXTURE} />);

        // Start with existing options
        expect(readJson("countryOptions").map((o: any) => o.label)).toEqual(["All", "Ireland", "Japan", "Scotland"]);

        // New items set (e.g., only Japan)
        const NEXT: D[] = [
            { id: "a", name: "Yoichi", country: "Japan", region: "Hokkaidō" },
            { id: "b", name: "Yamazaki", country: "Japan", region: "Kansai" },
        ];

        rerender(<Host items={NEXT} />);

        expect(readJson("countryOptions").map((o: any) => o.label)).toEqual(["All", "Japan"]);
        expect(readJson("regionOptions").map((o: any) => o.label)).toEqual(["All", "Hokkaidō", "Kansai"]);
        expect(readJson("filtered")).toEqual(["Yoichi", "Yamazaki"]);
    });
});
