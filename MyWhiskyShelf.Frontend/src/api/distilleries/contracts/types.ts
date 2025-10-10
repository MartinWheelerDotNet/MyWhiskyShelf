export interface DistilleryResponse {
    id: string;
    name: string;
    country: string;
    region: string;
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