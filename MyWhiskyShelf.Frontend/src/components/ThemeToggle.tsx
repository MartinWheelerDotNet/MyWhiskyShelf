import { Box, Tooltip, type SxProps, Switch } from "@mui/material";
import { styled, useTheme } from "@mui/material/styles";
import LightModeRoundedIcon from "@mui/icons-material/LightModeRounded";
import DarkModeRoundedIcon from "@mui/icons-material/DarkModeRounded";
// @ts-ignore
import { useThemeMode } from "@/theme/ThemeModeProvider";

type Props = { sx?: SxProps };

const GradientSwitch = styled(Switch)(({ theme }) => {
    const mode = theme.app?.controls?.modeToggle ?? {};
    const isDark = theme.palette.mode === "dark";

    const WIDTH = mode.width ?? 70;
    const HEIGHT = mode.height ?? 46;
    const PADDING = mode.padding ?? 10;
    const THUMB_SIZE = mode.thumbSize ?? 20;
    const LEFT_POS = mode.leftThumbPos ?? 12;
    const RIGHT_POS = (WIDTH - PADDING * 2) - THUMB_SIZE - 4;
    const TRACK_LIGHT = mode.track?.light ?? "linear-gradient(90deg, #f0b429 0%, #ff8ba7 100%)";
    const TRACK_DARK = mode.track?.dark  ?? "linear-gradient(90deg, #34b1eb 0%, #0e4f6e 100%)";

    return {
        width: WIDTH,
        height: HEIGHT,
        padding: PADDING,
        position: "relative",

        "& .MuiSwitch-switchBase": {
            padding: 0,
            margin: 0,
            left: 0,
            top: "50%",
            transform: `translate(${LEFT_POS}px, -50%)`,
            transition: theme.transitions.create("transform", {
                duration: theme.transitions.duration.shorter,
            }),
            "&.Mui-checked": {
                transform: `translate(${LEFT_POS + RIGHT_POS}px, -50%)`,
            },
        },

        "& .MuiSwitch-thumb": {
            width: THUMB_SIZE,
            height: THUMB_SIZE,
            borderRadius: THUMB_SIZE / 2,
            boxShadow: theme.shadows[2],
        },

        "& .MuiSwitch-track": {
            borderRadius: 999,
            background: isDark ? TRACK_DARK : TRACK_LIGHT,
            opacity: 1,
        },
    };
});

export default function ThemeToggle({ sx }: Readonly<Props>) {
    const theme = useTheme();
    const { toggleMode } = useThemeMode();
    const isDark = theme.palette.mode === "dark";
    const tokens = (theme as any).app?.controls?.modeToggle ?? {};
    const iconColor = tokens.icon ?? "#ffffff";
    const WIDTH = tokens.width ?? 70;
    const HEIGHT = tokens.height ?? 46;

    return (
        <Tooltip title={isDark ? "Switch to light mode" : "Switch to dark mode"}>
            <Box
                component="span"
                sx={{
                    position: "relative",
                    width: WIDTH,
                    height: HEIGHT,
                    display: "inline-flex",
                    alignItems: "center",
                    ...sx,
                }}
            >
                <GradientSwitch
                    checked={isDark}
                    onChange={toggleMode}
                    slotProps={{
                        input: { "aria-label": "Toggle color scheme" },
                        // set thumb color via sx so it can read theme tokens
                        thumb: {
                            sx: {
                                backgroundColor: tokens.thumb ?? "#ffffff",
                            },
                        },
                    }}
                />

                <LightModeRoundedIcon
                    sx={{
                        position: "absolute",
                        left: 14,
                        top: "50%",
                        transform: "translateY(-50%)",
                        color: iconColor,
                        opacity: isDark ? 0.2 : 1,
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
                        color: iconColor,
                        opacity: isDark ? 1 : 0.2,
                        pointerEvents: "none",
                        fontSize: 16,
                    }}
                />
            </Box>
        </Tooltip>
    );
}