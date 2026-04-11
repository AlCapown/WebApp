# Assessment Report: Semantic Kernel Agents to Agent Framework Migration

**Date**: 2025-07-10
**Repository**: `C:\Users\kyle6\source\repos\AlCapown\WebApp`
**Assessment Mode**: Scenario-Guided (Generic fallback — scenario instructions tool unavailable)
**Assessor**: Modernization Assessment Agent

---

## Executive Summary

The `WebApp.Server` project uses `Microsoft.SemanticKernel 1.74.0` to power an AI-driven background job (`SummarizeLastWeeksResults`) that summarizes football game prediction results. The implementation uses the low-level `IChatCompletionService` + `ChatHistory` orchestration pattern together with a `[KernelFunction]`-annotated plugin (`UserGamePredictionPlugin`). This is the pre-Agent-Framework approach to building SK-powered agents.

No `Microsoft.SemanticKernel.Agents.Core` or `Microsoft.SemanticKernel.Agents.OpenAI` package is currently referenced. The migration target is to replace the manual chat-completion orchestration loop with a first-class `ChatCompletionAgent` from the dedicated Agent Framework, which was introduced as a stable, standalone set of packages alongside SK 1.x.

The scope is narrow — one background job file and one plugin class in a single project — making this a low-risk, medium-effort migration. No breaking changes are expected to other parts of the solution.

---

## Scenario Context

**Scenario Objective**: Migrate from the manual Semantic Kernel `IChatCompletionService` + `ChatHistory` agent pattern to the official Semantic Kernel Agent Framework (`ChatCompletionAgent` / `AgentThread`).

**Assessment Scope**: `WebApp.Server` project — specifically the AI orchestration layer, plugin registration, and background job that drives AI inference.

**Methodology**: Static analysis of source files, NuGet dependency inspection, and code-pattern review.

---

## Current State Analysis

### Repository Overview

The solution contains five projects:

| Project | Role |
|---|---|
| `WebApp.Server` | ASP.NET Core host — contains all AI code |
| `WebApp.Client` | Blazor WebAssembly front-end |
| `WebApp.Common` | Shared models/constants |
| `WebApp.Database` | EF Core data layer |
| `ESPN.Service` | External ESPN API integration |

All projects target **.NET 10** with **C# 14**. The AI integration is entirely contained within `WebApp.Server`.

---

### Relevant Findings

#### 1. Semantic Kernel Version

**Current State**: `Microsoft.SemanticKernel 1.74.0`

**Observations**:
- Version 1.74.0 is a recent release; the core SK package already ships with Agent Framework types in the `Microsoft.SemanticKernel.Agents` namespace via the supplemental package `Microsoft.SemanticKernel.Agents.Core`.
- The meta-package `Microsoft.SemanticKernel` does **not** include Agents by default — a separate NuGet package is required.
- No `Microsoft.SemanticKernel.Agents.*` package is referenced in `WebApp.Server.csproj`.

**Relevance**: A new package reference must be added as part of the migration.

---

#### 2. AI Orchestration Pattern in `SummarizeLastWeeksResults`

**File**: `Server/Jobs/SummarizeLastWeeksResults.cs`

**Current State**: Manual `IChatCompletionService` + `ChatHistory` orchestration

**Observations**:
- Resolves `IChatCompletionService` directly from the `Kernel` via `_kernel.GetRequiredService<IChatCompletionService>()`.
- Creates a `ChatHistory` instance and adds a user message manually.
- Passes `OpenAIPromptExecutionSettings` with `FunctionChoiceBehavior = FunctionChoiceBehavior.Required()` to trigger plugin function calling.
- Calls `chatCompletionService.GetChatMessageContentAsync(history, executionSettings, _kernel, cancellationToken)` — a single-turn inference call.
- Reads `result.Content` for the final summary string.

**Agent Framework Equivalent**:
- A `ChatCompletionAgent` is constructed with the `Kernel`, optional instructions, and execution settings.
- Chat is conducted via an `AgentGroupChat` or directly via `ChatCompletionAgent.InvokeAsync(AgentThread)`.
- The Agent Framework handles the message-history lifecycle and function-calling loop internally.

**Relevance**: This is the primary method to be migrated. The logic is a single-turn completion (no multi-agent or multi-turn loop), so the migration will replace manual orchestration with a `ChatCompletionAgent` invocation.

