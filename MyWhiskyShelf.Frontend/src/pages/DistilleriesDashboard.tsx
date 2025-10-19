import { Container, Typography } from "@mui/material";

// @ts-ignore
import DistilleriesToolbar from "@/components/distilleries/DistilleriesToolbar";
// @ts-ignore
import { useInfiniteDistilleries } from "@/hooks/distilleries/useInfiniteDistilleries";
// @ts-ignore
import { useDistilleryFilters } from "@/hooks/distilleries/useDistilleryFilters"
// @ts-ignore
import DistilleriesList from "@/components/distilleries/DistilleriesList";
// @ts-ignore
import LoadingPill from "@/components/utils/LoadingPill";
// @ts-ignore
import { mapToDistilleryCardProps } from "@/lib/mappers/distillery";
// @ts-ignore
import { Distillery } from "@/lib/domain/types";

export default function DistilleriesDashboard() {
    const {
        items,
        loading,          // <-- use this for initial skeletons
        isPageLoading,
        error,
        hasMore,
        refresh,
        setSentinel,
    } = useInfiniteDistilleries({ initialAmount: 10 }); // <-- updated signature

    const {
        query, setQuery,
        country, setCountry,
        region, setRegion,
        countryOptions,
        regionOptions,
        filteredItems
    } = useDistilleryFilters(items);

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
                items={filteredItems as Distillery[]} // pass raw items; list maps to card props internally
                getId={(x: any) => String(x?.id ?? x?.Id ?? x?.distilleryId ?? "")}
                initialLoading={loading && items.length === 0}  // <-- initial load uses `loading`
                showMoreSkeletons={hasMore}
                initialSkeletonCount={6}
                moreSkeletonCount={3}
                setSentinel={setSentinel}
            />

            <LoadingPill loading={isPageLoading} hasMore={hasMore} />
        </Container>
    );
}