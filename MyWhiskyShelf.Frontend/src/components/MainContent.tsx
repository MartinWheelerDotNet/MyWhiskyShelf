import { Container, Typography } from "@mui/material";
import {useUser} from "../hooks/useUser.ts";

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
            <div>
                <Typography variant="h5">You’re signed in. Next step: call protected APIs.</Typography>
            </div>
            
        ) : (
            <Typography variant="h5">Welcome to MyWhiskyShelf</Typography>
        )}
        </Container>
    );
}