---

#### 3. Plugin Registration (`UserGamePredictionPlugin`)

**File**: `Server/OpenAIPlugins/UserGamePredictionPlugin.cs`

**Current State**: Standard `[KernelFunction]` + `[Description]` annotated class, registered via `kernel.Plugins.AddFromObject(...)` in `ConfigureOpenAIServices.cs`.

**Observations**:
- The plugin is a standard SK plugin using `[KernelFunction]` — this attribute is fully compatible with the Agent Framework and requires **no changes**.
- The plugin is scoped (registered as `AddScoped`) and injected into the `Kernel` at construction time.
- `SearchGamePredictionAsync` is the only kernel function exposed.

**Relevance**: No migration changes needed for the plugin itself. The plugin attachment mechanism to the Agent may need to reference the same `Kernel` instance or be re-attached to the `ChatCompletionAgent`.

---

#### 4. Kernel Registration (`ConfigureOpenAIServices`)

**File**: `Server/Infrastructure/ConfigureOpenAIServices.cs`

**Current State**: `Kernel` registered as a scoped service via `Kernel.CreateBuilder()`, with `AzureOpenAIChatCompletion` and `UserGamePredictionPlugin` added at build time.

**Observations**:
- The `Kernel` is scoped — a new instance is created per HTTP/background request.
- Azure OpenAI connection details (`DeploymentName`, `Endpoint`, `ApiKey`) are sourced from `IOptions<AzureOpenAI>`.
- `UserGamePredictionPlugin` is added to the kernel before it is returned, meaning the agent will automatically have access to it.
- `Microsoft.Extensions.AI` is imported but does not appear to be used actively in this file.

**Relevance**: The `Kernel` setup is already well-structured for Agent Framework usage. `ChatCompletionAgent` accepts a `Kernel` directly, so the DI registration will require only minor changes (or none at all, if the agent is constructed inline using the same scoped `Kernel`).

---

#### 5. `OpenAIPromptExecutionSettings` / `FunctionChoiceBehavior`

**File**: `Server/Jobs/SummarizeLastWeeksResults.cs`

**Current State**: `FunctionChoiceBehavior.Required()` is set on `OpenAIPromptExecutionSettings`.

**Observations**:
- `FunctionChoiceBehavior.Required()` forces the model to call at least one function. This is a strong constraint.
- In the Agent Framework, execution settings are passed to the `ChatCompletionAgent` constructor or via `AgentInvokeOptions`.
- `OpenAIPromptExecutionSettings` is fully compatible with Agent Framework — no API change needed for settings themselves.

**Relevance**: The execution settings instance will move from the manual call site into the agent construction/invocation.

---

## Issues and Concerns

### High Priority Issues

1. **Missing `Microsoft.SemanticKernel.Agents.Core` Package**
   - **Description**: The Agent Framework types (`ChatCompletionAgent`, `AgentThread`, `ChatHistoryAgentThread`) live in `Microsoft.SemanticKernel.Agents.Core`, which is not referenced.
   - **Impact**: Migration cannot proceed without adding this package. It must be version-aligned with the existing `Microsoft.SemanticKernel 1.74.0`.
   - **Evidence**: `WebApp.Server.csproj` dependencies — no `Microsoft.SemanticKernel.Agents.*` package present.
   - **Severity**: High (blocker for migration, not a runtime bug today)

---

### Medium Priority Issues

2. **Single-Turn vs. Agent Invocation Pattern Mismatch**
   - **Description**: `SummarizeLastWeeksResults.SummarizeResults()` uses a single `GetChatMessageContentAsync` call. The Agent Framework's `InvokeAsync` method can return a stream of `ChatMessageContent` items (async enumerable) and may iterate multiple turns for function calling.
   - **Impact**: The consuming code must be updated to collect the final response message from the agent's response stream.
   - **Evidence**: `result = await chatCompletionService.GetChatMessageContentAsync(...)` — single-item return vs. `IAsyncEnumerable<ChatMessageContent>` from agent invocation.
   - **Severity**: Medium

