import * as React from "react";
import {Container, Grid} from "@mui/material";
// @ts-ignore
import DistilleryCard, {DistilleryCardProps} from "@/components/DistilleryCard";
// @ts-ignore
import DistilleriesToolbar from "@/components/DistilleriesToolbar";

type FilterMenuOptions = {
    value: string;
    label: string;
}

const MOCK_DISTILLERY_CARD_PROPS: DistilleryCardProps[] = [
    {
        id: "ardbeg",
        name: "Ardbeg",
        region: "Islay",
        country: "Scotland",
        founded: 1815,
        isFavorite: true,
        whiskiesCount: 17,
        about: "Renowned for intensely peated single malts with maritime character.",
        notes: "Smoke, iodine, sea spray, citrus.",
        logoUrl: "/media/images/distilleries/ardbeg-logo.png",
    },
    {
        id: "yamazaki",
        name: "Yamazaki",
        region: "Kansai",
        country: "Japan",
        founded: 1923,
        whiskiesCount: 9,
        about: "Japan’s first and oldest malt whisky distillery; elegant, complex style.",
        logoUrl: "/media/images/distilleries/yamazaki-logo.png",
    },
];

const MOCK_COUNTRY_OPTIONS: FilterMenuOptions[] = [
    { 
        value: "japan", 
        label: "Japan" 
    },
    { 
        value: "scotland",
        label: "Scotland" 
    },
]

const MOCK_REGION_OPTIONS: FilterMenuOptions[] = [
    {
        value: "islay",
        label: "Islay"
    },
    {
        value: "kansai",
        label: "Kansai"
    },
]

export default function DistilleriesDashboard() {
    const [pattern, setPattern] = React.useState("");
    const [region, setRegion] = React.useState<string | "all">("all");
    const [country, setCountry] = React.useState<string | "all">("all");

    const filtered = React.useMemo(() => {
        const query = pattern.trim().toLowerCase();
        return MOCK_DISTILLERY_CARD_PROPS.filter((d) => {
            const okRegion = region === "all" || d.region?.toLowerCase() === region;
            const okCountry = country === "all" || d.country?.toLowerCase() === country;
            const okQuery =
                !query ||
                d.name.toLowerCase().includes(query) ||
                d.region?.toLowerCase().includes(query) ||
                d.country?.toLowerCase().includes(query);
            return okRegion && okCountry && okQuery;
        });
    }, [pattern, region, country]);

    return (
        <Container maxWidth="lg" sx={{ py: 3 }}>
            <DistilleriesToolbar
                title="Distilleries"
                query={pattern}
                onQueryChange={setPattern}
                country={country}
                onCountryChange={setCountry}
                region={region}
                onRegionChange={setRegion}
                countryOptions={MOCK_COUNTRY_OPTIONS}
                regionOptions={MOCK_REGION_OPTIONS}
            />
            <Grid container spacing={1}>
                {filtered.map((d) => (
                    <Grid key={d.id} size={{ xs: 12, sm: 6, md: 12 }}>
                        <DistilleryCard {...d} />
                    </Grid>
                ))}
            </Grid>
        </Container>
    );
}