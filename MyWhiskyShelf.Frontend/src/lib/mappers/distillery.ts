// @ts-ignore
import { DistilleryResponse } from "@/api/distilleries/contracts/types";
// @ts-ignore
import { Distillery } from "@/lib/domain/types";
// @ts-ignore
import { DistilleryCardProps } from "@/components/distilleries/DistilleryCard";

export function mapToDistillery(response: DistilleryResponse) : Distillery {
    return {
        id: response.id,
        name: response.name,
        country: response.country ?? undefined,
        region: response.region ?? undefined,
        founded: response.founded ?? undefined,
        owner: response.owner ?? undefined,
        type: response.type ?? undefined,
        description: response.description ?? undefined,
        tastingNotes: response.tastingNotes ?? undefined,
        flavourProfile: response.flavourProfile ?? undefined,
        active: response.active
    };
}

export function mapToDistilleryCardProps(domain: Distillery) : DistilleryCardProps {
    return {
        id: domain.id,
        name: domain.name,
        country: domain.country ?? undefined,
        region: domain.region ?? undefined,
        founded: domain.founded ?? undefined,
        owner: domain.owner ?? undefined,
        description: domain.description ?? undefined,
        tastingNotes: domain.tastingNotes ?? undefined,
    }
}