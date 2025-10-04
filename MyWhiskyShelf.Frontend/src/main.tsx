import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import { ThemeModeProvider } from "./theme/ThemeModeProvider";
import { ReactKeycloakProvider } from "@react-keycloak/web";
import { KC_INIT_OPTIONS, keycloak } from "./auth/keycloak";
import { initAxiosClient } from "./api/axiosClient";

initAxiosClient();

ReactDOM.createRoot(document.getElementById("root")!).render(
    <ReactKeycloakProvider authClient={keycloak} initOptions={KC_INIT_OPTIONS}>
        <React.StrictMode>
            <ThemeModeProvider>
                <App />
            </ThemeModeProvider>
        </React.StrictMode>
    </ReactKeycloakProvider>
);