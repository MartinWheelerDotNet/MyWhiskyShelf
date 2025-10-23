export interface Distillery {
    id: string;
    name: string;
    countryName: string;
    countryId: string;
    regionName?: string;
    regionId?: string;
    founded?: number;
    owner?: string;
    type: string;
    description?: string;
    tastingNotes?: string;
    flavourProfile?: {
        sweet: number; smoky: number; fruity: number; spicy: number; malty: number;
    };
    active: boolean;
}