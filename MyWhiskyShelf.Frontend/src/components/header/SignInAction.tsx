import { Button, IconButton, Tooltip, useMediaQuery } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import LoginRoundedIcon from "@mui/icons-material/LoginRounded";

export default function SignInAction({ onLogin }: Readonly<{ onLogin: () => void }>) {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down("sm"), { noSsr: true });

    if (isMobile) {
        return (
            <Tooltip title="Sign in">
                <IconButton color="inherit" size="large" onClick={onLogin} aria-label="Sign in">
                    <LoginRoundedIcon />
                </IconButton>
            </Tooltip>
        );
    }

    return (
        <Button
            variant="outlined"
            color="inherit"
            size="small"
            onClick={onLogin}
            aria-label="Sign in / Register"
        >
            Sign in / Register
        </Button>
    );
}