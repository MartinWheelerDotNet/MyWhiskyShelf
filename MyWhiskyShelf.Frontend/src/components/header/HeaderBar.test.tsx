import {beforeEach, describe, expect, it, vi} from "vitest";
import {render} from "@testing-library/react";
import {createTheme, ThemeProvider} from "@mui/material/styles";
import {hexToRgb} from "@mui/material";
import HeaderBar from "./HeaderBar";

vi.mock("@/components/ThemeToggle", () => ({
    __esModule: true,
    default: () => <div data-testid="theme-toggle">THEME-TOGGLE</div>,
}));
vi.mock("@/components/ThemeToggleMobile", () => ({
    __esModule: true,
    default: () => <div data-testid="theme-toggle-mobile">THEME-TOGGLE-MOBILE</div>,
}));
vi.mock("@/components/AccountMenuActions", () => ({
    __esModule: true,
    default: () => <div data-testid="account-menu">ACCOUNT-MENU</div>,
}));

type Mode = "light" | "dark";

function makeTheme(mode: Mode, options?: {
    brand?: Partial<{
        logoSrc: string;
        logoHeight: number;
        logoFilterLight: string;
        logoFilterDark: string;
    }>;
    layout?: Partial<{
        appBarPosition: "fixed" | "absolute" | "sticky" | "static" | "relative";
        appBarElevation: number;
        appBarColor: any;
        appBarBg: string;
    }>;
}) {
    return createTheme({
        palette: {mode},
        app: {
            brand: {
                logoSrc: "/media/images/mywhiskyshelf-logo-horizontal.png",
                logoHeight: 32,
                logoFilterLight: "none",
                logoFilterDark: "brightness(1) invert(1)",
                ...(options?.brand ?? {}),
            },
            layout: {
                appBarPosition: "static",
                appBarElevation: 1,
                appBarColor: "transparent",
                appBarBg: "#123456",
                ...(options?.layout ?? {}),
            },
            controls: {
                modeToggle: {},
            },
        },
    } as any);
}

function renderWithTheme(mode: Mode, opts?: Parameters<typeof makeTheme>[1]) {
    const theme = makeTheme(mode, opts);
    return render(
        <ThemeProvider theme={theme}>
            <HeaderBar />
        </ThemeProvider>
    );
}

beforeEach(() => {
    globalThis.location.hash = "";
});

describe("HeaderBar", () => {
    it("renders brand logo with src and height; light mode uses light filter", () => {
        const { getByAltText, getByTestId, getByRole } = renderWithTheme("light", {
            brand: {
                logoSrc: "/media/images/custom-logo.png",
                logoHeight: 40,
                logoFilterLight: "grayscale(1)",
            },
            layout: { appBarBg: "#abcdef" },
        });

        const banner = getByRole("banner");
        expect(banner).toBeInTheDocument();

        const bg = getComputedStyle(banner).backgroundColor;
        expect(bg).toBe(hexToRgb("#abcdef"));

        const logo = getByAltText(/mywhiskyshelf/i) as HTMLImageElement;
        expect(logo).toBeInTheDocument();
        expect(logo.src).toContain("/media/images/custom-logo.png");

        expect(getComputedStyle(logo).height).toBe("40px");

        expect(getComputedStyle(logo).filter).toBe("grayscale(1)");

        expect(getByTestId("theme-toggle-mobile")).toBeInTheDocument();
        expect(getByTestId("theme-toggle")).toBeInTheDocument();
        expect(getByTestId("account-menu")).toBeInTheDocument();
    });

    it("dark mode applies the dark filter (or default) to the logo", () => {
        const { getByAltText } = renderWithTheme("dark", {
            brand: { logoFilterDark: "brightness(0.8) invert(1)" },
        });

        const logo = getByAltText(/mywhiskyshelf/i);
        expect(getComputedStyle(logo).filter).toBe("brightness(0.8) invert(1)");
    });

    it("falls back to default filters when brand tokens are not provided", () => {
        const { getByAltText, rerender } = renderWithTheme("light", {
            brand: { logoFilterLight: undefined as any },
        });

        const logoLight = getByAltText(/mywhiskyshelf/i);
        expect(getComputedStyle(logoLight).filter).toBe("none");

        rerender(
            <ThemeProvider theme={makeTheme("dark", { brand: { logoFilterDark: undefined as any } })}>
                <HeaderBar />
            </ThemeProvider>
        );

        const logoDark = getByAltText(/mywhiskyshelf/i);
        expect(getComputedStyle(logoDark).filter).toBe("brightness(1) invert(1)");
    });

    it("respects layout tokens for AppBar background color", () => {
        const { getByRole } = renderWithTheme("light", {
            layout: { appBarBg: "#334455" },
        });

        const banner = getByRole("banner");
        const bg = getComputedStyle(banner).backgroundColor;
        expect(bg).toBe(hexToRgb("#334455"));
    });
});
