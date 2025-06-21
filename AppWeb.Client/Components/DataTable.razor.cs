using Microsoft.AspNetCore.Components;
namespace AppWeb.Client.Components;

public partial class DataTable<TItem> : ComponentBase
{
    /// <summary> Items to render in the table body. </summary>
    [Parameter, EditorRequired] public IReadOnlyList<TItem>? Items { get; set; }

    /// <summary> Template that defines how each row is rendered. </summary>
    [Parameter, EditorRequired] public RenderFragment<TItem> RowTemplate { get; set; } = default!;

    /// <summary> Optional header content (e.g. column headers). </summary>
    [Parameter] public RenderFragment? HeaderContent { get; set; }

    /// <summary> Text shown when <see cref="Items"/> is null or empty. </summary>
    [Parameter] public string EmptyText { get; set; } = "No records found.";

    /// <summary> Page size for pagination. Set to 0 to disable pagination. </summary>
    [Parameter]
    public int PageSize { get; set; } = 10;
    private int _currentPage = 1;
    protected int CurrentPage => _currentPage;
    protected bool IsEmpty => Items is null || Items.Count == 0;
    protected int TotalPages => IsEmpty || PageSize <= 0 ? 1 : (int)Math.Ceiling(Items!.Count / (double)PageSize);

    protected IReadOnlyList<TItem> PagedItems => IsEmpty || PageSize <= 0
        ? Items ?? Array.Empty<TItem>()
        : Items!.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

    protected bool DisableNext => CurrentPage >= TotalPages;
    protected bool DisablePrev => CurrentPage <= 1;

    protected void PrevPage() { if (!DisablePrev) SetPage(CurrentPage - 1); }
    protected void NextPage() { if (!DisableNext) SetPage(CurrentPage + 1); }
    private void SetPage(int page) { _currentPage = Math.Clamp(page, 1, TotalPages); }
}
