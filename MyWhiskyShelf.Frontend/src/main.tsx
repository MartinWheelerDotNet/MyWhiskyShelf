import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import {ThemeModeProvider} from "./theme/ThemeModeProvider.tsx";
import {ReactKeycloakProvider} from "@react-keycloak/web";
import {KC_INIT_OPTIONS, keycloak} from "./auth/keycloak.ts"

ReactDOM.createRoot(document.getElementById("root")!).render(
    <React.StrictMode>
        <ReactKeycloakProvider
            authClient={keycloak}
            initOptions={{ KC_INIT_OPTIONS }}
        >
            <ThemeModeProvider>
                <App />
            </ThemeModeProvider>
        </ReactKeycloakProvider>
    </React.StrictMode>
);