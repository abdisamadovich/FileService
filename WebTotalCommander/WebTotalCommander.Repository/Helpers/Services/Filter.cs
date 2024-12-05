using System.Globalization;
using WebTotalCommander.FileAccess.Models.Common;
using WebTotalCommander.Repository.Helpers.Interfaces;

namespace WebTotalCommander.Repository.Helpers.Services;

public class Filter : IFilter
{
    public List<FolderFileModel> FilterFolder(FilterModel filter, List<FolderFileModel> folderList)
    {
        var folderFilterColumns = new List<List<FolderFileModel>>();
        for (var i = 0; i < filter.Filters.Count; i++)
        {
            var columnName = filter.Filters[i].Filters[0].Field;
            if (!String.IsNullOrEmpty(columnName))
            {
                switch (columnName)
                {
                    case "name":
                        folderFilterColumns.Add(FilterNameColumn(filter.Filters[i], folderList));

                        break;

                    case "extension":
                        folderFilterColumns.Add(FilterExtensionColumn(filter.Filters[i], folderList));

                        break;

                    case "createdDate":
                        folderFilterColumns.Add(FilterCreatedDateColumn(filter.Filters[i], folderList));

                        break;

                    case "size":
                        folderFilterColumns.Add(FilterSizeColumn(filter.Filters[i], folderList));

                        break;
                }
            }
        }

        var folderFilterResult = new List<FolderFileModel>();
        folderFilterResult = folderFilterColumns[0].Slice(0, folderFilterColumns[0].Count);
        if (folderFilterColumns.Count > 1)
        {
            for (var i = 1; i < folderFilterColumns.Count; i++)
            {
                folderFilterResult = folderFilterResult.Intersect(folderFilterColumns[i]).ToList();
            }
        }

        return folderFilterResult;
    }

    private static List<FolderFileModel> FilterNameColumn(SubFilterModel filter, List<FolderFileModel> folderList)
    {
        var folderFilterListByName = new List<List<FolderFileModel>>();
        foreach (var item in filter.Filters)
        {
            switch (item.Operator)
            {
                case "contains":
                    folderFilterListByName.Add(folderList.Where(x => x.Name.Contains(item.Value)).ToList());
                    break;
                case "doesnotcontain":
                    folderFilterListByName.Add(folderList.Where(x => !x.Name.Contains(item.Value)).ToList());
                    break;
                case "eq":
                    folderFilterListByName.Add(folderList.Where(x => x.Name == item.Value).ToList());
                    break;
                case "neq":
                    folderFilterListByName.Add(folderList.Where(x => x.Name != item.Value).ToList());
                    break;
                case "startswith":
                    folderFilterListByName.Add(folderList.Where(x => x.Name.StartsWith(item.Value)).ToList());
                    break;
                case "endswith":
                    folderFilterListByName.Add(folderList.Where(x => x.Name.EndsWith(item.Value)).ToList());
                    break;
            }
        }
        if (folderFilterListByName.Count > 1)
        {
            if (filter.Logic == "and")
            {
                if (folderFilterListByName[1].Count > 0)
                {
                    if (folderFilterListByName[0].Intersect(folderFilterListByName[1]).ToList().Count > 0)
                    {
                        return folderFilterListByName[0].Intersect(folderFilterListByName[1]).ToList();
                    }
                    else
                    {
                        return folderFilterListByName[0];
                    }
                }
                else
                {
                    return folderFilterListByName[1];
                }
            }
            else
            {
                return folderFilterListByName[0].Concat(folderFilterListByName[1]).Distinct().ToList();
            }
        }
        else
        {
            return folderFilterListByName[0];
        }

    }

    private static List<FolderFileModel> FilterExtensionColumn(SubFilterModel filter, List<FolderFileModel> folderList)
    {
        var folderFilterListByExtension = new List<List<FolderFileModel>>();
        foreach (var item in filter.Filters)
        {
            switch (item.Operator)
            {
                case "contains":
                    folderFilterListByExtension.Add(folderList.Where(x => x.Extension.Contains(item.Value)).ToList());
                    break;
                case "doesnotcontain":
                    folderFilterListByExtension.Add(folderList.Where(x => !x.Extension.Contains(item.Value)).ToList());
                    break;
                case "eq":
                    folderFilterListByExtension.Add(folderList.Where(x => x.Extension == item.Value).ToList());
                    break;
                case "neq":
                    folderFilterListByExtension.Add(folderList.Where(x => x.Extension != item.Value).ToList());
                    break;
                case "startswith":
                    folderFilterListByExtension.Add(folderList.Where(x => x.Extension.StartsWith(item.Value)).ToList());
                    break;
                case "endswith":
                    folderFilterListByExtension.Add(folderList.Where(x => x.Extension.EndsWith(item.Value)).ToList());
                    break;
            }
        }
        if (folderFilterListByExtension.Count > 1)
        {
            if (filter.Logic == "and")
            {
                if (folderFilterListByExtension[1].Count > 0)
                {
                    if (folderFilterListByExtension[0].Intersect(folderFilterListByExtension[1]).ToList().Count > 0)
                    {
                        return folderFilterListByExtension[0].Intersect(folderFilterListByExtension[1]).ToList();
                    }
                    else
                    {
                        return folderFilterListByExtension[0];
                    }
                }
                else
                {
                    return folderFilterListByExtension[1];
                }

            }
            else
            {
                return folderFilterListByExtension[0].Concat(folderFilterListByExtension[1]).Distinct().ToList();
            }
        }
        else
        {
            return folderFilterListByExtension[0];
        }
    }

