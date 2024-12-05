import { FolderFileViewModel } from "./folder.file.view-model";
import { PaginationMetaDataView } from "./pagination.data";

export class FolderGetAllViewModel {
    public folderFile: Array<FolderFileViewModel> = [];
    public paginationMetaData: PaginationMetaDataView = new PaginationMetaDataView();
}