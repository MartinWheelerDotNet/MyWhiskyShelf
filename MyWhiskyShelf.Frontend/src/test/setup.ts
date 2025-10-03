import "@testing-library/jest-dom/vitest";
import { afterEach, beforeAll, afterAll } from "vitest";
import { cleanup } from "@testing-library/react";

import "@testing-library/jest-dom";

const origError = console.error;
beforeAll(() => {
    console.error = (...args) => {
        const msg = args[0] as string | undefined;
        if (msg?.includes("act(")) return;
        origError(...args);
    };
});
afterAll(() => { console.error = origError; });
afterEach(() => {
    cleanup();
});