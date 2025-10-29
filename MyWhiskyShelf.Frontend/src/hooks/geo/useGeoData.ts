import * as React from "react";
// @ts-ignore
import { getGeoBootstrap } from "@/api/geo/geoApi";
// @ts-ignore
import type { Country, Region, GeoBootstrap } from "@/api/geo/contracts/types";

export function useGeoData() {
    const abortRef = React.useRef<AbortController | null>(null);
    const [loading, setLoading] = React.useState(true);
    const [error, setError] = React.useState<unknown>(null);
    const [countries, setCountries] = React.useState<Country[]>([]);
    const [regions, setRegions] = React.useState<Region[]>([]);

    const load = React.useCallback(async () => {
        abortRef.current?.abort();
        const ac = new AbortController();
        abortRef.current = ac;
        setLoading(true);
        setError(null);
        try {
            const boot: GeoBootstrap = await getGeoBootstrap(ac.signal);
            setCountries(boot.countries ?? []);
            setRegions(boot.regions ?? []);
        } catch (e: any) {
            if (e?.name !== "AbortError") setError(e);
        } finally {
            setLoading(false);
        }
    }, []);

    React.useEffect(() => {
        void load();
        return () => abortRef.current?.abort();
    }, [load]);

    return { loading, error, reload: load, countries, regions };
}
