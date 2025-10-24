module.exports = {
  transformIgnorePatterns: [
    "node_modules/(?!(axios|some-other-esm-module)/)"
  ],
  setupFilesAfterEnv: ["<rootDir>/src/setupTests.js"],
  testEnvironment: "jsdom",
};
