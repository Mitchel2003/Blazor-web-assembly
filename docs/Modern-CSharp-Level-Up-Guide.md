# 📚 Level-Up Guide: Modern C# for Seasoned Front-End Engineers

> *“Think of C#/.NET as React + TypeScript on server-grade steroids: the same ergonomics of components, hooks, and compile-time safety—plus a toolbox for raw performance, cloud-scale, and domain-driven design.”*

---

## 1. Language Superpowers

| Category             | Why care?                     | Key Constructs                        | Cheat-Tips                                                                 |
|----------------------|-------------------------------|---------------------------------------|----------------------------------------------------------------------------|
| Functional Touch     | Expressive, null-safe code    | `record`, `with`, `Span<T>`           | Treat **record** as an immutable TS interface + structural equality        |
| Async & Concurrency  | React’s event-loop, but server-side | `IAsyncEnumerable<T>`, `Channel<T>`, `ValueTask` | `useEffect` polling with `await foreach` on a gRPC stream |
| Low-Allocation Perf  | GC pauses = jank              | `Span<T>`, `Memory<T>`, `stackalloc`, `ref struct` | Use `Span<char>` for CSV parsing without allocations          |
| Reflection-Free Meta | Compile-time everything       | **Source Generators** (Roslyn)        | Auto-generate DTO mappers → zero runtime reflection                        |

---

## 2. Reactive & State Management (Hooks Analogues)

| Need                       | .NET Option                           | Quick Start                                               |
|----------------------------|---------------------------------------|-----------------------------------------------------------|
| Memoisation (`useMemo`)    | `Lazy<T>` / `MemoryCache`             | `var result = _cache.GetOrCreate(key, _ => Expensive());` |
| Side-effects (`useEffect`) | `IHostedService`, `BackgroundService` | Run cron-like jobs or Kafka consumers                     |
| Signals / Observables      | **System.Reactive** (Rx.NET)          | `obs.Where(x => x > 5).Throttle(TimeSpan.FromSeconds(1))` |
| Component Re-Render        | `INotifyPropertyChanged`, Blazor `ObservableObject` | Same pattern used in `TableUserPageVM`      |

---

## 3. Architectural Gold Standards

1. **CQRS + MediatR** – command/query separation keeps write & read models clean.
2. **Pipeline Behaviors** – like Redux middleware: validation, logging, transactions.
3. **Clean Architecture** – Onion layers: Domain → Application → Infrastructure → Presentation.
4. **Vertical Slice** – Feature-first folders (`Features/Users`) over n-tier blobs.
5. **DDD Aggregates & Value Objects** – push invariants into the core, not the DB.

---

## 4. Toolchain & Diagnostics

| Tool                              | Purpose                                            | Analogy                     |
|-----------------------------------|----------------------------------------------------|-----------------------------|
| **dotnet CLI**                    | `dotnet new`, `dotnet add package`, `dotnet watch` | `npm` / `yarn`              |
| Visual Studio / Rider             | Rich debugger, hot-reload, profilers               | VSCode + DevTools           |
| ReSharper / Roslyn Analyzers      | Static analysis & refactors                        | ESLint + Prettier           |
| `dotnet-trace`, `dotnet-counters` | Prod CPU / allocations                             | Chrome perf tab             |
| PerfView / dotTrace               | Deep flamegraphs                                   | webpack bundle-analyser—CPU |
| `dotnet-ef`                       | DB migrations                                      | Prisma CLI                  |

> Install global: `dotnet tool install -g dotnet-ef`

---

## 5. Libraries You Should Master

| Area          | Package                                        | Why It Rocks                                                                 |
|---------------|------------------------------------------------|------------------------------------------------------------------------------|
| Mapping       | **Mapster** / AutoMapper SourceGen             | Compile-time generators → zero reflection                                    |
| Validation    | **FluentValidation**                           | Declarative rules, reusable across API + UI                                  |
| GraphQL       | **HotChocolate**                               | Schema-first, DataLoader perf                                                |
| Caching       | `IMemoryCache`, `IDistributedCache`, EasyCache | Swap in-proc ↔ Redis seamlessly                                              |
| Messaging     | **MassTransit**                                | Same API for RabbitMQ & Azure Service Bus                                    |
| Observability | Serilog + Seq, **OpenTelemetry**               | Structured logs + distributed traces                                         |

