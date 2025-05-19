import { defineConfig } from "cypress";

export default defineConfig({
  e2e: {
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
    baseUrl: 'http://localhost:3000',
    video: false,
    screenshotOnRunFailure: true,
    defaultCommandTimeout: 10000,
    requestTimeout: 10000,
    responseTimeout: 10000,
    pageLoadTimeout: 30000,
    viewportWidth: 1280,
    viewportHeight: 720,
    chromeWebSecurity: false,
    retries: {
      runMode: 2,
      openMode: 0
    },
    specPattern: 'cypress/e2e/**/*.{js,jsx,ts,tsx}',
    supportFile: 'cypress/support/e2e.ts'
  },
});