    private static List<FolderFileModel> FilterCreatedDateColumn(SubFilterModel filter, List<FolderFileModel> folderList)
    {
        const string dateTimeFormat = "yyyy/MM/dd hh:mm tt";
        var folderFilterListByCreatedDate = new List<List<FolderFileModel>>();
        foreach (var item in filter.Filters)
        {
            var dateInFilter = DateTime.Parse(item.Value);
            var itemValueDate = DateTime.ParseExact(dateInFilter.ToString(dateTimeFormat), dateTimeFormat, CultureInfo.InvariantCulture);
            switch (item.Operator)
            {
                case "eq":
                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.CreatedDate.ToString(dateTimeFormat) == itemValueDate.ToString(dateTimeFormat)).ToList());
                    break;
                case "neq":
                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.CreatedDate.ToString(dateTimeFormat) != itemValueDate.ToString(dateTimeFormat)).ToList());
                    break;
                case "gte":
                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.CreatedDate >= itemValueDate).ToList());
                    break;
                case "gt":
                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.CreatedDate > itemValueDate).ToList());
                    break;
                case "lte":
                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.CreatedDate <= itemValueDate).ToList());
                    break;
                case "lt":
                    folderFilterListByCreatedDate.Add(folderList.Where(x => x.CreatedDate < itemValueDate).ToList());
                    break;
            }
        }

        if (folderFilterListByCreatedDate.Count > 1)
        {
            if (filter.Logic == "and")
            {
                if (folderFilterListByCreatedDate[1].Count > 0)
                {
                    if (folderFilterListByCreatedDate[0].Intersect(folderFilterListByCreatedDate[1]).ToList().Count > 0)
                    {
                        return folderFilterListByCreatedDate[0].Intersect(folderFilterListByCreatedDate[1]).ToList();
                    }
                    else
                    {
                        return folderFilterListByCreatedDate[0];
                    }
                }
                else
                {
                    return folderFilterListByCreatedDate[1];
                }
            }
            else
            {
                return folderFilterListByCreatedDate[0].Concat(folderFilterListByCreatedDate[1]).Distinct().ToList();
            }
        }
        else
        {
            return folderFilterListByCreatedDate[0];
        }
    }

    private static List<FolderFileModel> FilterSizeColumn(SubFilterModel filter, List<FolderFileModel> folderList)
    {
        var folderFilterListBySize = new List<List<FolderFileModel>>();
        foreach (var item in filter.Filters)
        {
            switch (item.Operator)
            {
                case "eq":
                    folderFilterListBySize.Add(folderList.Where(x => x.Size == Double.Parse(item.Value)).ToList());
                    break;
                case "neq":
                    folderFilterListBySize.Add(folderList.Where(x => x.Size != Double.Parse(item.Value)).ToList());
                    break;
                case "gte":
                    folderFilterListBySize.Add(folderList.Where(x => x.Size >= Double.Parse(item.Value)).ToList());
                    break;
                case "gt":
                    folderFilterListBySize.Add(folderList.Where(x => x.Size > Double.Parse(item.Value)).ToList());
                    break;
                case "lte":
                    folderFilterListBySize.Add(folderList.Where(x => x.Size <= Double.Parse(item.Value)).ToList());
                    break;
                case "lt":
                    folderFilterListBySize.Add(folderList.Where(x => x.Size < Double.Parse(item.Value)).ToList());
                    break;
            }
        }
        if (folderFilterListBySize.Count > 1)
        {
            if (filter.Logic == "and")
            {
                if (folderFilterListBySize[1].Count > 0)
                {
                    if (folderFilterListBySize[0].Intersect(folderFilterListBySize[1]).ToList().Count > 0)
                    {
                        return folderFilterListBySize[0].Intersect(folderFilterListBySize[1]).ToList();
                    }
                    else
                    {
                        return folderFilterListBySize[0];
                    }
                }
                else
                {
                    return folderFilterListBySize[1];
                }
            }
            else
            {
                return folderFilterListBySize[0].Concat(folderFilterListBySize[1]).Distinct().ToList();
            }
        }
        else
        {
            return folderFilterListBySize[0];
        }
    }

}