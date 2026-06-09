# Task 10 – Student Panel in Blazor

## How to run

### 1. Start the API (terminal 1)
```bash
cd StudentApi
dotnet run --launch-profile http
# Listening on http://localhost:5200
```

### 2. Start the Blazor application (terminal 2)
```bash
cd StudentPanel
dotnet run --launch-profile http
# Open http://localhost:5100 in a browser
```

> **Note:** The API must be running before the Blazor app so HTTP calls succeed on first load.

---

## Blazor variant chosen

**Blazor Web App with Interactive Server rendering** (`@rendermode InteractiveServer`).

*Why:* Server-side rendering keeps secrets (connection strings, API keys) on the server, avoids loading a large WebAssembly bundle in the browser, and makes SignalR-based real-time state updates trivial. For a university assignment panel with no offline requirement, this is the most practical choice.

---

## Where is the typed client / API communication service?

`StudentPanel/Services/StudentsApiClient.cs`

Registered in `StudentPanel/Program.cs` as a typed `HttpClient`:
```csharp
builder.Services.AddHttpClient<StudentsApiClient>(client => {
    client.BaseAddress = new Uri(apiBaseUrl);
});
```
Components never call `new HttpClient()` — they receive `StudentsApiClient` through constructor DI.

---

## Lifecycle method locations

| Method | File | Purpose |
|--------|------|---------|
| `OnInitializedAsync` | `Students.razor` | Loads the full student list on first render |
| `OnInitializedAsync` | `MainLayout.razor` | Subscribes to `ObservedStudentsState.OnChange` |
| `OnParametersSetAsync` | `StudentDetails.razor` | Reloads student data whenever the `{Id}` route parameter changes |
| `OnAfterRenderAsync(firstRender)` | `StudentDetails.razor` | Imports the JS module after the DOM is ready |
| `OnAfterRenderAsync(firstRender)` | `ObservedStudents.razor` | Imports the JS module after the DOM is ready |

---

## EditForm and validation

`StudentPanel/Components/Pages/CreateStudent.razor`
- Uses `EditForm` with `OnValidSubmit="HandleSubmit"`
- `DataAnnotationsValidator` + `ValidationMessage` per field + `ValidationSummary`
- Validates: IndexNumber (required, max 20), FirstName (required), LastName (required), Email (required + `[EmailAddress]`), Semester (`[Range(1,8)]`)

`StudentPanel/Components/Pages/StudentDetails.razor`
- Uses a second `EditForm` for assigning a course
- Validates that a course was selected (`[Range(1, int.MaxValue)]`)

---

## StateContainer location

`StudentPanel/Services/ObservedStudentsState.cs`

Registered as **Scoped** in `Program.cs`:
```csharp
builder.Services.AddScoped<ObservedStudentsState>();
```
*Scoped* means each Blazor Server circuit (browser tab) gets its own instance — safe for Blazor Server. Singleton would share state across all users.

`MainLayout` subscribes to `OnChange` event to re-render the counter badge whenever the observed list changes.

---

## JS Interop example location

`StudentPanel/wwwroot/js/interop.js` — ES module with four functions:
- `copyToClipboard(text)` – browser Clipboard API
- `confirmDialog(message)` – native confirm dialog
- `saveToLocalStorage(key, value)`
- `loadFromLocalStorage(key)`

**Used in:**
- `StudentDetails.razor` – "Copy Email" button uses `copyToClipboard`
- `ObservedStudents.razor` – "Remove" button calls `confirmDialog` before removing

Module reference is stored as `IJSObjectReference` and released in `DisposeAsync` (implements `IAsyncDisposable`).

---

## Component with RenderFragment<T>

`StudentPanel/Components/Shared/DataTable.razor`

```razor
@typeparam TItem
[Parameter] RenderFragment HeaderTemplate
[Parameter] RenderFragment<TItem> RowTemplate
```

Used in three separate places: `Students.razor`, `StudentDetails.razor` (courses table), `ObservedStudents.razor`.

---

## ErrorBoundary location

- `Students.razor` – wraps the `DataTable` rendering
- `StudentDetails.razor` – wraps the entire details card and course assign form
- `ObservedStudents.razor` – wraps the `DataTable`

`ErrorBoundary` catches unexpected render exceptions and shows a friendly message instead of crashing the whole circuit.

---

## Answers to README questions

### 1. How is `OnInitializedAsync` different from `OnParametersSetAsync`?

`OnInitializedAsync` runs **once** when the component is first created. `OnParametersSetAsync` runs **every time** a parameter value changes (including the first render). Use `OnInitializedAsync` for one-time setup (registering services, loading reference data); use `OnParametersSetAsync` when the data to load depends on a route or cascading parameter that can change while the component stays mounted (e.g. navigating from `/students/1` to `/students/2`).

### 2. Why do we usually run DOM-dependent code in `OnAfterRenderAsync`?

Blazor renders HTML in two steps: first it builds the virtual DOM tree in C#, then it applies changes to the real browser DOM. Before `OnAfterRenderAsync` fires, the real DOM elements may not exist yet. JS Interop calls that reference DOM nodes or that import JS modules must wait until after Blazor has flushed its updates to the browser.

### 3. Why should you be careful with state registered as Singleton in Blazor Server?

In Blazor Server each browser tab opens a separate SignalR circuit, but a Singleton is shared across **all** circuits on the server. That means all users see each other's state — a serious data-isolation bug. User-specific state must be **Scoped** so each circuit gets its own instance.

### 4. What does a typed client give you compared to calling HttpClient directly in every component?

A typed client centralises the base address, default headers, and retry policies in one place. Components depend on `StudentsApiClient`, not on a raw `HttpClient`, which makes them easier to test (inject a mock client), easier to change, and prevents the socket-exhaustion problem caused by `new HttpClient()` in every component.

### 5. How is `NavLink` different from a regular `<a>` link?

`NavLink` adds an `active` CSS class automatically when the current URL matches its `href`. With `Match="NavLinkMatch.All"` it requires an exact match; with the default `NavLinkMatch.Prefix` it activates for any URL that starts with the href. A plain `<a>` tag is static and has no knowledge of the current route.

### 6. What is `RenderFragment<T>` used for?

`RenderFragment<T>` is a delegate that accepts a value of type `T` and returns UI markup. It lets you write a templated component: the component provides structure (e.g. iterating a list, handling empty state), while the consumer supplies the markup for each item. The `DataTable<TItem>` here uses `RenderFragment<TItem> RowTemplate` so callers decide how each row looks without duplicating the table chrome.

### 7. When does JS Interop make sense, and when is it better to stay with Blazor?

JS Interop makes sense for browser-only APIs that Blazor does not expose: Clipboard API, `localStorage`, `confirm`/`alert`, Canvas drawing, third-party JS libraries. Stay with Blazor for everything else: event handling, DOM updates, form state, navigation. Adding JS where Blazor already handles the task creates two competing ways to update the same DOM, which leads to bugs.

### 8. What problem does `ErrorBoundary` solve, and what should it not replace?

`ErrorBoundary` catches unhandled exceptions thrown during **rendering** of its child content and shows a fallback UI instead of crashing the entire Blazor circuit. It should **not** replace normal business-error handling: API errors, validation failures, or 404 responses should be caught with `try/catch` and shown as friendly messages. `ErrorBoundary` is only a safety net for truly unexpected rendering bugs.
# apbdtask10
