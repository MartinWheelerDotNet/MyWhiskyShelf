import { Paper, CircularProgress, Typography } from "@mui/material";
import CloseRoundedIcon from "@mui/icons-material/CloseRounded";
import Delayed from "./Delayed";

type Props = {
    loading: boolean;
    hasMore: boolean;
};

export default function LoadingPill({ loading, hasMore }: Readonly<Props>) {
    return (
        <output aria-live="polite">
            <Delayed show={loading && hasMore}>
                <Paper
                    elevation={3}
                    sx={{
                        position: "sticky",
                        bottom: 8,
                        mx: "auto",
                        width: "fit-content",
                        px: 1.5,
                        py: 0.5,
                        borderRadius: 10,
                        display: "flex",
                        alignItems: "center",
                        gap: 1,
                    }}
                >
                    <CircularProgress size={16} />
                    <Typography variant="caption">Loading more…</Typography>
                </Paper>
            </Delayed>

            {!loading && !hasMore && (
                <Paper
                    elevation={0}
                    sx={(t) => ({
                        position: "sticky",
                        bottom: 8,
                        mx: "auto",
                        width: "fit-content",
                        px: 1.2,
                        py: 0.4,
                        borderRadius: 10,
                        display: "flex",
                        alignItems: "center",
                        gap: 0.5,
                        bgcolor: t.palette.mode === "dark" ? "grey.800" : "grey.200",
                    })}
                >
                    <CloseRoundedIcon fontSize="small" sx={{ opacity: 0.7 }} />
                    <Typography variant="caption" sx={{ opacity: 0.8 }}>
                        No more results
                    </Typography>
                </Paper>
            )}
        </output>
    );
}
