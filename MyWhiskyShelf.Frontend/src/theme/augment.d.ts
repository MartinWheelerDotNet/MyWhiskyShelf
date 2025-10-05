import "@mui/material/styles";
import type { AppBarProps } from "@mui/material/AppBar";

declare module "@mui/material/styles" {
    interface AppBrand {
        logoSrc?: string;
        logoHeight?: number;
        logoFilterLight?: string;
        logoFilterDark?: string;
    }

    interface AppLayout {
        appBarPosition?: "fixed" | "absolute" | "sticky" | "static" | "relative";
        appBarElevation?: number;
        appBarColor?: AppBarProps["color"];
        appBarBg?: string;
    }

    interface AppControlsModeToggle {
        thumb?: string;
        icon?: string;
        track?: { light?: string; dark?: string };
        palette?: { lightThumb?: string; darkThumb?: string };
        width?: number;
        height?: number;
        padding?: number;
        thumbSize?: number;
        leftThumbPos?: number;
    }

    interface Theme {
        app: {
            brand: AppBrand;
            layout: AppLayout;
            controls: {
                modeToggle: AppControlsModeToggle;
            };
        };
    }

    /** Allow partial values at theme creation */
    interface ThemeOptions {
        app?: {
            brand?: AppBrand;
            layout?: AppLayout;
            controls?: {
                modeToggle?: AppControlsModeToggle;
            };
        };
    }
}