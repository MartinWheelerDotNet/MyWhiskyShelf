import * as React from "react";
import {
    Box,
    Card,
    CardHeader,
    CardContent,
    Avatar,
    IconButton,
    Chip,
    Collapse,
    Divider,
    Typography,
    Tooltip,
    Stack,
    alpha, 
    type IconButtonProps
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import FavoriteIcon from "@mui/icons-material/FavoriteBorder";
import LocalDrinkOutlinedIcon from "@mui/icons-material/LocalDrinkOutlined";
import PublicOutlinedIcon from "@mui/icons-material/PublicOutlined";
import PlaceOutlinedIcon from "@mui/icons-material/PlaceOutlined";
import { styled } from "@mui/material/styles";

export type DistilleryCardProps = Readonly<{
    id: string;
    name: string;
    region?: string;
    country?: string;
    founded?: number | string;
    isFavorite?: boolean;
    logoUrl?: string;
    onToggleFavorite?: (id: string) => void;
    about?: string;
    notes?: string;
    whiskiesCount?: number;
}>;

type ExpandMoreProps = {
    expand?: boolean;
} & Omit<IconButtonProps, "color">;

const ExpandMore = styled((props: ExpandMoreProps) => {
    const { expand, ...other } = props;
    return <IconButton {...other} />;
})(({ theme, expand }: any) => ({
    transform: expand ? "rotate(180deg)" : "rotate(0deg)",
    transition: theme.transitions.create("transform", {
        duration: theme.transitions.duration.shortest,
    }),
}));

export default function DistilleryCard(
    {
        id, 
        name,
        region,
        country,
        founded,
        isFavorite = false,
        logoUrl,
        onToggleFavorite,
        about,
        notes,
        whiskiesCount 
    }: DistilleryCardProps) {
    
    const [expanded, setExpanded] = React.useState(false);

    return (
        <Card
            elevation={8}
            sx={(theme) => ({
                borderRadius: 3,
                overflow: "hidden",
                bgcolor: alpha(theme.palette.background.paper, 0.9),
                backdropFilter: "blur(6px)",
                border: `2px solid ${alpha(theme.palette.divider, 1)}`,
                boxShadow: `0 8px 30px ${alpha(theme.palette.common.black, 0.25)}`,
                transition: "transform 150ms ease, box-shadow 200ms ease",
                "&:hover": {
                    transform: "translateY(-2px)",
                    boxShadow: `0 16px 36px ${alpha(theme.palette.common.black, 0.35)}`,
                },
            })}
        >
            <CardHeader
                avatar={
                    <Avatar
                        alt={name}
                        src={logoUrl}
                        sx={(theme) => ({
                            bgcolor: alpha(theme.palette.primary.main, 0.25),
                            border: `1px solid ${alpha(theme.palette.primary.main, 0.6)}`,
                        })}
                    >
                        {name.slice(0, 1)}
                    </Avatar>
                }
                slotProps={{
                    title: { sx: { fontWeight: 700 } },
                    subheader: { component: 'div' }
                }}
                title={name}
                subheader={
                    <Stack paddingTop={1} direction="row" gap={1} flexWrap="wrap" alignItems="center">
                        {country && (
                            <Chip
                                size="small"
                                variant="outlined"
                                icon={<PublicOutlinedIcon fontSize="small" />}
                                label={country}
                                sx={{ borderRadius: 999 }}
                            />
                        )}
                        {region && (
                            <Chip
                                size="small"
                                variant="outlined"
                                icon={<PlaceOutlinedIcon fontSize="small" />}
                                label={region}
                                sx={{ borderRadius: 999 }}
                            />
                        )}
                        {typeof whiskiesCount === "number" && (
                            <Chip
                                size="small"
                                color="primary"
                                icon={<LocalDrinkOutlinedIcon fontSize="small" />}
                                label={`${whiskiesCount} whisk${whiskiesCount === 1 ? "y" : "ies"}`}
                                sx={{ borderRadius: 999 }}
                            />
                        )}
                    </Stack>
                }
                action={
                    <Stack direction="row" alignItems="center">
                        <Tooltip title={isFavorite ? "Unfavorite" : "Add to favorites"}>
                            <IconButton
                                aria-label="toggle favorite"
                                onClick={() => onToggleFavorite?.(id)}
                            >
                                <FavoriteIcon color={isFavorite ? "error" : "inherit"} />
                            </IconButton>
                        </Tooltip>
                        <Tooltip title={expanded ? "Collapse" : "Expand"}>
                            <ExpandMore
                                expand={expanded}
                                aria-expanded={expanded}
                                aria-label="show more"
                                onClick={() => setExpanded((v) => !v)}
                            >
                                <ExpandMoreIcon />
                            </ExpandMore>
                        </Tooltip>
                    </Stack>
                }
            />

            <Collapse in={expanded} timeout="auto" unmountOnExit>
                <Divider sx={{ opacity: 0.2 }} />
                <CardContent>
                    <Stack spacing={3}>
                        <Section title="About">
                            <Typography variant="overline" sx={{ whiteSpace: "pre-wrap" }}>
                                <b>Founded:</b> {founded ?? "-"}
                            </Typography>
                            <Typography variant="body2" sx={{ whiteSpace: "pre-wrap" }}>
                                {about ?? "—"}
                            </Typography>
                        </Section>

                        <Section title="Tasting Notes">
                            <Typography variant="body2" sx={{ whiteSpace: "pre-wrap" }}>
                                {notes ?? "—"}
                            </Typography>
                        </Section>

                        <Section title="My Bottles">
                            <Typography variant="body2" color="text.secondary">
                                This is a list of my bottles which will be retrieved from the API.
                            </Typography>
                        </Section>
                    </Stack>
                </CardContent>
            </Collapse>
        </Card>
    );
}

function Section({ title, children }: React.PropsWithChildren<{ title: string }>) {
    return (
        <Box>
            <Typography variant="subtitle2" sx={{ fontWeight: 700, mb: 1 }}>
                {title}
            </Typography>
            {children}
        </Box>
    );
}