import { Container, Typography } from "@mui/material";

// @ts-ignore
import {useUser} from "@/hooks/useUser";
import DistilleriesDashboard from "../pages/DistilleriesDashboard";

export default function MainContent() {
    const { initialized, authenticated } = useUser();

    if (!initialized) {
        return (
            <Container sx={{ py: 4 }}>
                <Typography>Preparing…</Typography>
            </Container>
        );
    }

    return (
        <Container sx={{ py: 4 }}>
        {authenticated ? (
            <DistilleriesDashboard />
        ) : (
            <Typography variant="h5">Welcome to MyWhiskyShelf</Typography>
        )}
        </Container>
    );
}