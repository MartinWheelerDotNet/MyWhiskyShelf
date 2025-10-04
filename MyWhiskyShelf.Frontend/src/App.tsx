import {
    AppBar,
    Toolbar,
    Box
} from "@mui/material";

import AccountMenuActions from "./components/AccountMenuActions.tsx";
import MainContent from "./components/MainContent.tsx";
import ThemeToggle from "./components/ThemeToggle.tsx";
import ThemeToggleMobile from "./components/ThemeToggleMobile.tsx";

export default function App() {
    return (
        <Box sx={{ minHeight: "100vh" }}>
            <AppBar position="static" elevation={1} color="transparent">
                <Toolbar sx={{ bgcolor: "transparent", position: "relative" }} variant="dense">
                    <Box
                        component="img"
                        src="/media/images/mywhiskyshelf-logo-horizontal.png"
                        alt="MyWhiskyShelf Logo"
                        sx={(theme) => ({
                            position: "absolute",
                            left: 0,
                            pl: 2,
                            height: 32,
                            objectFit: "contain",
                            pointerEvents: "none",
                            userSelect: "none",
                            filter:
                                theme.palette.mode === "dark"
                                    ? "brightness(1) invert(1)"
                                    : "none",
                        })}
                    />

                    
                    <Box sx={{ ml: "auto", display: "flex", alignItems: "center", gap: 0.5 }}>
                        <Box sx={{ display: { xs: "inline-flex", sm: "none" } }}>
                            <ThemeToggleMobile />
                        </Box>

                        <Box sx={{ display: { xs: "none", sm: "inline-flex" } }}>
                            <ThemeToggle />
                        </Box>

                        <AccountMenuActions />
                    </Box>
                </Toolbar>
            </AppBar>
            <MainContent />
        </Box>
    );
}