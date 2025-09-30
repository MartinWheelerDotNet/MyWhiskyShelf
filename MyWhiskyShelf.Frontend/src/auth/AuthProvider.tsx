import React, { createContext, useContext, useEffect, useMemo, useRef, useState } from "react";
import keycloak, { initOptions } from "./keycloak";

type AuthContextModel = {
    initialized: boolean;
    authenticated: boolean;
    username?: string;
    token?: string;
    login: () => Promise<void>;
    logout: () => Promise<void>;
    register: () => Promise<void>;
};

const AuthContext = createContext<AuthContextModel | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [initialized, setInitialized] = useState(false);
    const [authenticated, setAuthenticated] = useState(false);
    const [username, setUsername] = useState<string>();
    const didInitRef = useRef(false); // ✅ guard against React StrictMode double-mount

    useEffect(() => {
        if (didInitRef.current) {
            // Already initialized in this session (e.g., StrictMode re-run)
            setInitialized(true);
            return;
        }
        didInitRef.current = true;

        const run = async () => {
            try {
                // Initialize Keycloak once per page load
                const auth = await keycloak.init(initOptions);
                setAuthenticated(!!auth);
                if (auth) {
                    try {
                        const profile = await keycloak.loadUserProfile();
                        setUsername(profile?.username || profile?.email || profile?.firstName);
                    } catch {
                        // profile optional
                    }
                }
            } catch (e) {
                // eslint-disable-next-line no-console
                console.error("Keycloak init failed", e);
            } finally {
                setInitialized(true);
            }
        };

        // Auth-related events keep UI in sync after redirects/refreshes
        keycloak.onAuthSuccess = async () => {
            setAuthenticated(true);
            try {
                const profile = await keycloak.loadUserProfile();
                setUsername(profile?.username || profile?.email || profile?.firstName);
            } catch {}
        };

        keycloak.onAuthError = () => setAuthenticated(false);

        keycloak.onAuthLogout = () => {
            setAuthenticated(false);
            setUsername(undefined);
        };

        keycloak.onTokenExpired = async () => {
            try {
                await keycloak.updateToken(30);
            } catch {
                setAuthenticated(false);
            }
        };

        run();
    }, []);

    const api = useMemo<AuthContextModel>(() => ({
        initialized,
        authenticated,
        username,
        token: keycloak.token,
        login: async () => keycloak.login({ redirectUri: window.location.origin }),
        logout: async () => keycloak.logout({ redirectUri: window.location.origin }),
        register: async () => keycloak.register({ redirectUri: window.location.origin })
    }), [initialized, authenticated, username]);

    return <AuthContext.Provider value={api}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error("useAuth must be used within <AuthProvider>");
    return ctx;
};