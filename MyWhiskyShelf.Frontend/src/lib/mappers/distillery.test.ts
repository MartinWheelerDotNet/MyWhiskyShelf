import { describe, it, expect } from "vitest";
import { mapToDistillery, mapToDistilleryCardProps } from "./distillery";

describe("mapToDistillery (API → Domain)", () => {
    it("maps all fields and converts nulls to undefined", () => {
        const api = {
            id: "123",
            name: "Ardbeg",
            countryId: "456",
            countryName: "Scotland",
            regionId: "789",
            regionName: "Islay",
            founded: 1815,
            owner: "LVMH",
            type: "Single Malt",
            description: "Peaty goodness",
            tastingNotes: "Smoke, brine",
            flavourProfile: { sweet: 1, smoky: 5, fruity: 2, spicy: 3, malty: 2 },
            active: true,
        } as const;

        const domain = mapToDistillery(api);
        expect(domain).not.toBe(api); // new object
        expect(domain).toEqual({
            id: "123",
            name: "Ardbeg",
            countryId: "456",
            countryName: "Scotland",
            regionId: "789",
            regionName: "Islay",
            founded: 1815,
            owner: "LVMH",
            type: "Single Malt",
            description: "Peaty goodness",
            tastingNotes: "Smoke, brine",
            flavourProfile: { sweet: 1, smoky: 5, fruity: 2, spicy: 3, malty: 2 },
            active: true,
        });
    });

    it("normalizes nullable fields to undefined", () => {
        const apiWithNulls = {
            id: "123",
            name: "Ardbeg",
            countryId: "456",
            countryName: "Scotland",
            regionId: null,
            regionName: null,
            founded: null,
            owner: null,
            type: null,
            description: null,
            tastingNotes: null,
            flavourProfile: null,
            active: false,
        } as any;

        const domain = mapToDistillery(apiWithNulls);
        expect(domain).toEqual({
            id: "123",
            name: "Ardbeg",
            countryId: "456",
            countryName: "Scotland",
            regionId: undefined,
            regionName: undefined,
            founded: undefined,
            owner: undefined,
            type: undefined,
            description: undefined,
            tastingNotes: undefined,
            flavourProfile: undefined,
            active: false,
        });
    });
});

describe("mapToDistilleryCardProps (Domain → CardProps)", () => {
    it("picks and normalizes only the fields needed by the card", () => {
        const domain = {
            id: "123",
            name: "Lagavulin",
            countryId: "456",
            countryName: "Scotland",
            regionId: "789",
            regionName: "Islay",
            founded: 1816,
            owner: "Diageo",
            type: "Malt",
            description: "Rich smoke",
            tastingNotes: "Smoke, iodine",
            flavourProfile: { sweet: 2, smoky: 5, fruity: 1, spicy: 2, malty: 2 },
            active: true,
        };

        const props = mapToDistilleryCardProps(domain);
        expect(props).toEqual({
            id: "123",
            name: "Lagavulin",
            countryName: "Scotland",
            regionName: "Islay",
            founded: 1816,
            owner: "Diageo",
            description: "Rich smoke",
            tastingNotes: "Smoke, iodine",
            type: "Malt"
        });

        expect("regionId" in props).toBe(false);
        expect("countryId" in props).toBe(false);
        expect("active" in props).toBe(false);
        expect("flavourProfile" in props).toBe(false);
    });

    it("converts null/undefined in domain to undefined in props", () => {
        const domain = {
            id: "123",
            name: "Ardbeg",
            countryId: "456",
            countryName: "Scotland",
            regionId: null,
            regionName: null as unknown as undefined,
            founded: undefined,
            owner: null as unknown as undefined,
            type: undefined,
            description: null as unknown as undefined,
            tastingNotes: undefined,
            flavourProfile: undefined,
            active: false,
        };

        const props = mapToDistilleryCardProps(domain as any);
        expect(props).toEqual({
            id: "123",
            name: "Ardbeg",
            countryName: "Scotland",
            regionName: undefined,
            founded: undefined,
            owner: undefined,
            type: undefined,
            description: undefined,
            tastingNotes: undefined,
        });
    });
});
