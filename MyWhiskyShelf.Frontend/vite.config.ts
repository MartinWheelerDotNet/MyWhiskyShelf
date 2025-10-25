import { defineConfig, loadEnv } from "vite";
import path from "node:path";
import react from "@vitejs/plugin-react";

export default defineConfig(( { mode }) => {
    const env = loadEnv(mode, process.cwd(), '')

    return {
        plugins: [react()],
        server: {
            port: parseInt(env.VITE_PORT) || 5173,
            strictPort: true,
            host: true,
            proxy: {
                "/api": {
                    target: process.env.services__webapi__https__0 || process.env.services__webapi__http__0,
                    changeOrigin: true,
                    secure: false,
                    rewrite: (path) => path.replace(/^\/api/, '')
                }
            }
        },
        build: { sourcemap: true },
        resolve: {
            alias: {
                "@": path.resolve(__dirname, "src"),
            },
        },
    };
});