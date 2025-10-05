import { IconButton, Tooltip, type SxProps } from "@mui/material";
import DarkModeRoundedIcon from "@mui/icons-material/DarkModeRounded";
import LightModeRoundedIcon from "@mui/icons-material/LightModeRounded";
import { useTheme } from "@mui/material/styles";
// @ts-ignore
import { useThemeMode } from "@/theme/ThemeModeProvider";

export default function ThemeToggleMobile({ sx }: Readonly<{ sx?: SxProps }>) {
    const theme = useTheme();
    const { toggleMode } = useThemeMode();
    const isDark = theme.palette.mode === "dark";
    const mt = (theme as any).app?.controls?.modeToggle ?? {};
    const targetIconColor = isDark 
        ? mt.palette?.lightThumb ?? mt.thumb ?? "#ffffff" 
        : mt.palette?.darkThumb  ?? mt.thumb ?? "#ffffff";

    return (
        <Tooltip title={isDark ? "Switch to light mode" : "Switch to dark mode"}>
            <IconButton
                color="inherit"
                onClick={toggleMode}
                sx={sx}
                aria-label="Toggle color scheme"
            >
                {isDark ? (
                    <LightModeRoundedIcon sx={{ color: targetIconColor }} />
                ) : (
                    <DarkModeRoundedIcon sx={{ color: targetIconColor }} />
                )}
            </IconButton>
        </Tooltip>
    );
}