3. **`FunctionChoiceBehavior.Required()` Behavioural Equivalence**
   - **Description**: With `FunctionChoiceBehavior.Required()`, the model is forced to call a function. In the Agent Framework, this setting should be passed via `PromptExecutionSettings` on the agent, but care must be taken that the behaviour is preserved.
   - **Impact**: If not carried over correctly, the AI may return a text response without calling the plugin, causing incorrect/empty summaries.
   - **Evidence**: `FunctionChoiceBehavior = FunctionChoiceBehavior.Required()` in `SummarizeLastWeeksResults.cs:122`.
   - **Severity**: Medium

---

### Low Priority Issues

4. **`Microsoft.Extensions.AI` Import in `ConfigureOpenAIServices`**
   - **Description**: `using Microsoft.Extensions.AI` is present but does not appear actively used in `ConfigureOpenAIServices.cs`.
   - **Impact**: Minor — unused using directive. Not a blocker.
   - **Evidence**: Line 3 of `ConfigureOpenAIServices.cs`.
   - **Severity**: Low

5. **No `IAgent` / `AgentThread` DI Registration Pattern Established**
   - **Description**: There is currently no DI registration pattern for Agent Framework types. The `ChatCompletionAgent` will likely need to be constructed per-invocation within the job class.
   - **Impact**: Minor architectural decision required during planning — whether to register the agent as a service or construct it inline.
   - **Severity**: Low

---

## Risks and Considerations

### Identified Risks

1. **Package Version Compatibility**
   - **Description**: `Microsoft.SemanticKernel.Agents.Core` must exactly align with the installed `Microsoft.SemanticKernel` version (1.74.0). A version mismatch may cause runtime assembly binding failures.
   - **Likelihood**: Low (versioning is deterministic if pinned correctly)
   - **Impact**: High (runtime failure)
   - **Mitigation**: Pin `Microsoft.SemanticKernel.Agents.Core` to `1.74.0`.

2. **Behavioural Difference in Multi-Turn Scenarios**
   - **Description**: If the AI model decides to make multiple sequential function calls, the Agent Framework handles this automatically in its invocation loop. The current single-call pattern would not cover this.
   - **Likelihood**: Low (current prompt uses `FunctionChoiceBehavior.Required()` with clear single intent)
   - **Impact**: Medium (could produce different/richer results or loop longer than expected)
   - **Mitigation**: Test the migrated agent with real/mock prompts to verify output equivalence.

3. **Azure OpenAI Endpoint Compatibility**
   - **Description**: Depending on the Azure OpenAI deployment's API version, some agent framework features (e.g., streaming tool calls) may require a specific API version header.
   - **Likelihood**: Low (SK manages API versioning internally)
   - **Impact**: Low

### Assumptions

- `Microsoft.SemanticKernel.Agents.Core` 1.74.0 is available on NuGet (it is, as of SK 1.x stable releases).
- The Azure OpenAI deployment supports function/tool calling (required for `FunctionChoiceBehavior`).
- No other files beyond those identified use `IChatCompletionService` in an agent-like pattern.

### Unknowns and Areas Requiring Further Investigation

- Whether `Microsoft.SemanticKernel.Agents.OpenAI` is needed (for `OpenAIAssistantAgent`) — based on current code, it is **not** needed; `Agents.Core` is sufficient.
- Whether the `Microsoft.Extensions.AI` import in `ConfigureOpenAIServices.cs` is intentional (reserved for future use or leftover).

---

## Opportunities and Strengths

### Existing Strengths

1. **Narrow AI Surface Area**
   - The entire AI integration is contained in 3 files: `SummarizeLastWeeksResults.cs`, `UserGamePredictionPlugin.cs`, and `ConfigureOpenAIServices.cs`. This makes the migration low-risk and easy to reason about.

2. **Well-Structured Plugin**
   - `UserGamePredictionPlugin` uses idiomatic `[KernelFunction]` + `[Description]` attributes. It requires no changes for Agent Framework compatibility.

3. **Scoped `Kernel` DI Pattern**
   - The scoped `Kernel` factory in `ConfigureOpenAIServices` is a clean pattern. `ChatCompletionAgent` can be constructed directly from the scoped `Kernel` instance with minimal changes.

4. **Up-to-Date SK Version**
   - `Microsoft.SemanticKernel 1.74.0` is a recent, stable release that ships alongside Agent Framework 1.74.0, ensuring API stability and compatibility.

