// @ts-ignore
import type { DistilleryResponse } from "@/api/distilleriesApi";
// @ts-ignore
import type { DistilleryCardProps } from "@/components/DistilleryCard";

export function toCardProps(d: DistilleryResponse): DistilleryCardProps {
    return {
        id: d.id,
        name: d.name,
        region: d.region,
        country: d.country,
        founded: d.founded,
        owner: d.owner,
        description: d.description,
        tastingNotes: d.tastingNotes,
        whiskiesCount: 0,
        logoUrl: `/media/images/distilleries/${d.name.toLowerCase()}-logo.png`,
    };
}