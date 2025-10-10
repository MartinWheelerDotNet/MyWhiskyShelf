import { Card, CardHeader, Skeleton, Stack, alpha, useMediaQuery } from "@mui/material";

export default function SkeletonDistilleryCard() {
    const reduceMotion = useMediaQuery("(prefers-reduced-motion: reduce)");
    const animation: "wave" | false = reduceMotion ? false : "wave";

    return (
        <Card
            elevation={8}
            sx={(t) => ({
                borderRadius: 3,
                overflow: "hidden",
                bgcolor: alpha(t.palette.background.paper, 0.9),
                backdropFilter: "blur(6px)",
                border: `2px solid ${alpha(t.palette.divider, 1)}`,
                boxShadow: `0 8px 30px ${alpha(t.palette.common.black, 0.25)}`,
            })}
        >
            <CardHeader
                avatar={<Skeleton variant="circular" width={40} height={40} animation={animation} />}
                title={<Skeleton variant="text" width="45%" animation={animation} sx={{ fontWeight: 700 }} />}
                subheader={
                    <Stack pt={1} direction="row" gap={1} flexWrap="wrap" alignItems="center">
                        {[90, 80, 110].map((w, i) => (
                            <Skeleton
                                key={i}
                                variant="rounded"
                                width={w}
                                height={24}
                                animation={animation}
                                sx={{ borderRadius: 999 }}
                            />
                        ))}
                    </Stack>
                }
                action={
                    <Stack direction="row" alignItems="center" gap={1}>
                        <Skeleton variant="circular" width={36} height={36} animation={animation} />
                        <Skeleton variant="circular" width={36} height={36} animation={animation} />
                    </Stack>
                }
                slotProps={{ subheader: { component: "div" } }}
            />
        </Card>
    );
}