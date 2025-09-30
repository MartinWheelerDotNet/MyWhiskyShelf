import { createTheme, type PaletteMode } from "@mui/material";
import { lighten, darken } from "@mui/material/styles";

export function createAppTheme(
    mode: PaletteMode,
    baseLight = "#a0b2ba", // ← your light base
    baseDark  = "#02141c"  // ← your dark base
) {
    const base = mode === "dark" ? baseDark : baseLight;

    // tweak these to taste
    const top    = lighten(base, mode === "light" ? 0.06 : 0.25);
    const bottom = darken( base, mode === "dark" ? 0.35 : 0.18);

    return createTheme({
        palette: {
            mode,
            primary: { main: base },            // you can choose a different primary if you prefer
            background: {
                default: top,                     // used by some surfaces
                paper: mode === "dark" ? baseDark : baseLight,
            }
        },
        components: {
            // put the gradient on the <body>
            MuiCssBaseline: {
                styleOverrides: {
                    body: {
                        backgroundImage: `linear-gradient(180deg, ${top} 0%, ${bottom} 100%)`,
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