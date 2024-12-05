namespace WebTotalCommander.FileAccess.Utils;

public class PaginationParams
{
    public PaginationParams(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }
    public int Skip { get; set; }
    public int Take { get; set; }

}
