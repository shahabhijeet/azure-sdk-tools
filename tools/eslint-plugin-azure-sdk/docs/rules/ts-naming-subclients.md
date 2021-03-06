# ts-naming-subclients

Requires client methods returning a subclient to have names prefixed suffixed with "get" and suffixed with "client".

## Examples

### Good

```ts
class ServiceClient {
  getSubClient(): SubClient {
    /* code to return instance of SubClient */
  }
}
```

```ts
// private methods are ignored
class ServiceClient {
  private _get(): SubClient {
    /* code to return instance of SubClient */
  }
}
```

### Bad

```ts
class ServiceClient {
  get(): SubClient {
    /* code to return instance of SubClient */
  }
}
```

## [Source](https://azuresdkspecs.z5.web.core.windows.net/TypeScriptSpec.html#ts-naming-subclients)
