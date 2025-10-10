import * as React from "react";
// @ts-ignore
import {Distillery} from "@/lib/domain/types";

export type SelectOption = { value: string; label: string };

function norm(v: string) {
    return String(v).trim();
}
function normLower(v: string) {
    return norm(v).toLowerCase();
}

function uniqueSorted(values: (string | undefined | null)[]) {
    return Array.from(new Set(values.filter(Boolean).map((v) => norm(v!)))).sort(
        (a, b) => a.localeCompare(b)
    );
}

export function useDistilleryFilters<T extends { name?: string; region?: string; country?: string }>(
    items: T[],
    defaults?: { country?: string; region?: string; query?: string }
) {
    const [query, setQuery] = React.useState(defaults?.query ?? "");
    const [country, setCountry] = React.useState<string>(defaults?.country ?? "all");
    const [region, setRegion] = React.useState<string>(defaults?.region ?? "all");

    const countryOptions: SelectOption[] = React.useMemo(() => {
        const opts = uniqueSorted(items.map((d) => d.country));
        return [{ value: "all", label: "All" }, ...opts.map((c) => ({ value: c.toLowerCase(), label: c }))];
    }, [items]);

    const regionOptions: SelectOption[] = React.useMemo(() => {
        const opts = uniqueSorted(items.map((d) => d.region));
        return [{ value: "all", label: "All" }, ...opts.map((r) => ({ value: r.toLowerCase(), label: r }))];
    }, [items]);

    const filteredItems: Distillery = React.useMemo(() => {
        const normalQuery = normLower(query);
        const countryFilter = country;
        const regionFilter = region;

        return items.filter((d) => {
            const normalDistilleryName = normLower(d.name as string);
            const normalDistilleryRegion = normLower(d.region as string);
            const normalDistilleryCountry = normLower(d.country as string);

            const okCountry = countryFilter === "all" || normalDistilleryCountry === countryFilter;
            const okRegion = regionFilter === "all" || normalDistilleryRegion === regionFilter;
            const okQuery = !normalQuery 
                || normalDistilleryName.includes(normalQuery) 
                || normalDistilleryRegion.includes(normalQuery) 
                || normalDistilleryCountry.includes(normalQuery);

            return okCountry && okRegion && okQuery;
        });
    }, [items, query, country, region]);

    const resetFilters = React.useCallback(() => {
        setQuery("");
        setCountry("all");
        setRegion("all");
    }, []);

    return {
        query, setQuery,
        country, setCountry,
        region, setRegion,
        countryOptions,
        regionOptions,
        filteredItems,
        resetFilters,
    };
}