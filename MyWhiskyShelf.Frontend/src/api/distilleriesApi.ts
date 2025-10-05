import { axiosClient } from "./axiosClient";

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

export async function getAllDistilleries(): Promise<DistilleryResponse[]> {
    const response = await axiosClient.get<DistilleryResponse[]>("/distilleries");
    return response.data;
}