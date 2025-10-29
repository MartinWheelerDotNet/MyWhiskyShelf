import * as React from "react";
import { Container, Typography } from "@mui/material";
// @ts-ignore
import DistilleriesToolbar from "@/components/distilleries/DistilleriesToolbar";
// @ts-ignore
import { useInfiniteDistilleries } from "@/hooks/distilleries/useInfiniteDistilleries";
// @ts-ignore
import { useDistilleryFilters } from "@/hooks/distilleries/useDistilleryFilters";
// @ts-ignore
import DistilleriesList from "@/components/distilleries/DistilleriesList";
// @ts-ignore
import LoadingPill from "@/components/utils/LoadingPill";
// @ts-ignore
import { Distillery } from "@/lib/domain/types";
// @ts-ignore
import { useGeoData } from "@/hooks/geo/useGeoData";

function normLower(v: string) {
    return String(v ?? "").trim().toLowerCase();
}

function useDebounced<T>(value: T, delay: number) {
    const [debounced, setDebounced] = React.useState(value);
    React.useEffect(() => {
        const id = setTimeout(() => setDebounced(value), delay);
        return () => clearTimeout(id);
    }, [value, delay]);
    return debounced;
}

export default function DistilleriesDashboard() {
    const { countries = [], regions = [] } = useGeoData() as any;

    const {
        query, setQuery,
        country, setCountry,
        region, setRegion
    } = useDistilleryFilters([]);

    const countryOptions = React.useMemo(
        () =>
            countries
                .slice()
                .sort((a: any, b: any) => a.name.localeCompare(b.name))
                .map((c: any) => ({ value: normLower(c.name), label: c.name })),
        [countries]
    );

    const selectedCountryId = React.useMemo(() => {
        if (country === "all") return null;
        const match = countries.find((c: any) => normLower(c.name) === country);
        return match ? match.id : null;
    }, [countries, country]);

    const regionOptions = React.useMemo(() => {
        const base = regions
            .filter((r: any) => r.isActive && (selectedCountryId ? r.countryId === selectedCountryId : true))
            .slice()
            .sort((a: any, b: any) => a.name.localeCompare(b.name));
        return base.map((r: any) => ({ value: normLower(r.name), label: r.name }));
    }, [regions, selectedCountryId]);

    const selectedRegionId = React.useMemo(() => {
        if (region === "all" || !selectedCountryId) return null;
        const match = regions.find((r: any) => r.countryId === selectedCountryId && normLower(r.name) === region);
        return match ? match.id : null;
    }, [regions, selectedCountryId, region]);

    const debouncedQuery = useDebounced(String(query ?? "").trim(), 350);
    const pattern = React.useMemo(() => {
        return debouncedQuery.length > 3 ? debouncedQuery : undefined;
    }, [debouncedQuery]);

    const {
        items,
        loading,
        isPageLoading,
        error,
        hasMore,
        refresh,
        setSentinel,
    } = useInfiniteDistilleries({
        initialAmount: 10,
        countryId: selectedCountryId ?? undefined,
        regionId: selectedRegionId ?? undefined,
        pattern
    });

    if (error) {
        return (
            <Container maxWidth="lg" sx={{ py: 3 }}>
                <Typography color="error" sx={{ mb: 1 }}>Couldn’t load distilleries.</Typography>
                <button onClick={refresh}>Retry</button>
            </Container>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ py: 3 }}>
            <DistilleriesToolbar
                title="Distilleries"
                query={query}
                onQueryChange={setQuery}
                country={country}
                onCountryChange={setCountry}
                region={region}
                onRegionChange={setRegion}
                countryOptions={countryOptions}
                regionOptions={regionOptions}
            />

            <DistilleriesList
                items={items as Distillery[]}
                initialLoading={loading && items.length === 0}
                showMoreSkeletons={hasMore}
                initialSkeletonCount={6}
                moreSkeletonCount={3}
                setSentinel={setSentinel}
            />

            <LoadingPill loading={isPageLoading} hasMore={hasMore} />
        </Container>
    );
}
