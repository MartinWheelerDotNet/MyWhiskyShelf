import * as React from "react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ThemeProvider, createTheme } from "@mui/material/styles";

import DistilleriesToolbar, { type DistilleriesToolbarProps } from "./DistilleriesToolbar";

function renderWithTheme(ui: React.ReactNode) {
    const theme = createTheme({ palette: { mode: "light" } });
    return render(<ThemeProvider theme={theme}>{ui}</ThemeProvider>);
}

let onQueryChange: (v: string) => void;
let onCountryChange: (v: string) => void;
let onRegionChange: (v: string) => void;

function makeProps(overrides: Partial<DistilleriesToolbarProps> = {}): DistilleriesToolbarProps {
    onQueryChange = vi.fn();
    onCountryChange = vi.fn();
    onRegionChange = vi.fn();

    return {
        query: "",
        onQueryChange,
        country: "all",
        onCountryChange,
        region: "all",
        onRegionChange,
        countryOptions: [
            { value: "scotland", label: "Scotland" },
            { value: "japan", label: "Japan" },
        ],
        regionOptions: [
            { value: "islay", label: "Islay" },
            { value: "kansai", label: "Kansai" },
        ],
        ...overrides,
    };
}

beforeEach(() => {
    // Sonar-friendly
    globalThis.location.hash = "";
    vi.clearAllMocks();
});

describe("DistilleriesToolbar", () => {
    it("renders with default title when none provided", () => {
        renderWithTheme(<DistilleriesToolbar {...makeProps()} />);
        expect(screen.getByRole("heading", { name: /distilleries/i })).toBeInTheDocument();
    });

    it("renders with a custom title", () => {
        renderWithTheme(<DistilleriesToolbar {...makeProps({ title: "My Shelf" })} />);
        expect(screen.getByRole("heading", { name: /my shelf/i })).toBeInTheDocument();
    });

    it("typing in Search results in the latest accumulated value when controlled", async () => {
        const user = userEvent.setup();
        const spy = vi.fn(); 
        
        function ToolbarHarness() {
            const [query, setQuery] = React.useState("");
            return (
                <DistilleriesToolbar
                    {...makeProps({
                        query,
                        onQueryChange: (v) => {
                            setQuery(v);
                            spy(v);
                        },
                    })}
                />
            );
        }
        renderWithTheme(<ToolbarHarness />);

        const search = screen.getByRole("textbox", { name: /search/i });
        await user.type(search, "ard");
        
        const last = spy.mock.calls.at(-1);
        expect(last).toEqual(["ard"]);
    });

    it("Country select shows All + provided options and calls onCountryChange", async () => {
        const user = userEvent.setup();
        renderWithTheme(<DistilleriesToolbar {...makeProps()} />);

        const countryButton = screen.getByLabelText(/country/i);
        expect(countryButton).toBeInTheDocument();

        await user.click(countryButton);

        const listbox = await screen.findByRole("listbox");
        const items = within(listbox).getAllByRole("option");
        expect(items.length).toBeGreaterThanOrEqual(3);
        expect(within(listbox).getByRole("option", { name: /all/i })).toBeInTheDocument();
        expect(within(listbox).getByRole("option", { name: /scotland/i })).toBeInTheDocument();
        expect(within(listbox).getByRole("option", { name: /japan/i })).toBeInTheDocument();

        await user.click(within(listbox).getByRole("option", { name: /japan/i }));
        expect(onCountryChange).toHaveBeenCalledWith("japan");
    });

    it("Region select shows All + provided options and calls onRegionChange", async () => {
        const user = userEvent.setup();
        renderWithTheme(<DistilleriesToolbar {...makeProps()} />);

        const regionButton = screen.getByLabelText(/region/i);
        expect(regionButton).toBeInTheDocument();

        await user.click(regionButton);

        const listbox = await screen.findByRole("listbox");
        expect(within(listbox).getByRole("option", { name: /all/i })).toBeInTheDocument();
        expect(within(listbox).getByRole("option", { name: /islay/i })).toBeInTheDocument();
        expect(within(listbox).getByRole("option", { name: /kansai/i })).toBeInTheDocument();

        await user.click(within(listbox).getByRole("option", { name: /islay/i }));
        expect(onRegionChange).toHaveBeenCalledWith("islay");
    });

    it("reflects controlled values for country and region", async () => {
        const props = makeProps({ country: "japan", region: "kansai" });
        renderWithTheme(<DistilleriesToolbar {...props} />);

        expect(screen.getByLabelText(/country/i)).toHaveTextContent(/japan/i);
        expect(screen.getByLabelText(/region/i)).toHaveTextContent(/kansai/i);
    });

    it("works with empty option arrays and still shows 'All'", async () => {
        const user = userEvent.setup();
        const props = makeProps({ countryOptions: [], regionOptions: [] });
        renderWithTheme(<DistilleriesToolbar {...props} />);

        await user.click(screen.getByLabelText(/country/i));
        let listbox = await screen.findByRole("listbox");
        expect(within(listbox).getByRole("option", { name: /all/i })).toBeInTheDocument();

        await user.click(within(listbox).getByRole("option", { name: /all/i })); // close
        await user.click(screen.getByLabelText(/region/i));
        listbox = await screen.findByRole("listbox");
        expect(within(listbox).getByRole("option", { name: /all/i })).toBeInTheDocument();
    });
});
