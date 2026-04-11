# Migration Plan: Semantic Kernel Agents to Agent Framework

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Migration Strategy](#2-migration-strategy)
3. [Detailed Dependency Analysis](#3-detailed-dependency-analysis)
4. [Project-by-Project Migration Plans](#4-project-by-project-migration-plans)
   - 4.1 [WebApp.Server](#41-webappserver)
5. [Risk Management](#5-risk-management)
6. [Testing & Validation Strategy](#6-testing--validation-strategy)
7. [Complexity & Effort Assessment](#7-complexity--effort-assessment)
8. [Source Control Strategy](#8-source-control-strategy)
9. [Success Criteria](#9-success-criteria)

---

## 1. Executive Summary

### Scenario Description

Migrate the `WebApp.Server` project from the manual Semantic Kernel `IChatCompletionService` + `ChatHistory` orchestration pattern to the official Semantic Kernel Agent Framework (`ChatCompletionAgent`). This modernizes the AI layer of the `SummarizeLastWeeksResults` background job to use first-class agent abstractions introduced in SK 1.x, improving maintainability and aligning with the current Semantic Kernel SDK direction.

### Scope

| Item | Detail |
|---|---|
| **Projects affected** | `WebApp.Server` only |
| **Files requiring changes** | 2 (`SummarizeLastWeeksResults.cs`, `WebApp.Server.csproj`) |
| **Files confirmed unchanged** | 2 (`UserGamePredictionPlugin.cs`, `ConfigureOpenAIServices.cs`) |
| **Current SK version** | `Microsoft.SemanticKernel 1.74.0` |
| **New package required** | `Microsoft.SemanticKernel.Agents.Core 1.74.0` |
| **Target .NET** | .NET 10 (no framework change) |

### Current State

- `Microsoft.SemanticKernel 1.74.0` installed; no `Agents.*` package present.
- `SummarizeLastWeeksResults` manually resolves `IChatCompletionService` from the `Kernel`, constructs a `ChatHistory`, and calls `GetChatMessageContentAsync` with `FunctionChoiceBehavior.Required()`.
- `UserGamePredictionPlugin` is a standard `[KernelFunction]` plugin — fully Agent Framework compatible with no changes needed.

### Target State

- `Microsoft.SemanticKernel.Agents.Core 1.74.0` added to `WebApp.Server.csproj`.
- `SummarizeLastWeeksResults.SummarizeResults()` replaced with `ChatCompletionAgent` construction and invocation.
- `FunctionChoiceBehavior.Required()` preserved via agent execution settings.
- All other files, projects, and configurations remain unchanged.

### Selected Strategy

**All-At-Once Strategy** — All changes applied simultaneously in a single coordinated operation.

**Rationale**:
- Scope is minimal: 1 project, 2 files, 1 new package.
- No dependency tiers or intermediate states exist.
- Risk is low — no other projects are affected, and the plugin layer is unchanged.
- The change is atomic by nature: package + code update must be verified together.

### Complexity Assessment

**Simple** — single project, narrow file scope, no circular dependencies, no security vulnerabilities, no multi-project coordination required.

### Critical Issues

| Priority | Issue | Description |
|---|---|---|
| High | Missing package | `Microsoft.SemanticKernel.Agents.Core 1.74.0` must be added before any code changes |
| Medium | Response collection pattern | `IAsyncEnumerable<ChatMessageContent>` from agent invocation replaces single `GetChatMessageContentAsync` result |
| Medium | Execution settings placement | `FunctionChoiceBehavior.Required()` must be correctly attached to `ChatCompletionAgent` |

---

## 2. Migration Strategy

### Approach Selection

**All-At-Once** — The entire migration is a single coordinated batch: add the NuGet package, refactor one method, verify build, run tests.

**Justification**:
- The AI integration is self-contained within `WebApp.Server` and does not cross project boundaries.
- The `UserGamePredictionPlugin` and `ConfigureOpenAIServices` registrations require zero code changes — they are already Agent Framework compatible.
- There is no meaningful intermediate state: the project either uses `IChatCompletionService` directly or uses `ChatCompletionAgent`. There is no value in partial migration.
- Reverting is trivial (one method + one package reference).

### Dependency-Based Ordering

No project dependency ordering is required — only `WebApp.Server` is affected. Within the single project, the natural order is:

1. **Add NuGet package** → unlocks Agent Framework types
2. **Update source code** → replace manual orchestration with `ChatCompletionAgent`
3. **Restore & build** → verify compilation
4. **Run tests** → verify runtime behaviour (if test project exists)

### Execution Approach

**Atomic operation** — all changes applied in one pass with no intermediate commits until the build is green.

### Phase Definitions

| Phase | Content |
|---|---|
| Phase 0 | Pre-verification (confirm package availability) |
| Phase 1 | Atomic upgrade (package + code + build) |
| Phase 2 | Test validation |

---

## 3. Detailed Dependency Analysis

### Solution Overview

```
WebApp.Server (migration target)
  ├── Microsoft.SemanticKernel 1.74.0            ← keep, unchanged
  ├── Microsoft.SemanticKernel.Agents.Core        ← ADD 1.74.0
  ├── WebApp.Client (project ref)                ← unaffected
  ├── WebApp.Common (project ref)                ← unaffected
  ├── WebApp.Database (project ref)              ← unaffected
  └── ESPN.Service (project ref)                 ← unaffected
```

### AI Layer Internal Dependency Graph

```
SummarizeLastWeeksResults (job)
  └── Kernel                    ← injected (scoped DI)
        ├── IChatCompletionService (AzureOpenAI backend)
        └── UserGamePredictionPlugin
              └── [KernelFunction] SearchGamePredictionAsync
```

**Post-migration**:

```
SummarizeLastWeeksResults (job)
  └── Kernel                    ← injected (scoped DI) — unchanged
        ├── AzureOpenAIChatCompletion (backend) — unchanged
        └── UserGamePredictionPlugin             — unchanged
              └── [KernelFunction] SearchGamePredictionAsync — unchanged

  └── ChatCompletionAgent       ← NEW — constructed inline from Kernel
        └── AgentThread         ← NEW — scoped per invocation
```

### Project Groupings

All work is in a **single group** — `WebApp.Server`. No tier or phase separation is needed.

### Critical Path

`WebApp.Server.csproj` (add package) → `SummarizeLastWeeksResults.cs` (refactor method) → `dotnet restore` → `dotnet build` → tests.

### Circular Dependencies

None identified.

---

## 4. Project-by-Project Migration Plans

## 4. Project-by-Project Migration Plans

### 4.1 WebApp.Server

**Current State**

| Property | Value |
|---|---|
| Target framework | `net10.0` |
| SK package | `Microsoft.SemanticKernel 1.74.0` |
| Agents package | Not referenced |
| AI orchestration | Manual `IChatCompletionService` + `ChatHistory` |
| Plugin pattern | `[KernelFunction]` on `UserGamePredictionPlugin` |
| Risk level | Low |

**Target State**

| Property | Value |
|---|---|
| Target framework | `net10.0` (unchanged) |
| SK package | `Microsoft.SemanticKernel 1.74.0` (unchanged) |
| Agents package | `Microsoft.SemanticKernel.Agents.Core 1.74.0` (new) |
| AI orchestration | `ChatCompletionAgent` + `AgentThread` |
| Plugin pattern | `[KernelFunction]` on `UserGamePredictionPlugin` (unchanged) |

---

#### Migration Steps

##### Step 1 — Add NuGet Package Reference

**File**: `Server/WebApp.Server.csproj`

Add the following package reference:

```xml
<PackageReference Include="Microsoft.SemanticKernel.Agents.Core" Version="1.74.0" />
```

**Placement**: Alongside the existing `Microsoft.SemanticKernel` reference for consistency.

**Version rationale**: Must match the installed `Microsoft.SemanticKernel 1.74.0` to avoid assembly binding conflicts. Version `1.74.0` is confirmed available on NuGet for `net10.0`.

---

##### Step 2 — Refactor `SummarizeLastWeeksResults.SummarizeResults()`

**File**: `Server/Jobs/SummarizeLastWeeksResults.cs`

**Current implementation summary**:
```csharp
var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
var executionSettings = new OpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Required()
};
history.AddUserMessage(inputPrompt);
var result = await chatCompletionService.GetChatMessageContentAsync(
    history, executionSettings, _kernel, cancellationToken);
return result.Content ?? string.Empty;
```

**Target implementation**:

Replace the above block with a `ChatCompletionAgent` construction and invocation:

```csharp
using Microsoft.SemanticKernel.Agents;

// Construct the agent with the scoped Kernel and execution settings
var agent = new ChatCompletionAgent
{
    Kernel = _kernel,
    Instructions = null,  // no system-level instructions; prompt is the user message
    ExecutionSettings = new OpenAIPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Required()
    }
};

// Create a thread scoped to this invocation
var thread = new ChatHistoryAgentThread();

// Collect the assistant response(s)
var responseContent = new System.Text.StringBuilder();
await foreach (var response in agent.InvokeAsync(
    new ChatMessageContent(AuthorRole.User, inputPrompt),
    thread,
    cancellationToken: cancellationToken))
{
    if (response.Role == AuthorRole.Assistant)
        responseContent.Append(response.Content);
}

return responseContent.ToString();
```

**Key changes explained**:

| Change | Reason |
|---|---|
| Remove `IChatCompletionService` resolution | Agent Framework manages chat completion internally |
| Remove manual `ChatHistory` construction | `ChatHistoryAgentThread` manages history lifecycle |
| `ChatCompletionAgent` with `ExecutionSettings` | Replaces manual `OpenAIPromptExecutionSettings` at the call site |
| `FunctionChoiceBehavior.Required()` on agent | Preserves the original constraint forcing a function call |
| `IAsyncEnumerable` iteration | Agent Framework returns a stream; collect all `Assistant` role messages |
| `ChatHistoryAgentThread` | Lightweight thread scoped per invocation, no persistence required |

**Using directive changes**:

| Directive | Action |
|---|---|
| `using Microsoft.SemanticKernel.ChatCompletion;` | Remove if `IChatCompletionService` and `ChatHistory` are no longer used directly |
| `using Microsoft.SemanticKernel.Agents;` | Add — provides `ChatCompletionAgent`, `ChatHistoryAgentThread` |
| `using Microsoft.SemanticKernel;` | Keep — `ChatMessageContent`, `AuthorRole`, `OpenAIPromptExecutionSettings`, `FunctionChoiceBehavior` |
| `using Microsoft.SemanticKernel.Connectors.OpenAI;` | Keep — `OpenAIPromptExecutionSettings` |

**⚠️ Verification**: After the refactor, confirm that the method signature `Task<string> SummarizeResults(...)` is preserved — only the internal implementation changes.

---

##### Step 3 — Restore Dependencies

Run `dotnet restore` on `WebApp.Server.csproj` to pull `Microsoft.SemanticKernel.Agents.Core 1.74.0` and update the lock file.

---

##### Step 4 — Build and Fix Compilation Errors

Build the entire solution. Expected compilation issues are limited to:

- Any residual `IChatCompletionService` or `ChatHistory` references in `SummarizeLastWeeksResults.cs` if the old code was not fully removed.
- Missing `using Microsoft.SemanticKernel.Agents;` directive if not added.

No compilation errors are expected in any other project or file.

---

##### Step 5 — Validation Checklist

- [ ] `WebApp.Server.csproj` includes `Microsoft.SemanticKernel.Agents.Core 1.74.0`
- [ ] `SummarizeLastWeeksResults.cs` no longer references `IChatCompletionService` or `ChatHistory` directly in `SummarizeResults()`
- [ ] `ChatCompletionAgent` is constructed with the scoped `_kernel`
- [ ] `FunctionChoiceBehavior.Required()` is present on the agent's `ExecutionSettings`
- [ ] `ChatHistoryAgentThread` is used per-invocation (not reused across calls)
- [ ] Solution builds with 0 errors and 0 warnings related to the change
- [ ] `UserGamePredictionPlugin.cs` is unchanged
- [ ] `ConfigureOpenAIServices.cs` is unchanged

---

## 5. Risk Management

### High-Risk Changes

| Project | Risk Level | Description | Mitigation |
|---|---|---|---|
| `WebApp.Server` | Medium | `IAsyncEnumerable` response collection replaces single-result `GetChatMessageContentAsync` — incorrect iteration could produce empty or duplicate output | Collect only `AuthorRole.Assistant` messages; append `Content` to a `StringBuilder`; verify output is non-empty |
| `WebApp.Server` | Medium | `FunctionChoiceBehavior.Required()` must be on the agent's `ExecutionSettings`, not a separate call-site variable | Always set `ExecutionSettings` on `ChatCompletionAgent` constructor; do not pass settings separately to `InvokeAsync` |
| `WebApp.Server` | Low | Package version mismatch between `Microsoft.SemanticKernel` and `Microsoft.SemanticKernel.Agents.Core` | Pin `Agents.Core` to exactly `1.74.0` — the same as the existing SK package |

### Security Vulnerabilities

None identified. No security-related packages are being changed.

### Contingency Plans

| Scenario | Contingency |
|---|---|
| `ChatCompletionAgent.InvokeAsync` returns no `Assistant` messages | Fall back to iterating all response messages regardless of role and concatenating non-empty `Content` values |
| Agent Framework produces different AI output from original pattern | Acceptable — the AI model output is non-deterministic; functional equivalence (function called, summary returned) is the acceptance criterion, not exact text match |
| `Microsoft.SemanticKernel.Agents.Core 1.74.0` unavailable | Check NuGet.org for the nearest matching patch version; update both `Microsoft.SemanticKernel` and `Agents.Core` to the same version together |
| Build fails with assembly binding conflict | Inspect the conflict message; ensure both packages are pinned to identical versions; clear NuGet cache and restore again |

---

## 6. Testing & Validation Strategy

### Phase 1: Atomic Upgrade — Build Validation

After completing Step 4 (build):

- **Expected outcome**: `dotnet build` exits with 0 errors.
- **Spot-check**: Open `SummarizeLastWeeksResults.cs` — confirm no remaining `IChatCompletionService` or standalone `ChatHistory` usage inside `SummarizeResults()`.
- **Spot-check**: Confirm `UserGamePredictionPlugin.cs` and `ConfigureOpenAIServices.cs` are bit-for-bit identical to their pre-migration state.

### Phase 2: Test Validation

Discover and run any test projects that cover `SummarizeLastWeeksResults` or the AI orchestration layer.

> ⚠️ **Note**: The assessment did not identify a dedicated test project for `WebApp.Server` AI features. If no automated test exists for this job, the validation outcome relies on build success and manual integration testing (running the background job against a real or mocked Azure OpenAI endpoint).

**If automated tests exist**:
- Run all tests in the `WebApp.Server` test project.
- Expected: all pre-existing passing tests continue to pass.

**If no automated tests exist**:
- Confirm the job can be triggered manually or via Hangfire dashboard.
- Verify the `SummarizeLastWeeksResults` job completes without exception and the `CreateGameSummary` command is invoked with a non-empty summary string.

### Smoke Test Checklist

- [ ] Solution builds with 0 errors
- [ ] `Microsoft.SemanticKernel.Agents.Core 1.74.0` appears in the NuGet restore output
- [ ] No `CS*` compiler errors in `SummarizeLastWeeksResults.cs`
- [ ] No regressions in other features (all other server endpoints still compile and respond)

---

## 7. Complexity & Effort Assessment

### Per-Project Complexity

| Project | Complexity | Files Changed | New Packages | Risk | Notes |
|---|---|---|---|---|---|
| `WebApp.Server` | **Low** | 2 | 1 | Low–Medium | Single method refactor; plugin and DI layers unchanged |

### Phase Complexity Assessment

| Phase | Complexity | Description |
|---|---|---|
| Phase 0: Pre-verification | Low | Confirm NuGet package availability |
| Phase 1: Atomic upgrade | Low | Add package + refactor one method + restore + build |
| Phase 2: Test validation | Low | Run existing tests or manual smoke test |

### Resource Requirements

| Skill | Level Required | Notes |
|---|---|---|
| Semantic Kernel SDK knowledge | Intermediate | Understanding `ChatCompletionAgent`, `ChatHistoryAgentThread`, and `IAsyncEnumerable` response iteration |
| .NET / C# | Intermediate | Standard async patterns, DI |
| Azure OpenAI | Basic awareness | No configuration changes; existing options/secrets unchanged |

---

## 8. Source Control Strategy

### Branching Strategy

- **Working branch**: Create a dedicated feature branch from `main` before making any changes (e.g., `upgrade/sk-agent-framework`).
- All migration changes are committed to this branch.
- Merge back to `main` via a pull request after validation passes.

### Commit Strategy

**Preferred approach**: Single commit for the entire atomic upgrade — package reference update + code refactor together, once the build is green.

**Commit message format**:
```
refactor(server): migrate SummarizeLastWeeksResults to SK Agent Framework

- Add Microsoft.SemanticKernel.Agents.Core 1.74.0
- Replace IChatCompletionService + ChatHistory with ChatCompletionAgent
- Preserve FunctionChoiceBehavior.Required() via agent ExecutionSettings
- Use ChatHistoryAgentThread for per-invocation history management
```

**Rationale**: A single commit keeps the package and code change atomic and makes it easy to revert if needed.

### Review and Merge Process

**Pull request checklist**:
- [ ] Build passes (`dotnet build` — 0 errors)
- [ ] `UserGamePredictionPlugin.cs` and `ConfigureOpenAIServices.cs` are unchanged (diff confirms)
- [ ] `FunctionChoiceBehavior.Required()` is present on the agent
- [ ] No unintended files modified
- [ ] Tests pass (if available)

---

## 9. Success Criteria

### Technical Criteria

- [ ] `Microsoft.SemanticKernel.Agents.Core 1.74.0` is referenced in `WebApp.Server.csproj`
- [ ] `SummarizeLastWeeksResults.SummarizeResults()` uses `ChatCompletionAgent` and `ChatHistoryAgentThread`
- [ ] `FunctionChoiceBehavior.Required()` is set on `ChatCompletionAgent.ExecutionSettings`
- [ ] No direct `IChatCompletionService` resolution or standalone `ChatHistory` construction remains in `SummarizeResults()`
- [ ] Solution builds with 0 errors
- [ ] `UserGamePredictionPlugin.cs` is byte-for-byte unchanged
- [ ] `ConfigureOpenAIServices.cs` is byte-for-byte unchanged
- [ ] All other projects (`WebApp.Client`, `WebApp.Common`, `WebApp.Database`, `ESPN.Service`) are unmodified

### Quality Criteria

- [ ] Code follows existing patterns (async/await, cancellation token propagation, `StringBuilder` for string accumulation)
- [ ] No new compiler warnings introduced
- [ ] `using` directives are clean — unused directives removed, new ones added as needed

### Process Criteria

- [ ] All-At-Once strategy followed: package + code + build in one coordinated operation
- [ ] Single commit used for the entire upgrade
- [ ] Pull request created from feature branch to `main`
- [ ] PR description references this migration plan
