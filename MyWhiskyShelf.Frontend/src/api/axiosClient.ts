import axios, { 
    AxiosError, 
    type AxiosInstance,
    type InternalAxiosRequestConfig 
} from "axios";

// @ts-ignore
import { keycloakAuthProvider, type AuthProvider } from "@/auth/keycloakAdapter";

type RetriableConfig = InternalAxiosRequestConfig & { _retry?: boolean };

const apiBaseUrl = import.meta.env?.VITE_WEBAPI_URL as string | undefined;

export const axiosClient: AxiosInstance = axios.create({
    baseURL: apiBaseUrl,
    headers: { "Content-Type": "application/json" },
});

const INSTALLED = Symbol.for("mws:axios:installed");

function setAuthHeader(
    config: InternalAxiosRequestConfig,
    token: string | undefined
) {
    if (!token) return config;
    
    config.headers = config.headers ?? {};
    (config.headers as any).Authorization = `Bearer ${token}`;
    
    return config;
}

function installInterceptors(client: AxiosInstance, auth: AuthProvider) {
    if ((client as any)[INSTALLED]) return;
    (client as any)[INSTALLED] = true;

    client.interceptors.request.use(async (config: InternalAxiosRequestConfig) => {
        const token = auth.getToken?.();
        return setAuthHeader(config, token);
    });

    client.interceptors.response.use(
        (resp) => resp,
        async (error: AxiosError) => {
            const status = error.response?.status ?? 0;
            const original = error.config as RetriableConfig;
            if (!original) throw error;

            if (!original._retry && (status === 401 || status === 403)) {
                original._retry = true;

                const refreshed = await auth.refreshToken?.(10);
                if (refreshed) {
                    const token = auth.getToken?.();
                    setAuthHeader(original, token);
                    return client.request(original);
                }

                auth.clearStorage?.();
                auth.loginRedirect?.();
            }

            throw error;
        }
    );
}

installInterceptors(axiosClient, keycloakAuthProvider);