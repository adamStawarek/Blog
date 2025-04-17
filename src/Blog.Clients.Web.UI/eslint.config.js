// @ts-check
const eslint = require("@eslint/js");
const tseslint = require("typescript-eslint");
const angular = require("angular-eslint");

module.exports = tseslint.config(
  {
    files: ["**/*.ts"],
    ignores: ["**/*.generated.ts"],
    extends: [
      eslint.configs.recommended,
      ...tseslint.configs.recommended,
      ...tseslint.configs.stylistic,
      ...angular.configs.tsRecommended,
    ],
    processor: angular.processInlineTemplates,
    rules: {
      "@angular-eslint/directive-selector": [
        "error",
        {
          type: "attribute",
          prefix: "app",
          style: "camelCase",
        },
      ],
      "@angular-eslint/component-selector": [
        "error",
        {
          type: "element",
          prefix: "app",
          style: "kebab-case",
        },
      ],
      "no-console": "error",
      "no-multiple-empty-lines": "error",
      "@typescript-eslint/member-ordering": "error",
      "@typescript-eslint/naming-convention": [
        "error",
        {
          "selector": "property",
          "modifiers": [
            "private"
          ],
          "format": [
            "camelCase"
          ],
          "leadingUnderscore": "require"
        }
      ],
      "@typescript-eslint/explicit-function-return-type": "error",
      "@typescript-eslint/explicit-member-accessibility": [
        "error",
        {
          "accessibility": "explicit",
          "ignoredMethodNames": [
            "ngOnInit",
            "ngOnDestroy",
            "ngOnChanges",
            "ngAfterViewInit"
          ],
          "overrides": {
            "constructors": "off"
          }
        }
      ],
      "@angular-eslint/template/click-events-have-key-events": "off"
    },
  },
  {
    files: ["**/*.html"],
    extends: [
      ...angular.configs.templateRecommended,
      ...angular.configs.templateAccessibility,
    ],
    rules: {
      "no-multiple-empty-lines": [
        "error",
        {
          "max": 1
        }
      ]
    },
  }
);
