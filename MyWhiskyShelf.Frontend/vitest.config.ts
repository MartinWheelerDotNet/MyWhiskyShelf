/// <reference types="vitest/config" />
import { defineConfig } from "vitest/config";
import path from "node:path";

export default defineConfig({
    test: {
        environment: "jsdom",
        setupFiles: ["/src/test/setup.ts"],
        globals: true,
        reporters: ["default", "junit"],
        outputFile: "./TestResults/vitest-junit.xml",
        includeSource: ["src/*.{ts,tsx}"],
        coverage: {
            provider: "v8",
            include: ['src/**.{ts,tsx}'],
            exclude: [
                "src/**/__tests__/**",
                "src/**/*.test.*",
                "src/test/**",
                "src/**/stories/**",
                "**/node_modules/**",
                "**/dist/**",
            ],
            reporter: ["lcov", "html", "text-summary"],
            reportsDirectory: "coverage",
        },
        environmentOptions: {
            jsdom: { url: "http://localhost/" },
        },
    },
    resolve: {
        alias: {
            "@": path.resolve(__dirname, "src"),
        },
    },
});