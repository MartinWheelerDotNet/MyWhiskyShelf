import * as React from "react";
import { Avatar, Chip, Menu, MenuItem } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import LogoutRoundedIcon from "@mui/icons-material/LogoutRounded";
import {COLOURS} from "../../theme/tokens.ts";

type Props = {
    username: string;
    initials: string;
    onLogout: () => void;
};

export default function AccountMenu({ username, initials, onLogout}: Readonly<Props>) {
    const theme = useTheme();
    const isDark = theme.palette.mode === "dark";
    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);

    const handleOpen = (e: React.MouseEvent<HTMLElement>) => setAnchorEl(e.currentTarget);
    const handleClose = () => setAnchorEl(null);

    return (
        <>
            <Chip
                id="account-chip"
                aria-label="Account"
                clickable
                onClick={handleOpen}
                avatar={
                    <Avatar 
                        aria-label="Account Avatar" 
                        sx={{
                            bgcolor: isDark ? "black" : "white", 
                            color: isDark ? COLOURS.light.text : COLOURS.dark.text
                        }}
                    >
                        {initials}
                    </Avatar>
                }
                sx={{
                    paddingLeft: 1.5,
                    bgcolor: isDark ? COLOURS.dark.border : COLOURS.light.border,
                    maxWidth: 28,
                    maxHeight: 28,
                    "& .MuiChip-label": { overflow: "hidden", textOverflow: "ellipsis" },
                }}
            />
            <Menu
                id="account-menu"
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
                anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
                transformOrigin={{ vertical: "top", horizontal: "right" }}
                slotProps={{ list: { "aria-labelledby": "account-chip" }}}
            >
                <MenuItem disabled>{username}</MenuItem>
                <MenuItem
                    onClick={() => {
                        handleClose();
                        onLogout();
                    }}
                >
                    <LogoutRoundedIcon fontSize="small" style={{ marginRight: 8 }} /> Logout
                </MenuItem>
            </Menu>
        </>
    );
}
