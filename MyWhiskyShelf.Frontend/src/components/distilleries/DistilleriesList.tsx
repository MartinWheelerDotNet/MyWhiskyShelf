import Grid from "@mui/material/Grid";

// @ts-ignore
import SkeletonDistilleryCard from "@/components/distilleries/SkeletonDistilleryCard";
// @ts-ignore
import DistilleryCard from "@/components/distilleries/DistilleryCard";
// @ts-ignore
import {mapToDistillery, mapToDistilleryCardProps} from "@/lib/mappers/distillery";
// @ts-ignore
import { Distillery } from "@/lib/domain/types";

type DistilleryListProps = {
    items: Distillery[];
    getId?: (raw: Distillery) => string;
    initialLoading: boolean;
    showMoreSkeletons?: boolean;
    initialSkeletonCount?: number;
    moreSkeletonCount?: number;
    setSentinel: (el: HTMLDivElement | null) => void;
};

const defaultGetId = (x: any) => String(x?.id ?? x?.Id ?? x?.distilleryId ?? "");

export default function DistilleriesList(
    {
        items, 
        getId = defaultGetId,
        initialLoading,
        showMoreSkeletons = true,
        initialSkeletonCount = 6,
        moreSkeletonCount = 3,
        setSentinel
    }: DistilleryListProps) {
    
    if (initialLoading) {
        return (
            <Grid container spacing={1} sx={{ mt: 1 }}>
                {Array.from({ length: initialSkeletonCount }).map((_, i) => (
                    <Grid key={`init-sk-${i}`} size={{ xs: 12, sm: 6, md: 12 }}>
                        <SkeletonDistilleryCard />
                    </Grid>
                ))}
            </Grid>
        );
    }

    return (
        <>
            <Grid container spacing={1} sx={{ mb: 2 }}>
                {items.map((raw: Distillery) => {
                    const key = getId(raw);
                    if (!key) return null;
                    const card = mapToDistilleryCardProps(raw);
                    if (!card) return null;
                    return (
                        <Grid key={key} size={{ xs: 12, sm: 6, md: 12 }}>
                            <DistilleryCard {...card} />
                        </Grid>
                    );
                })}

                {showMoreSkeletons &&
                    Array.from({ length: moreSkeletonCount }).map((_, i) => (
                        <Grid key={`more-sk-${i}`} size={{ xs: 12, sm: 6, md: 12 }}>
                            <SkeletonDistilleryCard />
                        </Grid>
                    ))}
            </Grid>

            <div ref={setSentinel} style={{ height: 1 }} aria-hidden="true" />
        </>
    );
}