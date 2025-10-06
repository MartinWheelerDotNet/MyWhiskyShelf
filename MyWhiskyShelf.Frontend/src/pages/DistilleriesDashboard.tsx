import * as React from "react";
import { Container, Grid, Typography } from "@mui/material";
import Button from "@mui/material/Button";
// @ts-ignore
import DistilleryCard from "@/components/DistilleryCard";
// @ts-ignore
import DistilleriesToolbar from "@/components/DistilleriesToolbar";
// @ts-ignore
import { useFetchAllDistilleries } from "@/hooks/useFetchAllDistilleries";
// @ts-ignore
import { toCardProps } from "@/hooks/mapDistillery";

export default function DistilleriesDashboard() {
    const { data, loading, error, refresh } = useFetchAllDistilleries();

    const [query, setQuery] = React.useState("");
    const [region, setRegion] = React.useState<string>("all");
    const [country, setCountry] = React.useState<string>("all");

    const cards = React.useMemo(() => data.map(toCardProps), [data]);

    const filtered = React.useMemo(() => {
        const q = query.trim().toLowerCase();
        return cards.filter((d : any) => {
            const okRegion = region === "all" || d.region?.toLowerCase() === region;
            const okCountry = country === "all" || d.country?.toLowerCase() === country;
            const okQuery =
                !q ||
                d.name.toLowerCase().includes(q) ||
                d.region?.toLowerCase().includes(q) ||
                d.country?.toLowerCase().includes(q);
            return okRegion && okCountry && okQuery;
        });
    }, [query, region, country, cards]);

    const countryOptions = React.useMemo(
        () => [
            { value: "all", label: "All" },
            ...Array.from(new Set(cards.map((d : any) => d.country).filter(Boolean))).map((c) => ({
                value: String(c).toLowerCase(),
                label: String(c),
            })),
        ],
        [cards]
    );

    const regionOptions = React.useMemo(
        () => [
            { value: "all", label: "All" },
            ...Array.from(new Set(cards.map((d : any) => d.region).filter(Boolean))).map((r) => ({
                value: String(r).toLowerCase(),
                label: String(r),
            })),
        ],
        [cards]
    );

    if (loading) {
        return (
            <Container maxWidth="lg" sx={{ py: 3 }}>
                <Typography>Loading distilleries…</Typography>
            </Container>
        );
    }

    if (error) {
        return (
            <Container maxWidth="lg" sx={{ py: 3 }}>
                <Typography color="error" sx={{ mb: 2 }}>
                    Failed to load distilleries.
                </Typography>
                <Typography variant="body2" sx={{ mb: 2 }}>
                    {(error as any)?.message ?? "Please try again."}
                </Typography>
                <Button
                    variant="text"
                    type="button"
                    onClick={refresh}
                    sx={{ textDecoration: "underline", p: 0, minWidth: 0, textTransform: "none" }}
                >
                    Retry
                </Button>
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

            <Grid container spacing={1}>
                {filtered.map((d : any) => (
                    <Grid key={d.id} size={{ xs: 12, sm: 6, md: 12 }}>
                        <DistilleryCard {...d} />
                    </Grid>
                ))}
            </Grid>
        </Container>
    );
}
