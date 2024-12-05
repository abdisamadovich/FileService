import { FolderFileModel } from "./folder.file-model";
import { PaginationMetaData } from "./pagination.data";

export interface FolderGetAllModel{
    folderFile:Array<FolderFileModel>;
    paginationMetaData:PaginationMetaData;
}