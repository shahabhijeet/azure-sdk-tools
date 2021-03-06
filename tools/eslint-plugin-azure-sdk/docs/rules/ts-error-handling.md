# ts-error-handling

Limits thrown errors to ECMAScript built-in error types (`TypeError`, `RangeError`, `Error`).

Allows exceptions for `catch` blocks where the type of the caught error is unknown.

## Examples

### Good

```ts
throw new Error("error");
```

```ts
const err = new TypeError("invalid type");
throw err;
```

```ts
try {
  /* code here */
} catch (err) {
  throw err;
}
```

### Bad

```ts
throw 1;
```

```ts
throw "error";
```

```ts
throw new OtherError("error");
```

```ts
try {
  /* code here */
} catch (err) {
  throw err;
}
```

## [Source](https://azuresdkspecs.z5.web.core.windows.net/TypeScriptSpec.html#ts-error-handling)
