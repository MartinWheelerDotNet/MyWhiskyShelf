import {alpha, createTheme, type PaletteMode} from "@mui/material";
import {darken, lighten} from "@mui/material/styles";
import {COLOURS} from "./tokens";

export function createAppTheme(mode: PaletteMode) {
    const themeColours = COLOURS[mode];
    const gradientLight  = lighten(themeColours.base, 0.15);
    const gradientDark = darken(themeColours.base, 0.5);

    return createTheme({
        palette: {
            mode,
            primary: { main: themeColours.accent },
            secondary: { main: themeColours.accentHover },
            background: {
                default: gradientLight,                     
                paper: themeColours.base,
            },
            text: { primary: themeColours.text },
            divider: themeColours.border,
            action: {
                hover: alpha(themeColours.accent, 0.08),
                selected: alpha(themeColours.accent, 0.16),
                focus: alpha(themeColours.accent, 0.24),
                active: themeColours.accent,
            },
        },
        app: {
            brand: {
                logoSrc: "/media/images/mywhiskyshelf-logo-horizontal.png",
                logoHeight: 32,
                logoFilterLight: "none",
                logoFilterDark: "brightness(1) invert(1)",
            },
            layout: {
                appBarPosition: "static",
                appBarElevation: 1,
                appBarColor: "transparent",
                appBarBg: "transparent",
            },
            controls: {
                modeToggle: {
                    thumb: themeColours.modeToggle,
                    icon: "#ffffff",
                    track: {
                        light: "linear-gradient(90deg, #f0b429 0%, #ff8ba7 100%)",
                        dark:  "linear-gradient(90deg, #34b1eb 0%, #0e4f6e 100%)",
                    },
                    palette: {
                        lightThumb: COLOURS.light.modeToggle,
                        darkThumb:  COLOURS.dark.modeToggle,
                    },
                    width: 70,
                    height: 46,
                    padding: 10,
                    thumbSize: 20,
                    leftThumbPos: 12,
                },
            },
        },
        components: {
            MuiCssBaseline: {
                styleOverrides: {
                    body: {
                        backgroundImage: `linear-gradient(180deg, ${gradientLight} 0%, ${gradientDark} 100%)`,
                        backgroundAttachment: "fixed",
                        backgroundRepeat: "no-repeat",
                        backgroundSize: "cover",
                    }
                }
            },
            MuiOutlinedInput: {
                styleOverrides: {
                    root: {
                        "& .MuiOutlinedInput-input:focus": {
                            outline: "none"
                        },
                        "& .MuiOutlinedInput-notchedOutline": {
                            borderColor: alpha(themeColours.border, 0.6),
                        },
                        "&:hover .MuiOutlinedInput-notchedOutline": {
                            borderColor: themeColours.accentHover
                        },
                        "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
                            borderColor: themeColours.accent,
                            borderWidth: 2
                        },
                        "&.Mui-focused": {
                            borderRadius: 12
                        },
                    },
                },
            },
            MuiInputLabel: {
                styleOverrides: {
                    outlined: {
                        "&.MuiInputLabel-shrink": {
                            paddingLeft: 4,
                            paddingRight: 4,
                            borderRadius: 4,
                            lineHeight: 1.2,
                        },
                    },
                },
            },
        },
        shape: { borderRadius: 12 }
    });
}