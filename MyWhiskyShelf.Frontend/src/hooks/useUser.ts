import {useEffect, useMemo, useState} from "react";
import {useKeycloak} from "@react-keycloak/web";

type FreshProfile = {
    firstName?: string;
    lastName?: string;
    email?: string;
    preferredUsername?: string;
};

export function useUser() {
    const { keycloak, initialized } = useKeycloak();
    const authenticated = !!keycloak.authenticated;

    const idp = (keycloak.idTokenParsed ?? {}) as Record<string, any>;
    const atp = (keycloak.tokenParsed ?? {}) as Record<string, any>;

    const tokenFirstName = idp.given_name ?? atp.given_name;
    const tokenLastName = idp.family_name ?? atp.family_name;
    const tokenEmail = idp.email ?? atp.email;
    const tokenPreferredUsername =
        idp.preferred_username ?? atp.preferred_username;

    const [fresh, setFresh] = useState<FreshProfile>({});

    useEffect(() => {
        let cancelled = false;

        if (!authenticated) {
            setFresh({});
            return;
        }

        keycloak
            .loadUserInfo()
            .then((u) => {
                if (cancelled) return;
                const obj = u as Record<string, any>;
                setFresh({
                    firstName: obj.given_name ?? obj.firstName,
                    lastName: obj.family_name ?? obj.lastName,
                    email: obj.email,
                    preferredUsername: obj.preferred_username ?? obj.username,
                });
            })
            .catch(() => {});

        return () => {
            cancelled = true;
        };
    }, [authenticated, keycloak]);

    const firstName = fresh.firstName ?? (tokenFirstName as string | undefined);
    const lastName = fresh.lastName ?? (tokenLastName as string | undefined);
    const email = fresh.email ?? (tokenEmail as string | undefined);
    const username =
        fresh.preferredUsername ??
        (tokenPreferredUsername as string | undefined) ??
        firstName ??
        email;

    return useMemo(
        () => ({
            initialized,
            authenticated,
            sub: (idp.sub ?? atp.sub) as string | undefined,
            username,
            firstName,
            lastName,
            email,
            token: keycloak.token,
            login: () => keycloak.login({redirectUri: globalThis.location.origin}),
            logout: () => keycloak.logout({redirectUri: globalThis.location.origin}),
            register: () => keycloak.register({redirectUri: globalThis.location.origin}),
        }),
        [initialized, authenticated, idp.sub, atp.sub, username, firstName, lastName, email, keycloak.token]
    );
}