import { Box, Link } from "@mui/material";
// @ts-ignore
import MainContent from "@/components/MainContent";
// @ts-ignore
import HeaderBar from "@/components/header/HeaderBar";

export default function App() {
    return (
        <Box sx={{ minHeight: "100vh", display: "flex", flexDirection: "column" }}>
            {/* Skip link for keyboard users */}
            <Link
                href="#main"
                underline="none"
                sx={{
                    position: "absolute",
                    left: -9999,
                    top: 0,
                    p: 1,
                    bgcolor: "background.paper",
                    borderRadius: 1,
                    "&:focus": { left: 8, top: 8, zIndex: 1200, boxShadow: 3 },
                }}
            >
                Skip to content
            </Link>
            <HeaderBar />
            <Box
                component="main"
                id="main"
                sx={{ outline: 0, flex: 1 }}
            >
                <MainContent />
            </Box>
        </Box>
    );
}
