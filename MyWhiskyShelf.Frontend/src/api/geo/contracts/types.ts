export type CountryResponse = {
    id: string;
    name: string;
    isActive: boolean;
    regions: RegionResponse[];
};

export type RegionResponse = {
    id: string;
    countryId: string;
    name: string;
    isActive: boolean;
};

export type Country = {
    id: string;
    name: string;
    isActive: boolean;
};

export type Region = {
    id: string;
    countryId: string;
    name: string;
    isActive: boolean;
};

export type GeoBootstrap = {
    countries: Country[];
    regions: Region[];
    countryMap: Map<string, Country>;
    regionMap: Map<string, Region>;
};