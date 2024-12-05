using WebTotalCommander.FileAccess.Utils;

namespace WebTotalCommander.Repository.Helpers.Interfaces;

public interface IPaginator
{
    public PaginationMetaData Paginate(long itemsCount, PaginationParams @params);
}
