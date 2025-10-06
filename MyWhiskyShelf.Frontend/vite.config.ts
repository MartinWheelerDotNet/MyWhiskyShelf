import { defineConfig } from "vite";
import path from "node:path";
import react from "@vitejs/plugin-react";

export default defineConfig(() => {
    const port = Number(process.env.VITE_PORT) || 5173;

    const apiTarget =
        process.env["services__webapi__https__0"] ||
        process.env["services__webapi__http__0"];

    return {
        plugins: [react()],
        server: {
            port,
            strictPort: true,
            host: true,
            proxy: apiTarget
                ? {
                    "/api": {
                        target: apiTarget,
                        changeOrigin: true,
                        secure: false
                    }
                }
                : undefined
        },
        build: { sourcemap: true },
        resolve: {
            alias: {
                "@": path.resolve(__dirname, "src"),
            },
        },
    };
});