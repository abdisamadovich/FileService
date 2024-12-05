import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { FolderCreateModel } from "@@models/folder/folder.create-model";
import { FolderDeleteModel } from "@@models/folder/folder.delete-model";
import { FolderGetAllModel } from "@@models/common/folder.getall-model";
import { SortModel } from "@@models/common/sort.model";
import { SubFilter } from "@@viewmodels/common/filter/sub-filter";


@Injectable({ providedIn: "root" })
export class FolderApiService {
    //Variable HttpClient Inject
    private client: HttpClient = inject(HttpClient);
    //Variable Backend URL
    private urlMain: string = "api/folder"

    //Function (request) GetAll Folders and Files
    public getAllFolder(folderPath: string, skip: number, take: number, sort?: SortModel,
        filters?: { 'Filter.Logic': string; 'Filter.Filters': Array<SubFilter>; }): Observable<FolderGetAllModel> {

        let path: string = encodeURIComponent(folderPath);
        let url: string = `${this.urlMain}?Path=${path}&Offset=${skip}&Limit=${take}`;

        if (filters) {
            url = this.addFiltersToQuery(url, filters);
        }
        if (sort && sort.dir) {
            url = this.addSort(url, sort);
        }

        return this.client.get<FolderGetAllModel>(url);
    }

    //Function (request) Create Folder
    public addFolder(folder: FolderCreateModel): Observable<boolean> {
        return this.client.post<boolean>(this.urlMain, folder);
    }

    //Function (request) Download Folder Zip
    public downloadFolderZip(folderPath: string, folderName: string): Observable<Blob> {
        let path: string = encodeURIComponent(folderPath);

        return this.client.get(`${this.urlMain}/zip?folderPath=${path}&folderName=${folderName}`, {
            responseType: 'blob'
        });
    }

    //Function (request) Delete Folder
    public deleteFolder(folder: FolderDeleteModel): Observable<boolean> {
        return this.client.delete<boolean>(this.urlMain, { body: folder })
    }

    //Function (helper) Add filter query in getAll request url
    private addFiltersToQuery(url: string, params: { 'Filter.Logic': string; 'Filter.Filters': Array<SubFilter>; }): string {
        let resultUrl: string = url;
        if (params['Filter.Filters'].length > 0) {
            resultUrl += `&Filter.Logic=${params['Filter.Logic']}`;
            for (let i = 0; i < params['Filter.Filters'].length; i++) {
                const subFilter = params['Filter.Filters'][i];

                if (subFilter.filters && subFilter.filters.length > 0)
                    for (let j = 0; j < subFilter.filters.length; j++) {
                        const filterDefinition = subFilter.filters[j];
                        resultUrl += `&Filter.Filters[${i}].Filters[${j}].Field=${filterDefinition.field}`;
                        resultUrl += `&Filter.Filters[${i}].Filters[${j}].Operator=${filterDefinition.operator}`;
                        resultUrl += `&Filter.Filters[${i}].Filters[${j}].Value=${filterDefinition.value}`;
                    }
                resultUrl += `&Filter.Filters[${i}].Logic=${subFilter.logic}`;
            }
        }
        return resultUrl;
    }

    //Function (helper) Add sort query in getAll request url
    private addSort(url: string, sort: SortModel): string {
        const resultUrl = url + `&SortDir=${sort.dir}&SortField=${sort.field}`;
        return resultUrl;
    }

}   