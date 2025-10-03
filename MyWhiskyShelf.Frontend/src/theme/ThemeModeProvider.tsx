import * as React from "react";
import { CssBaseline, type PaletteMode } from "@mui/material";
import { ThemeProvider as MuiThemeProvider } from "@mui/material/styles";
import { createAppTheme } from "./createAppTheme";

const STORAGE_KEY = "mws-theme-mode";

export type ThemeModeContextType = {
    mode: PaletteMode;
    toggleMode: () => void;
    setMode: (m: PaletteMode) => void;
};

export const ThemeModeContext = React.createContext<ThemeModeContextType>({
    mode: "light",
    toggleMode: () => {},
    setMode: () => {}
});

function getInitialMode(): PaletteMode {
    try {
        const saved = localStorage.getItem(STORAGE_KEY) as PaletteMode | null;
        if (saved === "light" || saved === "dark") return saved;
    } catch {}
    return globalThis.matchMedia?.("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

export const ThemeModeProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [mode, setMode] = React.useState<PaletteMode>(() => getInitialMode());

    React.useEffect(() => { 
        try { 
            localStorage.setItem(STORAGE_KEY, mode); 
        } catch {} 
    }, [mode]);
    
    const toggleMode = React.useCallback(
        () => setMode(m => (m === "light" ? "dark" : "light")), []
    );

    const theme = React.useMemo(() => createAppTheme(mode), [mode]);

    const value = React.useMemo(
        () => ({ mode, toggleMode, setMode }), [mode, toggleMode]
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