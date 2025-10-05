import { describe, it, expect, beforeEach, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { useTheme } from "@mui/material/styles";

import { ThemeModeProvider, useThemeMode } from "./ThemeModeProvider";

const STORAGE_KEY = "mws-theme-mode";

function makeMockLocalStorage() {
    const store = new Map<string, string>();
    return {
        getItem: vi.fn((k: string) => (store.has(k) ? store.get(k)! : null)),
        setItem: vi.fn((k: string, v: string) => {
            store.set(k, v);
        }),
        removeItem: vi.fn((k: string) => {
            store.delete(k);
        }),
        clear: vi.fn(() => store.clear()),
        _dump: () => Object.fromEntries(store.entries()),
    } as unknown as Storage & { _dump: () => Record<string, string> };
}

function stubMatchMedia(prefersDark: boolean) {
    const mm = vi.fn().mockImplementation((query: string) => {
        const isDarkQuery = /\(prefers-color-scheme:\s*dark\)/i.test(query);
        return {
            matches: isDarkQuery ? prefersDark : false,
            media: query,
            onchange: null,
            addListener: vi.fn(),
            removeListener: vi.fn(),
            addEventListener: vi.fn(),
            removeEventListener: vi.fn(),
            dispatchEvent: vi.fn(),
        } as unknown as MediaQueryList;
    });
    globalThis.matchMedia = mm;
    return mm;
}

function Probe() {
    const { mode, toggleMode, setMode } = useThemeMode();
    const theme = useTheme();
    return (
        <button
            data-testid="probe"
            data-mode={mode}
            data-theme-mode={theme.palette.mode}
            onClick={toggleMode}
            onAuxClick={() => setMode("light")}
            onDoubleClick={() => setMode("dark")}
        >
            probe
        </button>
    );
}

let lifecycle: Storage & { _dump: () => Record<string, string> };

beforeEach(() => {
    lifecycle = makeMockLocalStorage();
    globalThis.localStorage = lifecycle;

    stubMatchMedia(false);

    vi.useRealTimers();
    vi.clearAllMocks();
});

describe("ThemeModeProvider", () => {
    it("initializes from localStorage when value is present", async () => {
        localStorage.setItem(STORAGE_KEY, "dark");

        render(
            <ThemeModeProvider>
                <Probe />
            </ThemeModeProvider>
        );

        const probe = screen.getByTestId("probe");
        expect(probe.dataset.mode).toBe("dark");
        expect(probe.dataset.themeMode).toBe("dark");
    });

    it("falls back to prefers-color-scheme when localStorage is empty", async () => {
        stubMatchMedia(true);

        render(
            <ThemeModeProvider>
                <Probe />
            </ThemeModeProvider>
        );

        const probe = screen.getByTestId("probe");
        expect(probe.dataset.mode).toBe("dark");
        expect(probe.dataset.themeMode).toBe("dark");
    });

    it("defaults to light when no localStorage and system preference is not dark", async () => {
        stubMatchMedia(false);

        render(
            <ThemeModeProvider>
                <Probe />
            </ThemeModeProvider>
        );

        const probe = screen.getByTestId("probe");
        expect(probe.dataset.mode).toBe("light");
        expect(probe.dataset.themeMode).toBe("light");
    });

    it("toggleMode flips the mode and persists to localStorage", async () => {
        const user = userEvent.setup();
        localStorage.setItem(STORAGE_KEY, "light");

        render(
            <ThemeModeProvider>
                <Probe />
            </ThemeModeProvider>
        );

        const probe = screen.getByTestId("probe");
        expect(probe.dataset.mode).toBe("light");
        expect(probe.dataset.themeMode).toBe("light");

        await user.click(probe);

        const probeAfter = screen.getByTestId("probe");
        expect(probeAfter.dataset.mode).toBe("dark");
        expect(probeAfter.dataset.themeMode).toBe("dark");

        expect(localStorage.setItem).toHaveBeenCalledWith(STORAGE_KEY, "dark");
        expect(lifecycle._dump()[STORAGE_KEY]).toBe("dark");
    });

    it("setMode('light')/setMode('dark') set explicit mode and persist", async () => {
        const user = userEvent.setup();

        render(
            <ThemeModeProvider>
                <Probe />
            </ThemeModeProvider>
        );

        const probe = screen.getByTestId("probe");

        await user.pointer({ keys: "[MouseLeft>]", target: probe });
        await user.pointer({ keys: "[MouseMiddle]", target: probe });

        let probeAfter = screen.getByTestId("probe");
        expect(probeAfter.dataset.mode).toBe("light");
        expect(probeAfter.dataset.themeMode).toBe("light");
        expect(localStorage.setItem).toHaveBeenCalledWith(STORAGE_KEY, "light");

        await user.dblClick(probeAfter);

        probeAfter = screen.getByTestId("probe");
        expect(probeAfter.dataset.mode).toBe("dark");
        expect(probeAfter.dataset.themeMode).toBe("dark");
        expect(localStorage.setItem).toHaveBeenLastCalledWith(STORAGE_KEY, "dark");
    });

    it("persists mode on every change (effect runs when mode changes)", async () => {
        const user = userEvent.setup();

        render(
            <ThemeModeProvider>
                <Probe />
            </ThemeModeProvider>
        );

        const probe = screen.getByTestId("probe");

        await user.click(probe);
        await user.click(probe);
        await user.click(probe);

        expect((localStorage.setItem as any).mock.calls.length).toBeGreaterThanOrEqual(3);
        const lastCall = (localStorage.setItem as any).mock.calls.at(-1);
        expect(lastCall).toEqual([STORAGE_KEY, "dark"]);
    });
});
