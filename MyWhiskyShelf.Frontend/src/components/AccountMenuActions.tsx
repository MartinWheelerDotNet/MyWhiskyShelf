import LoadingAction from "./header/LoadingAction";
import SignInAction from "./header/SignInAction";
import AccountMenu from "./header/AccountMenu";
import {useUser} from "../hooks/useUser.ts";

export default function AccountMenuActions() {
    const { initialized, authenticated, username, firstName, lastName, email, login, logout } = useUser();
    const initials =
        (firstName?.charAt(0).toUpperCase() || "W") + (lastName?.charAt(0).toUpperCase() || "S"); 
    
    if (!initialized) return <LoadingAction />;
    return authenticated ? (
        <AccountMenu username={username || email || "Account" } initials={initials} onLogout={logout} />
    ) : (
        <SignInAction onLogin={() => login()} />
    );
}