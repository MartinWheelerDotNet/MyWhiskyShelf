import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App";
import { AuthProvider } from "./auth/AuthProvider";
import {ThemeModeProvider} from "./theme/ThemeModeProvider.tsx";

ReactDOM.createRoot(document.getElementById("root")!).render(
    <React.StrictMode>
        <ThemeModeProvider>
            <AuthProvider>
                <App />
            </AuthProvider>
        </ThemeModeProvider>
    </React.StrictMode>
);