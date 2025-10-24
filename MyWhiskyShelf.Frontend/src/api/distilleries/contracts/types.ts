export interface DistilleryResponse {
    id: string;
    name: string;
    countryId: string;
    countryName: string;
    regionId: string;
    regionName: string;
    founded: number;
    owner: string;
    type: string;
    description: string;
    tastingNotes: string;
    flavourProfile: {
        sweet: number;
        smoky: number;
        fruity: number;
        spicy: number;
        malty: number;
    };
    active: boolean;
}