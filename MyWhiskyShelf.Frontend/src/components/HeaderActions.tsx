import * as React from "react";
import {
    CircularProgress,
    Tooltip,
    IconButton,
    Chip,
    Menu,
    MenuItem,
    Avatar,
} from "@mui/material";

import LoginRoundedIcon from "@mui/icons-material/LoginRounded";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import { useAuth } from "../auth/AuthProvider";

function getInitials(name?: string): string {
    if (!name) return "?";
    
    const base = name.includes("@") 
        ? name.split("@")[0] 
        : name;
    
    const parts = base.replace(/[_.-]+/g, " ")
        .trim()
        .split(/\s+/);
    
    return (parts[0][0] + (parts[1]?.[0] ?? parts[0][1])).toUpperCase();
}

export default function HeaderActions() {
    const { initialized, authenticated, username, login, logout } = useAuth();

    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);
    const handleOpen = (e: React.MouseEvent<HTMLElement>) =>
        setAnchorEl(e.currentTarget);
    const handleClose = () => setAnchorEl(null);

    const initials = React.useMemo(() => getInitials(username), [username]);

    if (!initialized) return <CircularProgress size={22} color="inherit" />;

    if (authenticated) {
        return (
            <>
                <Chip
                    clickable
                    onClick={handleOpen}
                    avatar={<Avatar sx={{ width: 28, height: 28 }}>{initials}</Avatar>}
                    label={username ?? "Account"}
                    variant="outlined"
                    sx={{
                        bgcolor: "background.paper",
                        maxWidth: { xs: 180, sm: 220 },
                        "& .MuiChip-label": { overflow: "hidden", textOverflow: "ellipsis" },
                    }}
                />
                <Menu
                    anchorEl={anchorEl}
                    open={open}
                    onClose={handleClose}
                    anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
                    transformOrigin={{ vertical: "top", horizontal: "right" }}
                >
                    <MenuItem disabled>{username ?? "Account"}</MenuItem>
                    <MenuItem
                        onClick={() => {
                            handleClose();
                            logout();
                        }}
                    >
                        <LogoutRoundedIcon fontSize="small" style={{ marginRight: 8 }} /> Logout
                    </MenuItem>
                </Menu>
            </>
        );
    }

    return (
        <Tooltip title="Sign in">
            <IconButton color="inherit" size="large" onClick={login} aria-label="Sign in">
                <LoginRoundedIcon />
            </IconButton>
        </Tooltip>
    );
}