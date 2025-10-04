import axios, { AxiosError, type AxiosInstance, type InternalAxiosRequestConfig } from "axios";
import { keycloak } from "../auth/keycloak";

export const axiosInstance: AxiosInstance = axios.create({
    baseURL: import.meta.env?.VITE_API_BASE_URL,
    timeout: 30000,
});

export type TokenProvider = () => string | undefined;
export type LoginAction = (redirectUri: string) => void | Promise<void>;

export interface InstallInterceptorsOptions {
    getToken?: TokenProvider;
    login?: LoginAction;
    shouldAutoLogin401?: (err: AxiosError) => boolean;
}

export interface InstalledInterceptors {
    request: (config: InternalAxiosRequestConfig) => InternalAxiosRequestConfig | Promise<InternalAxiosRequestConfig>;
    response: {
        onFulfilled?: (value: any) => any;
        onRejected: (error: any) => Promise<never> | Promise<void>;
    };
}

export function installInterceptors(opts: InstallInterceptorsOptions = {}): InstalledInterceptors {
    const {
        getToken = () => (keycloak?.authenticated ? keycloak?.token : undefined),
        login = (redirectUri: string) => keycloak?.login?.({ redirectUri }),
        shouldAutoLogin401 = (err: AxiosError) =>
            Boolean((err as any)?.config?.autoLoginOn401 === true)
    } = opts;

    const onRequest = (config: InternalAxiosRequestConfig) => {
        const lsToken = (typeof localStorage === "undefined") 
            ? undefined 
            : localStorage.getItem("token") ?? undefined;
        
        const token = lsToken ?? getToken();
        if (token) {
            config.headers = config.headers ?? {};
            (config.headers as any).Authorization = (String(token).startsWith("Bearer "))
                ? String(token)
                : `Bearer ${token}`;
        }
        return config;
    };

    const onResponseRejected = async (error: AxiosError) => {
        if (error?.response?.status === 401 && shouldAutoLogin401(error)) {
            const redirectUri = globalThis?.location?.href ?? "/";
            await Promise.resolve();
            await Promise.resolve(login(redirectUri));
            return new Promise<never>(() => {});
        }
        throw error;
    };

    axiosInstance.interceptors.request.use(onRequest);
    axiosInstance.interceptors.response.use(undefined, onResponseRejected);

    return {
        request: onRequest,
        response: { onRejected: onResponseRejected },
    };
}

export function initAxiosClient(): void {
    installInterceptors();
}
export default axiosInstance;