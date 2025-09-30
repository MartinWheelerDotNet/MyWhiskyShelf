import {
    AppBar,
    Toolbar,
    Typography,
    Box
} from "@mui/material";


import HeaderActions from "./components/HeaderActions.tsx";
import MainContent from "./components/MainContent.tsx";
import ThemeToggleSwitchGradients from "./components/ThemeToggleSwitch.tsx";

export default function App() {
    return (
        <Box sx={{ minHeight: "100vh" }}>
            <AppBar position="static" elevation={1}>
                <Toolbar>
                    <Typography variant="h6" sx={{ flexGrow: 1 }}>
                        MyWhiskyShelf
                    </Typography>
                    <ThemeToggleSwitchGradients />
                    <HeaderActions />
                </Toolbar>
            </AppBar>
            <MainContent />
        </Box>
    );
}