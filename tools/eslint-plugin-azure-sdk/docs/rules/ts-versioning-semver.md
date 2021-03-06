# ts-versioning-semver

Requires `version` in `package.json` to be in [SemVer](https://semver.org/).

Additionally, major versions of `0` are not permitted, preview versions must be in the format `<version>-preview.<preview-version>`, where `preview-version` is an integer.

## Examples

### Good

```json
{
  "version": "1.0.0"
}
```

```json
{
  "version": "1.0.0-preview.1"
}
```

### Bad

```json
{
  "version": "1.0.01"
}
```

```json
{
  "version": "1.0"
}
```

```json
{
  "version": "1"
}
```

```json
{
  "version": "0.0.1"
}
```

```json
{
  "version": "1.0.0.0"
}
```

```json
{
  "version": "1.0.0-preview1"
}
```

```json
{
  "version": "1.0.0-preview.1.0"
}
```

```json
{
  "version": "1.0.0-Preview.1"
}
```

```json
{}
```

## [Source](https://azuresdkspecs.z5.web.core.windows.net/TypeScriptSpec.html#ts-versioning-semver)

Also encompasses [ts-versioning-no-version-0](https://azuresdkspecs.z5.web.core.windows.net/TypeScriptSpec.html#ts-versioning-no-version-0), as the rules are similar enough to not exist separately for linting purposes.
