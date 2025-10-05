import { AppBar, Toolbar, Box, Stack } from "@mui/material";
import { useTheme } from "@mui/material/styles";

// @ts-ignore
import AccountMenuActions from "@/components/AccountMenuActions";
// @ts-ignore
import ThemeToggle from "@/components/ThemeToggle";
// @ts-ignore
import ThemeToggleMobile from "@/components/ThemeToggleMobile";

export default function HeaderBar() {
    const theme = useTheme();

    const brand = theme.app?.brand ?? {};
    const layout = theme.app?.layout ?? {};

    const logoHeight = brand.logoHeight ?? 32;
    const logoSrc = brand.logoSrc ?? "/media/images/mywhiskyshelf-logo-horizontal.png";

    const logoFilter =
        theme.palette.mode === "dark"
            ? brand.logoFilterDark ?? "brightness(1) invert(1)"
            : brand.logoFilterLight ?? "none";

    return (
        <AppBar
            position={layout.appBarPosition ?? "static"}
            elevation={layout.appBarElevation ?? 1}
            color={layout.appBarColor ?? "transparent"}
            sx={{ bgcolor: layout.appBarBg ?? "transparent" }}
        >
            <Toolbar variant="dense" sx={{ position: "relative" }}>
                <Box
                    component="img"
                    src={logoSrc}
                    alt="MyWhiskyShelf"
                    sx={{
                        position: "absolute",
                        left: 0,
                        pl: 2,
                        height: logoHeight,
                        objectFit: "contain",
                        pointerEvents: "none",
                        userSelect: "none",
                        filter: logoFilter,
                    }}
                />

                <Stack direction="row" spacing={0.5} alignItems="center" sx={{ ml: "auto" }}>
                    <Box sx={{ display: { xs: "inline-flex", sm: "none" } }}>
                        <ThemeToggleMobile />
                    </Box>
                        <Box sx={{ display: { xs: "none", sm: "inline-flex" } }}>
                        <ThemeToggle />
                    </Box>
                    <AccountMenuActions />
                </Stack>
            </Toolbar>
        </AppBar>
    );
}
