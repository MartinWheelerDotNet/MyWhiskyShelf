import { Stack, TextField, MenuItem, Typography } from "@mui/material";

export type DistilleriesToolbarProps = {
    title?: string;
    query: string;
    onQueryChange: (value: string) => void;
    country: string | "all";
    onCountryChange: (value: string | "all") => void;
    region: string | "all";
    onRegionChange: (value: string | "all") => void;
    countryOptions?: Array<{ value: string; label: string }>;
    regionOptions?: Array<{ value: string; label: string }>;
};

export default function DistilleriesToolbar(
    {
        title = "Distilleries",
        query,
        onQueryChange,
        country,
        onCountryChange,
        region,
        onRegionChange,
        countryOptions = [],
        regionOptions = []
    }: Readonly<DistilleriesToolbarProps>) {
    
    return (
        <Stack
            direction={{ xs: "column", sm: "row" }}
            gap={2}
            alignItems={{ xs: "stretch", sm: "center" }}
            justifyContent="space-between"
            sx={{ mb: 3 }}
        >
            <Typography variant="h5" fontWeight={800}>
                {title}
            </Typography>

            <Stack direction="row" gap={2} alignItems="center" flexWrap="wrap">
                <TextField
                    size="small"
                    label="Search"
                    value={query}
                    onChange={(e) => onQueryChange(e.target.value)}
                />

                <TextField
                    size="small"
                    label="Country"
                    select
                    value={country}
                    onChange={(e) => onCountryChange(e.target.value as any)}
                    sx={{ minWidth: 180 }}
                >
                    <MenuItem value="all">All</MenuItem>
                    {countryOptions.map((o) => (
                        <MenuItem key={o.value} value={o.value}>
                            {o.label}
                        </MenuItem>
                    ))}
                </TextField>

                <TextField
                    size="small"
                    label="Region"
                    select
                    value={region}
                    onChange={(e) => onRegionChange(e.target.value as any)}
                    sx={{ minWidth: 180 }}
                >
                    <MenuItem value="all">All</MenuItem>
                    {regionOptions.map((o) => (
                        <MenuItem key={o.value} value={o.value}>
                            {o.label}
                        </MenuItem>
                    ))}
                </TextField>
            </Stack>
        </Stack>
    );
}