# Contributing

All contributions must follow the rules below to stay consistent with our automation and downstream consumers.

## Expectations
- Use English for all code, documentation, and commit messages.
- Follow the trunk-based workflow implied by GitVersion; prefer short-lived feature branches merged via pull requests.
- Keep production code under `src/` and test projects under `tests/` using the `{ProjectName}.Tests.Unit` / `{ProjectName}.Tests.Integration` pattern.
- Add NuGet packages without versions in project files; define versions centrally in `Directory.Packages.props` only.
- Leave repository-wide config files (e.g., `.editorconfig`, `Directory.Build.props`, `Directory.Packages.props` structure) unchanged unless explicitly requested.

## Commit messages (required)
Commit messages **must** follow the [Conventional Commits 1.0.0](https://www.conventionalcommits.org/en/v1.0.0/) format:

```text
<type>[optional scope]: <description>
```

Allowed types include `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`, `build`, `ci`, `perf`, and `revert`. Indicate breaking changes with `!` after the type/scope or a `BREAKING CHANGE:` footer.

## Development workflow
- The `global.json` file pins test runner and in some cases the required SDK version; see README.md for the required .NET SDK version and any additional prerequisites.
- Restore, build, and test from the solution root before opening a pull request: `dotnet restore`, `dotnet build`, `dotnet test`.
- Execute code formatting using `csharpier format .` from the solution root.
- Keep code and documentation clear and concise; prefer small, focused pull requests.
- Update documentation when behavior or public surface changes.

## Pull requests
- Provide a short summary, the motivation for the change, and any relevant issue links.
- List the tests you ran and the outcomes.
- Ensure new code includes appropriate tests (unit or integration) placed under the matching `tests/` project.
- Avoid reformatting unrelated code or introducing drive-by changes.

## Dependency updates
We use Renovate Bot to create automated dependency update pull requests using conventional commit prefixes. When adding new dependencies manually, follow the same conventions:
- Declare the version in `Directory.Packages.props`.
- Reference the package in the project file without a version attribute.
- Prefer the smallest viable dependency set; remove unused packages.