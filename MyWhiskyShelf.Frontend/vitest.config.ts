import { defineConfig } from "vitest/config";
import path from "node:path";

export default defineConfig({
    test: {
        environment: "jsdom",
        setupFiles: ["/src/test/setup.ts"],
        globals: true,
        reporters: ["default", "junit"],
        outputFile: "./TestResults/vitest-junit.xml",
        coverage: {
            provider: "v8",
            reporter: ["lcov", "html", "text-summary"],
            reportsDirectory: "coverage",
            include: ["src/**/*.{ts,tsx}"],
            exclude: ["**/*.test.*", "**/__tests__/**", "test/**"],
        },
    },
    resolve: {
        alias: {
            "@": path.resolve(__dirname, "src"),
        },
    },
});