---

## Recommendations for Planning Stage

> **Note**: These are observations for the planner — not a migration plan.

### Prerequisites

- Confirm NuGet availability of `Microsoft.SemanticKernel.Agents.Core 1.74.0`.
- Establish a baseline test for `SummarizeLastWeeksResults` output before migration (to verify behavioural equivalence after migration).

### Focus Areas for Planning

1. **Package Addition**: Add `Microsoft.SemanticKernel.Agents.Core 1.74.0` to `WebApp.Server.csproj`.
2. **`SummarizeResults` Method Refactor**: Replace `IChatCompletionService` + `ChatHistory` with `ChatCompletionAgent` construction and invocation.
3. **Execution Settings Preservation**: Ensure `FunctionChoiceBehavior.Required()` is correctly passed to the agent.
4. **Response Consumption Update**: Adapt `result.Content` extraction to handle `IAsyncEnumerable<ChatMessageContent>` from agent invocation.

### Suggested Approach

The migration is a targeted refactor of a single method (`SummarizeResults`) within one background job. The plugin, DI registration, and all other project code remain unchanged.

---

## Data for Planning Stage

### Key Metrics and Counts

- **Files requiring changes**: 2 (`SummarizeLastWeeksResults.cs`, `WebApp.Server.csproj`)
- **Files confirmed no change needed**: 2 (`UserGamePredictionPlugin.cs`, `ConfigureOpenAIServices.cs`)
- **New NuGet packages required**: 1 (`Microsoft.SemanticKernel.Agents.Core`)
- **SK namespaces actively used in migration scope**:
  - `Microsoft.SemanticKernel` ✅ keep
  - `Microsoft.SemanticKernel.ChatCompletion` — may be replaced by Agent Framework types
  - `Microsoft.SemanticKernel.Connectors.OpenAI` ✅ keep (for `OpenAIPromptExecutionSettings`)

### Inventory of Relevant Items

**AI-Related Source Files**:
- `Server/Jobs/SummarizeLastWeeksResults.cs` — primary migration target
- `Server/OpenAIPlugins/UserGamePredictionPlugin.cs` — no changes needed
- `Server/Infrastructure/ConfigureOpenAIServices.cs` — likely no changes needed

**NuGet Packages (AI-Related)**:
- `Microsoft.SemanticKernel 1.74.0` — current
- `Microsoft.SemanticKernel.Agents.Core` — to be added

### Dependencies and Relationships

```
SummarizeLastWeeksResults
  └── depends on: Kernel (scoped DI)
        └── configured by: ConfigureOpenAIServices.AddOpenAIServices()
              ├── AzureOpenAIChatCompletion (Azure OpenAI backend)
              └── UserGamePredictionPlugin (KernelFunction plugin)
```

---

## Assessment Artifacts

### Tools Used

- `upgrade_get_project_dependencies` — NuGet and project reference inventory
- `get_files_in_project` — file enumeration
- `get_file` — source code analysis
- `code_search` — SK/Agent pattern detection across solution

### Files Analyzed

- `Server/Jobs/SummarizeLastWeeksResults.cs`
- `Server/OpenAIPlugins/UserGamePredictionPlugin.cs`
- `Server/Infrastructure/ConfigureOpenAIServices.cs`
- `Server/Program.cs`
- `Server/WebApp.Server.csproj` (via dependency tool)

---

## Conclusion

The `WebApp.Server` project has a focused, well-structured Semantic Kernel integration that is ready for Agent Framework migration. The codebase currently uses the manual `IChatCompletionService` + `ChatHistory` orchestration pattern — the pre-Agent-Framework approach. The migration scope is confined to one background job method and one project file update (adding a new NuGet package). No other projects in the solution are affected.

The primary migration action is replacing the manual chat-completion invocation in `SummarizeLastWeeksResults.SummarizeResults()` with a `ChatCompletionAgent` construction and invocation, ensuring `FunctionChoiceBehavior.Required()` execution settings are preserved and the response is correctly extracted from the agent's async enumerable output.

**Next Steps**: This assessment is ready for the Planning stage, where a detailed migration plan will be created based on these findings.

---

*This assessment was generated by the Assessment Agent to support the Planning and Execution stages of the modernization workflow.*
