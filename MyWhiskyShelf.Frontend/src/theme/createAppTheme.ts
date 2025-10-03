import {createTheme, type PaletteMode} from "@mui/material";
import {darken, lighten} from "@mui/material/styles";
import {COLOURS} from "./tokens";

export function createAppTheme(mode: PaletteMode) {
    const baseColour = mode === "dark" ? COLOURS.dark.base : COLOURS.light.base;

    const gradientLighten    = lighten(baseColour, 0.15);
    const gradientDarken = darken( baseColour, 0.5);

    return createTheme({
        palette: {
            mode,
            primary: { main: baseColour },            
            background: {
                default: gradientLighten,                     
                paper: baseColour,
            }
        },
        components: {
            MuiCssBaseline: {
                styleOverrides: {
                    body: {
                        backgroundImage: `linear-gradient(180deg, ${gradientLighten} 0%, ${gradientDarken} 100%)`,
                        backgroundAttachment: "fixed",
                        backgroundRepeat: "no-repeat",
                        backgroundSize: "cover",
                    }
                }
            }
        },
        shape: { borderRadius: 12 }
    });
}