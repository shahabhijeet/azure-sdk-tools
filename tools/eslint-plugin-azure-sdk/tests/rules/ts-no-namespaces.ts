/**
 * @fileoverview Testing the ts-no-namespaces rule.
 * @author Arpan Laha
 */

import rule from "../../src/rules/ts-no-namespaces";
import { RuleTester } from "eslint";

//------------------------------------------------------------------------------
// Tests
//------------------------------------------------------------------------------

const ruleTester = new RuleTester({
  parser: "@typescript-eslint/parser",
  parserOptions: {
    project: "./tsconfig.json",
    sourceType: "module"
  },
  settings: {
    main: "test.ts"
  }
});

ruleTester.run("ts-no-namespaces", rule, {
  valid: [],
  invalid: [
    {
      code: "namespace Test {}",
      errors: [
        {
          message: "TypeScript namespaces should not be used"
        }
      ]
    },
    {
      code: "module Test {}",
      errors: [
        {
          message: "TypeScript namespaces should not be used"
        }
      ]
    }
  ]
});
