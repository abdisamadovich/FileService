import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { FileApiService } from "@@api-services/file.api-service";
import { FileViewDeleteModel } from "@@viewmodels/file/file.view-delete.model";
import { FileViewEditModel } from "@@viewmodels/file/file.view-edit.model";

@Injectable({ providedIn: "root" })
export class FileService {
    //Variable Inject FolderApiService
    private fileApiService: FileApiService = inject(FileApiService)

    //Function (request) Download File
    public downloadFile(filePath: string): Observable<Blob> {
        return this.fileApiService.downloadFile(filePath);
    }

    //Function (request) Delete File
    public deleteFile(fileDeleteModel: FileViewDeleteModel): Observable<boolean> {
        return this.fileApiService.deleteFile(fileDeleteModel);
    }

    //Function (request) Get Txt File
    public getTxtFile(filePath: string): Observable<Blob> {
        return this.fileApiService.getTxtFile(filePath);
    }

    //Function (request) Edit Txt File
    public editTxtFile(fileEditModel: FileViewEditModel): Observable<boolean> {
        return this.fileApiService.editTxtFile(fileEditModel);
    }
}