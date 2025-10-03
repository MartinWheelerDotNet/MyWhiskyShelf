import axios, { AxiosError, type AxiosInstance, type AxiosRequestConfig } from "axios";
import { keycloak } from "../auth/keycloak";

const API_BASE = import.meta.env.VITE_API_BASE;

export type ApiConfig = AxiosRequestConfig & {
    autoLoginOn401?: boolean;
};

export const api: AxiosInstance = axios.create({
    baseURL: API_BASE
});

const RETRY_MARKER = "__kc_retry__";

api.interceptors.request.use(async (config) => {
    if (keycloak.authenticated) {
        try {
            await keycloak.updateToken(30);
        } catch {}
    }

    const token = keycloak.token;
    if (token) {
        config.headers = config.headers ?? {};
        config.headers.Authorization = `Bearer ${token}`;
    }

    const isFormData =
        typeof config.data === "object" &&
        config.data != null &&
        typeof (config.data).append === "function";

    if (!isFormData) {
        const headers = config.headers as Record<string, any>;
        if (!headers["Content-Type"] && config.data !== undefined) {
            headers["Content-Type"] = "application/json";
        }
    }

    return config;
});

api.interceptors.response.use(
    (res) => res,
    async (error: AxiosError) => {
        const cfg = (error.config ?? {}) as ApiConfig & { [RETRY_MARKER]?: boolean };

        if (error.response?.status === 401 && !cfg[RETRY_MARKER]) {
            cfg[RETRY_MARKER] = true;

            if (keycloak.authenticated) {
                try {
                    await keycloak.updateToken(60);
                    return api.request(cfg);
                } catch {}
            }

            if (cfg.autoLoginOn401) {
                await keycloak.login({ redirectUri: globalThis.location.href });
                return new Promise(() => {});
            }
        }

        throw error;
    }
);