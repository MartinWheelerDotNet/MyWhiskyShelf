import * as React from "react";
import { CssBaseline, type PaletteMode } from "@mui/material";
import { ThemeProvider as MuiThemeProvider } from "@mui/material/styles";
import { createAppTheme } from "./createAppTheme";

const STORAGE_KEY = "mws-theme-mode";

export type ThemeModeContextType = {
    mode: PaletteMode;
    toggleMode: () => void;
    setMode: (m: PaletteMode) => void;
    baseLight: string;
    baseDark: string;
    setBaseLight: (hex: string) => void;
    setBaseDark: (hex: string) => void;
};

export const ThemeModeContext = React.createContext<ThemeModeContextType>({
    mode: "light",
    toggleMode: () => {},
    setMode: () => {},
    baseLight: "#ff8ba7",
    baseDark: "#1e3a8a",
    setBaseLight: () => {},
    setBaseDark: () => {}
});

function getInitialMode(): PaletteMode {
    try {
        const saved = localStorage.getItem(STORAGE_KEY) as PaletteMode | null;
        if (saved === "light" || saved === "dark") return saved;
    } catch {}
    return window.matchMedia?.("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

export const ThemeModeProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [mode, setMode] = React.useState<PaletteMode>(() => getInitialMode());
    const [baseLight, setBaseLight] = React.useState("#ff8ba7");
    const [baseDark, setBaseDark]   = React.useState("#1e3a8a");

    React.useEffect(() => { try { localStorage.setItem(STORAGE_KEY, mode); } catch {} }, [mode]);
    const toggleMode = React.useCallback(() => setMode(m => (m === "light" ? "dark" : "light")), []);

    const theme = React.useMemo(() => createAppTheme(mode, baseLight, baseDark), [mode, baseLight, baseDark]);

    const value = React.useMemo(
        () => ({ mode, toggleMode, setMode, baseLight, baseDark, setBaseLight, setBaseDark }),
        [mode, toggleMode, baseLight, baseDark]
    );

    return (
        <ThemeModeContext.Provider value={value}>
            <MuiThemeProvider theme={theme}>
                <CssBaseline enableColorScheme />
                {children}
            </MuiThemeProvider>
        </ThemeModeContext.Provider>
    );
};

export const useThemeMode = () => React.useContext(ThemeModeContext);