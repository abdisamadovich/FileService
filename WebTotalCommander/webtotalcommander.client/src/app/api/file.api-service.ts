import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError } from 'rxjs';
import { FileDeleteModel } from '@@models/file/file.delete-model';
import { FileEditModel } from '@@models/file/file.edit-model';

@Injectable({ providedIn: 'root' })
export class FileApiService {
    //Variable HttpClient Inject
    private client: HttpClient = inject(HttpClient);

    //Variable Backend URL
    private url: string = 'api/file';

    //Function (request) Download File
    public downloadFile(filePath: string): Observable<Blob> {
        let path: string = encodeURIComponent(filePath);
        return this.client.get(`${this.url}?filePath=${path}`, {
            responseType: 'blob',
        });
    }

    //Function (request) Delete File
    public deleteFile(fileDeleteModel: FileDeleteModel): Observable<boolean> {
        return this.client.delete<boolean>(this.url, { body: fileDeleteModel });
    }

    //Function (request) Get Txt File (for edit)
    public getTxtFile(filePath: string): Observable<Blob> {
        let path: string = encodeURIComponent(filePath);
        return this.client
            .get(`${this.url}/Text?filePath=${path}`, {
                responseType: 'blob',
            }).pipe(catchError((error) => {
                throw error;
            })
            );
    }

    //Function (request) Txt File
    public editTxtFile(fileEditModel: FileEditModel): Observable<boolean> {
        let path: string = encodeURIComponent(fileEditModel.filePath);
        const formData: FormData = new FormData();
        formData.append('file', fileEditModel.file!);
        return this.client.put<boolean>(
            `${this.url}/Text?filePath=${path}`,
            formData
        );
    }
}
