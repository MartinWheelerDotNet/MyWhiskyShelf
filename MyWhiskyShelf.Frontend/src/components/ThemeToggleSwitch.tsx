
import { Box, Tooltip, type SxProps } from "@mui/material";
import { styled, useTheme } from "@mui/material/styles";
import Switch from "@mui/material/Switch";
import LightModeRoundedIcon from "@mui/icons-material/LightModeRounded";
import DarkModeRoundedIcon from "@mui/icons-material/DarkModeRounded";
import { useThemeMode } from "../theme/ThemeModeProvider";

type Props = { sx?: SxProps };

const W = 80;      
const H = 46;      
const P = 10;      
const THUMB = 20;  
const START_X = 12; 
const TRAVEL_X = (W - P * 2) - THUMB - 4;

const GradientSwitch = styled(Switch)(({ theme }) => {
    const isDark = theme.palette.mode === "dark";
    const lightTrack = "linear-gradient(90deg, #ff8ba7 0%, #f0b429 100%)";
    const darkTrack  = "linear-gradient(90deg, #34b1eb 0%, #0e4f6e 100%)";

    return {
        width: W,
        height: H,
        padding: P,
        position: "relative",

        "& .MuiSwitch-switchBase": {
            padding: 0,
            margin: 0,
            left: 0,
            top: "50%",
            transform: `translate(${START_X}px, -50%)`,
            transition: "transform 180ms ease",
            "&.Mui-checked": {
                transform: `translate(${START_X + TRAVEL_X}px, -50%)`,
            }
        },

        "& .MuiSwitch-thumb": {
            width: THUMB,
            height: THUMB,
            borderRadius: THUMB / 2,
            backgroundColor: "#e5e7eb"
        },

        "& .MuiSwitch-track": {
            borderRadius: 999,
            background: isDark ? darkTrack : lightTrack,
            border: 5 ,
            opacity: 1,
        },
    };
});

export default function ThemeToggleSwitchGradients({ sx }: Readonly<Props>) {
    const theme = useTheme();
    const { toggleMode } = useThemeMode();
    const isDark = theme.palette.mode === "dark";

    return (
        <Tooltip title={isDark ? "Switch to light mode" : "Switch to dark mode"}>
            <Box
                component="span"
                sx={{ position: "relative", width: W, height: H, display: "inline-flex", alignItems: "center", ...sx }}
            >
                <GradientSwitch
                    checked={isDark}
                    onChange={toggleMode}
                    slotProps={{ input: { "aria-label": "Toggle color scheme" } }}
                />

                <LightModeRoundedIcon
                    sx={{
                        position: "absolute",
                        left: 14,
                        top: "50%",
                        transform: "translateY(-50%)",
                        color: "#ffffff",
                        opacity: isDark ? 0.55 : 1,
                        pointerEvents: "none",
                        fontSize: 16,
                    }}
                />
                <DarkModeRoundedIcon
                    sx={{
                        position: "absolute",
                        right: 14,
                        top: "50%",
                        transform: "translateY(-50%)",
                        color: "#ffffff",
                        opacity: isDark ? 1 : 0.7,
                        pointerEvents: "none",
                        fontSize: 16,
                    }}
                />
            </Box>
        </Tooltip>
    );
}