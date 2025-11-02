// @ts-ignore
import { axiosClient } from "@/api/axiosClient";
// @ts-ignore
import type { CountryResponse, Region, Country, GeoBootstrap } from "@/api/geo/contracts/types";

export async function getGeoBootstrap(signal?: AbortSignal): Promise<GeoBootstrap> {
    const res = await axiosClient.get<CountryResponse[]>("/geo", { signal });

    const countries: Country[] = [];
    const regions: Region[] = [];
    
    for (const c of res.data ?? []) {
        const country: Country = {
            id: c.id,
            name: c.name,
            isActive: !!c.isActive,
        };
        countries.push(country);

        for (const r of c.regions ?? []) {
            const region : Region = {
                id: r.id,
                countryId: r.countryId,
                name: r.name,
                isActive: !!r.isActive,
            }
            regions.push(region);
        }
    }

    const countryMap = new Map(countries.map(c => [c.id, c]));
    const regionMap = new Map(regions.map(r => [r.id, r]));

    return { countries, regions, countryMap, regionMap };
}