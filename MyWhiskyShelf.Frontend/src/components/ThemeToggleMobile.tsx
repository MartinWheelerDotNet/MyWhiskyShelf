import { IconButton, Tooltip, type SxProps } from "@mui/material";
import DarkModeRoundedIcon from "@mui/icons-material/DarkModeRounded";
import LightModeRoundedIcon from "@mui/icons-material/LightModeRounded";
import { useTheme } from "@mui/material/styles";
import { useThemeMode } from "../theme/ThemeModeProvider";

export default function ThemeToggleMobile({ sx }: Readonly<{ sx?: SxProps }>) {
    const theme = useTheme();
    const { toggleMode } = useThemeMode();
    const isDark = theme.palette.mode === "dark";

    return (
        <Tooltip title={isDark ? "Switch to light mode" : "Switch to dark mode"}>
            <IconButton color="inherit" onClick={toggleMode} sx={sx} aria-label="Toggle color scheme">
                {isDark 
                    ? <DarkModeRoundedIcon sx={{ color: "white" }}/>
                    : <LightModeRoundedIcon sx={{ color: "yellow" }}/>
                } 
            </IconButton>
        </Tooltip>
    );
}