---

## 6. Performance Playbook (10-Year Veteran Tricks)

1. **Measure first** – `BenchmarkDotNet` for micro-benchmarks.
2. **Avoid boxing** – generic constraints, `struct` enumerables.
3. **Pool aggressively** – `ArrayPool<T>` & `ObjectPool<T>` kill GC spikes.
4. **Async all the way** – never `.Result` / `.Wait()`. Use `ValueTask` for hot paths.
5. **Minimise virtual calls** – mark classes `sealed` where inheritance isn’t needed.
6. **DB round-trips** – project to DTOs with Mapster `.ProjectToType<>()`.
7. **Stream everything** – chunk uploads via `IAsyncEnumerable<byte[]>`.

---

## 7. Testing & CI

| Kind               | Framework    | Highlight                                                                 |
|--------------------|--------------|---------------------------------------------------------------------------|
| Unit               | **xUnit**    | `[Theory]` data-driven tests                                              |
| Snapshot           | **Verify**   | Jest-style snapshot testing for C#                                        |
| BDD                | **SpecFlow** | Gherkin specs → living docs                                               |
| Component (Blazor) | **bUnit**    | Render-tree diff & DOM queries                                            |
| Integration        | **Respawn**  | DB reset between tests in <30 ms                                          |

**GitHub Actions** snippet:
```yaml
- name: Test
  run: dotnet test --no-build --verbosity normal
```

---

## 8. Migration Mindset – React ► C#

| React Habit          | C# Counterpart                                                   |
|----------------------|------------------------------------------------------------------|
| Stateless components | `record` types or static helpers                                 |
| Context API          | Built-in DI (`services.AddScoped<T>()`)                          |
| Suspense / lazy      | `Task<T>` + `await`                                              |
| Hooks                | Event/Observable patterns (`OnParametersSetAsync` ≈ `useEffect`) |

---

## 9. Curiosities & Hidden Gems

* **Interpolated string handlers** – alloc-free logging: `$"User {id} failed"`.
* **UTF-8 literals** – `"Hello"u8` compile-time UTF-8 spans.
* **File-scoped namespaces** – `namespace Foo;` removes braces.
* `dotnet watch --hot-reload` – live reload Razor + C# like Vite.
* **Minimal APIs** – `app.MapGet("/hi", () => "👋");` = 5 lines to prod.
* `Enumerable.Append/Prepend` – chain lists without extra enumerations.
* **NativeAOT** – single-file, self-contained exes, startup in milliseconds.

---

## 10. 6-Week Learning Roadmap

| Week | Focus                      | Deliverable                                                       |
|------|----------------------------|-------------------------------------------------------------------|
| 1    | Modern language features   | Kata repo: convert TS utils → C# records + LINQ                   |
| 2    | Async & Channels           | Real-time chat via `IAsyncEnumerable` + SignalR                   |
| 3    | EF Core + Mapster          | CRUD API with soft-delete & DTO projections                       |
| 4    | CQRS + MediatR             | Split commands/queries, add validation pipeline                   |
| 5    | Observability              | Serilog + OpenTelemetry + Grafana dashboards                      |
| 6    | Performance & NativeAOT    | Benchmark hot paths, publish single-file exe                      |

---

### Further Reading

* *CLR via C#* – Jeffrey Richter  
* *Domain-Driven Design* – Eric Evans  
* Microsoft performance docs: <https://aka.ms/dotnet-perf>  
* Blazor docs: <https://learn.microsoft.com/aspnet/core/blazor>

---

> **Next Step:** Port a small slice of a React side-project to Blazor WASM using these patterns. Your TypeScript instincts + these C# tools will make you lethal in full-stack clean architecture.
