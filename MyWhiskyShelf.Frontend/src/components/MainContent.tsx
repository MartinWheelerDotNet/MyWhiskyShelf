import { Container, Typography } from "@mui/material";
import { useAuth } from "../auth/AuthProvider";

export default function MainContent() {
    const { initialized, authenticated } = useAuth();

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
            <Typography variant="h5">You’re signed in. Next step: call protected APIs.</Typography>
        ) : (
            <Typography variant="h5">Welcome to MyWhiskyShelf</Typography>
        )}
        </Container>
    );
}