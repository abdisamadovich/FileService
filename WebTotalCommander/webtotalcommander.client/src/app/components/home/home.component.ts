//Angular local Libraries
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Location } from '@angular/common';

//begin:: Kendo

//Kendo BreadCrumb
import { BreadCrumbItem } from '@progress/kendo-angular-navigation';
//Kenod Grid Libraries
import { CellClickEvent, PageChangeEvent } from '@progress/kendo-angular-grid';
//Filter and Sort Descriptor
import { CompositeFilterDescriptor, SortDescriptor, } from '@progress/kendo-data-query';
import { UploadEvent, SuccessEvent, ErrorEvent } from '@progress/kendo-angular-upload';

//end:: Kendo

//Toastr
import { ToastrService } from 'ngx-toastr';

//Filter Helper Service
import { FilterHelperServices } from '@@services/helpers/filter-helper.services';
import { KendoIcons } from '@@services/helpers/get-icons';
//Service Folder
import { FolderService } from '@@services/folder.service';
//ServiceFile
import { FileService } from '@@services/file.service';
//ViewModels GetAll
import { GridDataView } from '@@viewmodels/common/folder_file_getall/grid.data.view';
import { FolderFileViewModel } from '@@viewmodels/common/folder_file_getall/folder.file.view-model';
import { PaginationMetaDataView } from '@@viewmodels/common/folder_file_getall/pagination.data';
//ViewModel Filter
import { FilterState } from '@@viewmodels/common/filter/filter-state';
//ViewModel Sort
import { SortViewModel } from '@@viewmodels/common/sort/sort.view-model';
//ViewModel File
import { FileViewDeleteModel } from '@@viewmodels/file/file.view-delete.model';
import { FileViewEditModel } from '@@viewmodels/file/file.view-edit.model';
//ViewModel Folder
import { FolderCreateViewModel } from '@@viewmodels/folder/folder.view-create.model';
import { FolderDeleteViewModel } from '@@viewmodels/folder/folder.view-delete.model';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
    @ViewChild('fileInput') fileInput!: ElementRef;

    //Construktor
    constructor(
        private toastr: ToastrService,
        private _serviceFolder: FolderService,
        private _serviceFile: FileService,
        private _serviceFilter: FilterHelperServices,
        public _iconsKendo: KendoIcons,
        public _location: Location
    ) { }

    //Variables Array Folder GetALL
    public gridView: GridDataView = new GridDataView();
    public paginationMetaData: PaginationMetaDataView = new PaginationMetaDataView();

    //Variables for Pagination
    public pageSize = 10;
    public skip = 0;

    //Variable Create Folder (Modal)
    public openedCreateFolderModal = false;
    public folderName = "";

    //Variables Upload files (Modal)
    public openedUploadFilesModal = false;
    public fileSource: File | null = null;

    //Variables Edit Txt File (Modal)
    public openedEditTxtModal = false;
    public txtFileContent: string = '';
    public fileNameTxtFile: string = '';

    //Variables Delete Folder (Modal)
    public openedDeleteFolderModal = false;
    public folderNameDelete: string = '';

    //Variables Delete File (Modal)
    public openedDeleteFileModal = false;
    public fileNameDelete: string = '';

    //Variable (get event dateItem when click folder or file)
    public clickSelectFolderFile!: FolderFileViewModel;

    //Variable Loader
    public isLoading: boolean = false;

    //Variable filter
    public filterState: FilterState = this._serviceFilter.creteInitialFilterState();

    //Variable for check click forward or back button
    private isClickedBackButton: boolean = false;
    private isClickedForwardButton: boolean = false;

    //Variables for disable or enable up,forward,back buttons
    public isBackDisabled: boolean = true;
    public isForwardDisabled: boolean = true;
    public isUpDisabled: boolean = true;

    //Variable upload file url
    public saveUrl: string = `api/file`

    //Variable sort
    public sort: SortDescriptor[] = [
        {
            field: 'name',
            dir: undefined,
        }
    ];

    //Arrays (for BreadCrumb Values collector and transmitter)
    private collectorBreadCrumbValue: BreadCrumbItem[] = [
        {
            text: 'Home',
            title: 'Home',
            svgIcon: this._iconsKendo.homeIcon,
        }
    ];

    public transmitterBreadCrumbValue: BreadCrumbItem[] = [
        ...this.collectorBreadCrumbValue,
    ];

    //Arrays Back and Forwar (buttons) history
    public backHistory: Array<BreadCrumbItem[]> = [];
    public forwardHistory: Array<BreadCrumbItem[]> = [];
    public pageHistory: Array<number> = [];

    //Function NgOnit
    ngOnInit(): void {
        this.toFillBreadCrumbWithUrlPath()
        this.getAll(this.skip, this.pageSize);
    }

    //Function (ngOnInit) GetAll Folders and Files
    public getAll(skip: number, take: number): void {
        this.isLoading = true;
        //Create Filter object and get filters
        const filter = {
            'Filter.Logic': 'and',
            'Filter.Filters': this._serviceFilter.convertFilters(this.filterState.filter)
        };
        //Create Sort object and get sort
        const sortViewModel: SortViewModel = new SortViewModel();
        sortViewModel.dir = this.sort[0].dir;
        sortViewModel.field = this.sort[0].field;
        //Load to get folder path from url and if url has page number and size then get values for page number and page size
        let path: string = this.extractPathFromUrl();
        skip = this.skip;
        take = this.pageSize;

        //Logic to retrieve folders and files in the backend
        this._serviceFolder.getFolder(path, skip, take, sortViewModel, filter)
            .subscribe({
                next: (response) => {
                    this.gridView.data = response.folderFile;
                    this.gridView.total = response.paginationMetaData.totalItems;
                    this.paginationMetaData = response.paginationMetaData;
                    //To add a return row to the table if the da enters the folder
                    if (path.length !== 0) {
                        this.creteBackRowInGridRows();
                    }
                    if (this.isClickedBackButton || this.isClickedForwardButton) {
                        this.clearFilter();
                        this.clearSort();
                    }
                    this.refreshBreadCrumb();
                    this.isLoading = false;
                    this.isClickedBackButton = false;
                    this.isClickedForwardButton = false;
                },
                error: (err) => {
                    if (err.status === 404) {
                        if (this.isClickedBackButton) {
                            this.toastr.warning('This place cannot be back history. This location may have changed!');
                            this.reloadPathWhenBackFolderNotFound();
                        }
                        else if (this.isClickedForwardButton) {
                            this.toastr.warning('This place cannot be forward history. This location may have changed!');
                            this.reloadPathWhenForwardFolderNotFound();
                        }
                        else {
                            this.toastr.warning('Folder path not found!');
                        }
                        this.refreshBreadCrumb();
                        this.isLoading = false;
                    } else {
                        this.toastr.warning('An error occurred while retrieving folders and files!');
                        this.isLoading = false;
                    }
                    this.isClickedBackButton = false;
                    this.isClickedForwardButton = false;
                },
            });
    }

    //Function for cut path from url, define skip and take
    private extractPathFromUrl(): string {
        //Get folder path from url
        let path: string = this._location.path();

        //If the url has a page size, take the value from it and store it in the pageSize variable.
        if (path.includes("?size=")) {
            this.pageSize = (Number(path.slice(path.indexOf("?size=") + 6, path.length)));
        }
        else if (path.includes("&size=")) {
            this.pageSize = (Number(path.slice(path.indexOf("&size=") + 6, path.length)));
        }

        //If the url has a page number, take the value from it and store it in the skip variable.
        if (path.includes("?page=")) {
            if (path.includes("&size=")) {
                this.skip = (Number(path.slice(path.indexOf("?page=") + 6, path.indexOf('&'))) - 1) * this.pageSize;
            }
            else {
                this.skip = (Number(path.slice(path.indexOf("?page=") + 6, path.length)) - 1) * this.pageSize;
            }
        }
        return this.toCollectPath();
    }

    //Function to change the url by taking the path from BreadCrumb
    public changeUrl(): void {
        let path: string = this.toCollectPath();
        if (path) {
            path = `/${path}`;
        }
        this._location.replaceState(`/home${path.slice(0, path.length - 1)}`);
    }

    //Function Add Url Page and PageSize
    private addPageSizeUrl(skip: number, take: number): void {
        //Find current page by skip
        let currentPage: number = Math.ceil((skip + 1) / this.pageSize);
        let path: string = this.toCollectPath();

        if (path) {
            path = `/${path}`
        }

        if (currentPage == 1 && take == 10) {
            this._location.replaceState(`/home${path.slice(0, path.length - 1)}`);
        } else if (currentPage == 1 && take != 10) {
            this._location.replaceState(`/home${path.slice(0, path.length - 1)}?size=${take}`)
        } else if (currentPage > 1 && take == 10) {
            this._location.replaceState(`/home${path.slice(0, path.length - 1)}?page=${currentPage}`)
        } else {
            this._location.replaceState(`/home${path.slice(0, path.length - 1)}?page=${currentPage}&size=${take}`)
        }
    }

    //Function to fill Breadcrumb With Url Path
    public toFillBreadCrumbWithUrlPath(): void {
        //Get full path and parametrs in url (without /home)
        let path: string = this._location.path().slice(0, this._location.path().length);

        //If just cutting the path (without parametrs)
        if (path.indexOf('?') !== -1) {
            path = path.slice(6, path.indexOf('?'));
        } else {
            path = path.slice(6, path.length);
        }

        const folderNameArray: string[] = path.split('/').filter((segment) => segment !== '');
        for (let i: number = 0; i < folderNameArray.length; i++) {
            this.collectorBreadCrumbValue.push({
                text: folderNameArray[i],
                title: folderNameArray[i],
            });
        }
        this.transmitterBreadCrumbValue = [...this.collectorBreadCrumbValue];
    }

    //Function add back rows in grid rows
    private creteBackRowInGridRows(): void {
        const exitRow: FolderFileViewModel = new FolderFileViewModel();
        exitRow.name = '...';
        exitRow.extension = '';
        exitRow.path = '';

        this.gridView.data.unshift(exitRow);
    }

    //Functionn For Pagination (Change page)
    public pageChange({ skip, take }: PageChangeEvent): void {
        this.skip = skip;
        this.pageSize = take;
        this.addPageSizeUrl(skip, take);

        this.getAll(this.skip, this.pageSize);
    }

    //Function find skip
    public findSkipPreviousPath(): number {
        if (this.pageHistory.length > 0) {
            let itemIndex: number = this.pageHistory[this.pageHistory.length - 1];
            // Identify the page where this item is located
            let currentPage: number = Math.ceil((itemIndex + 1) / this.pageSize);
            // How many items to skip to this page
            let itemsToSkip: number = (currentPage - 1) * this.pageSize;
            this.pageHistory.pop();
            return itemsToSkip;
        } else {
            return 0;
        }
    }

    //Function Filter change
    public filterChange(ev: CompositeFilterDescriptor): void {
        if (ev) {
            this.filterState.filter = ev;
        } else {
            this.filterState.filter = {
                logic: 'and',
                filters: [],
            };
        }
        this.skip = 0;
        this.addPageSizeUrl(this.skip, this.pageSize);
        this.getAll(this.skip, this.pageSize);
    }

    //Function Clear filter
    public clearFilter(): void {
        this.filterState = this._serviceFilter.creteInitialFilterState();
    }

    //Function Sort change
    public sortChange(sort: SortDescriptor[]): void {
        this.sort = sort;
        this.skip = 0;
        this.addPageSizeUrl(this.skip, this.pageSize);
        this.getAll(this.skip, this.pageSize);
    }

    //Function Clear sort
    public clearSort(): void {
        this.sort = [{ field: 'name', dir: undefined }]
    }

    //Function (button) Close EditModal
    public closeEditTxtModal(): void {
        this.openedEditTxtModal = false;
    }

    //Function (button) Open EditModal
    public openEditTxtModal(fileName: string): void {
        this.fileNameTxtFile = fileName;
        const path: string = this.toCollectPath() + fileName;

        this._serviceFile.getTxtFile(path).subscribe({
            next: (response) => {
                this.openedEditTxtModal = true;
                const reader: FileReader = new FileReader();
                reader.onload = () => {
                    this.txtFileContent = reader.result as string;
                };
                reader.readAsText(response);
            },
            error: (err) => {
                this.toastr.warning('Error retrieving file content!');
            },
        });
    }

    //Function (button) Save EditModal
    public submitEditTxtModal(): void {
        this.isLoading = true;

        const fileEditModel: FileViewEditModel = new FileViewEditModel();
        fileEditModel.filePath = this.toCollectPath() + this.fileNameTxtFile;
        fileEditModel.file = new File([this.txtFileContent], this.fileNameTxtFile);

        this._serviceFile.editTxtFile(fileEditModel).subscribe({
            next: (response) => {
                this.toastr.success('File content updated successfully!');
                this.closeEditTxtModal();
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.warning('Error updating file content!');
                this.closeEditTxtModal();
                this.isLoading = false;
            },
        });
    }

    //Function (event) AddFolderModal
    public eventAddFolderModal(status: string): void {
        if (status === 'add') {
            if (this.folderName) {
                this.isLoading = true;
                const folderViewCreateModel = new FolderCreateViewModel();
                folderViewCreateModel.folderName = this.folderName;
                folderViewCreateModel.folderPath = this.toCollectPath();

                this._serviceFolder.addFolder(folderViewCreateModel).subscribe({
                    next: (response) => {
                        this.openedCreateFolderModal = false;
                        this.getAll(this.skip, this.pageSize);
                        this.toastr.success('Folder success created!');
                        this.folderName = '';
                        this.isLoading = false;
                    },
                    error: (err) => {
                        if (err.status == 409) {
                            this.toastr.warning('Folder already exists!');
                        } else if (err.status == 422) {
                            this.toastr.warning('Cannot create folder with this name!');
                        } else if (err.status == 404) {
                            this.toastr.warning('Folder path not found!');
                        } else {
                            this.toastr.warning('Error during folder create!');
                        }
                        this.isLoading = false;
                    },
                });
            } else {
                this.toastr.warning('Please enter a folder name!');
            }
        }
        else if (status === "cancel") {
            this.openedCreateFolderModal = false;
        }
    }

    //Function (Button) open Create folder Modal
    public openCreateFolderModal(): void {
        this.openedCreateFolderModal = true;
    }

    //Function (event) DeleteFolderModal
    public eventDeleteFolderModal(status: string): void {
        if (status === 'yes') {
            this.isLoading = true;

            const folder: FolderDeleteViewModel = new FolderDeleteViewModel();
            folder.folderName = this.folderNameDelete;
            folder.folderPath = this.toCollectPath();

            this._serviceFolder.deleteFolder(folder).subscribe({
                next: (response) => {
                    if (this.paginationMetaData.currentPageSize === 1 && this.paginationMetaData.hasPrevious) {
                        this.skip = this.skip - this.pageSize;
                        this.addPageSizeUrl(this.skip, this.pageSize);

                    }
                    this.getAll(this.skip, this.pageSize);
                    this.toastr.success('Delete folder success!');
                    this.isLoading = false;
                },
                error: (err) => {
                    this.toastr.warning('Delete folder warning!');
                    this.isLoading = false;
                },
            });
            this.openedDeleteFolderModal = false;
        } else if (status === 'no' || status === 'cancel') {
            this.openedDeleteFolderModal = false;
        }
    }

    //Function open DeleteFolderModal
    public openDeleteModalFolder(folderName: string): void {
        this.folderNameDelete = folderName;
        this.openedDeleteFolderModal = true;
    }

    //Function (Button) opne Upload file Modal
    public opneUploadFileModal(): void {
        this.openedUploadFilesModal = true;
    }

    //Function (Button) Close Upload Files Modal
    public closeUploadFilesModal(): void {
        this.openedUploadFilesModal = false;
    }

    //Function Upload file add filePath in event
    public uploadEventHandler(e: UploadEvent): void {
        e.data = {
            filePath: this.toCollectPath(),
        };
    }

    //Function Success (When successfuly upload file GetFiles)
    public successEventHandler(event: SuccessEvent) {
        this.toastr.success(`${event.files[0].name} file upload!`);
        this.getAll(this.skip, this.pageSize)
    }

    //Function Error (When error upload file show toastr)
    public errorEventHandler(event: ErrorEvent) {
        if (event.response.status === 409) {
            this.toastr.warning(`${event.files[0].name} file already exsist!`);
        }
        else {
            this.toastr.warning(`${event.files[0].name} file uploading file unexpected error!`);
        }
    }

    //Function (event) DeleteFileModal
    public eventDeleteModalFile(status: string): void {
        if (status === 'yes') {
            this.isLoading = true;

            const file: FileViewDeleteModel = new FileViewDeleteModel();
            file.fileName = this.fileNameDelete;
            file.filePath = this.toCollectPath();

            this._serviceFile.deleteFile(file).subscribe({
                next: (response) => {
                    if (this.paginationMetaData.currentPageSize === 1 && this.paginationMetaData.hasPrevious) {
                        this.skip = this.skip - this.pageSize;
                        this.addPageSizeUrl(this.skip, this.pageSize);
                    }
                    this.getAll(this.skip, this.pageSize);
                    this.toastr.success('Delete file success!');
                    this.openedDeleteFileModal = false;
                    this.isLoading = false;
                },
                error: (err) => {
                    this.toastr.warning('Delete file warning!');
                    this.openedDeleteFileModal = false;
                    this.isLoading = false;
                },
            });
        } else if (status === 'no' || status === 'cancel') {
            this.openedDeleteFileModal = false;
        }
    }

    //Function open DeleteFileModal
    public openDeleteModalFile(fileName: string): void {
        this.fileNameDelete = fileName;
        this.openedDeleteFileModal = true;
    }

    // Function (Button) Refresh BreadCrumb
    public refreshBreadCrumb(): void {
        if (this.collectorBreadCrumbValue.length <= 1) {
            this.isUpDisabled = true;
        }
        else {
            this.isUpDisabled = false;
        }

        if (this.backHistory.length === 0) {
            this.isBackDisabled = true;
        }
        else {
            this.isBackDisabled = false;
        }

        if (this.forwardHistory.length === 0) {
            this.isForwardDisabled = true
        }
        else {
            this.isForwardDisabled = false;
        }

        this.transmitterBreadCrumbValue = [...this.collectorBreadCrumbValue];
    }

    //Function BreadCrumb Item click
    public onBreadCrumbFolderClick(item: BreadCrumbItem): void {
        const indexFolderNameInBreadCrumbs = this.collectorBreadCrumbValue.findIndex((e) => e.text === item.text);
        let countCutBreadCrumbItems = this.collectorBreadCrumbValue.length - (indexFolderNameInBreadCrumbs + 1);
        this.backHistory.push(this.collectorBreadCrumbValue.slice());
        this.isBackDisabled = false;
        this.collectorBreadCrumbValue = this.collectorBreadCrumbValue.slice(0, indexFolderNameInBreadCrumbs + 1);
        if (this.collectorBreadCrumbValue.length === 1) {
            this.isUpDisabled = true;
        }
        //Remove history paths in pageHistory
        for (let i = 1; i < countCutBreadCrumbItems; i++) {
            this.pageHistory.pop();
        }
        //Clear sort and filter
        this.clearSort();
        this.clearFilter();
        //To find the previous path's page
        this.skip = this.findSkipPreviousPath();
        //Change the url to the new path
        this.changeUrl();

        this.addPageSizeUrl(this.skip, this.pageSize);
        //To update BreadCrumb and fetch folders and files in the new path
        this.getAll(this.skip, this.pageSize);
        this.refreshBreadCrumb();
    }

    //Function (Tables Items Folder and File dbclick)
    public doubleClickFolderFile(args: MouseEvent): void {
        //For double Click only works when it happens on the Table body
        const target = args.target as HTMLElement;
        const tbody = target.closest('tbody');
        if (tbody) {
            if (this.clickSelectFolderFile.extension === 'folder') {
                //Save the current path to history (for back button)
                this.backHistory.push(this.collectorBreadCrumbValue.slice());
                this.isBackDisabled = false;
                this.isUpDisabled = false;
                //Add the folder name to BreadCrumb
                this.collectorBreadCrumbValue.push({
                    text: this.clickSelectFolderFile.name,
                    title: this.clickSelectFolderFile.name
                });
                //Find folder fndex and save to page history
                const indexFolder: number = this.findFolderIndex();
                this.pageHistory.push(indexFolder);
                //Clear sort and filter
                this.clearSort();
                this.clearFilter();
                //To start from the first page when entering the folder
                this.skip = 0;
                //Change the url to the new path
                this.changeUrl();
                //Add Url pageNumber and pageSize
                this.addPageSizeUrl(this.skip, this.pageSize);
                //To update BreadCrumb and fetch folders and files in the new path
                this.getAll(this.skip, this.pageSize);
                this.refreshBreadCrumb();
            } else if (this.clickSelectFolderFile.extension === '') {
                //Save the current path to history (for back button)
                this.backHistory.push(this.collectorBreadCrumbValue.slice());
                this.isBackDisabled = false;
                //To remove the last folder name from BreadCrumb when you click Back
                this.collectorBreadCrumbValue.pop();
                if (this.collectorBreadCrumbValue.length === 1) {
                    this.isUpDisabled = true;
                }
                //Clear sort and filter
                this.clearSort();
                this.clearFilter();
                //To find the previous path's page
                this.skip = this.findSkipPreviousPath();
                //Change the url to the new path
                this.changeUrl();
                //Add Url pageNumber and pageSize
                this.addPageSizeUrl(this.skip, this.pageSize);
                //To update BreadCrumb and fetch folders and files in the new path
                this.getAll(this.skip, this.pageSize);
                this.refreshBreadCrumb();
            } else {
                //To create a file path
                const path: string = `${this.toCollectPath()}${this.clickSelectFolderFile.name}`;
                this.downloadFile(path, this.clickSelectFolderFile.name);
            }
        }
    }

    //Function find folder index
    public findFolderIndex(): number {
        let index = this.gridView.data.findIndex((e) => e.name === this.clickSelectFolderFile.name);
        index += this.skip;
        return index
    }

    //Function get event when click folder or file
    public cellClickHandler(args: CellClickEvent): void {
        this.clickSelectFolderFile = args.dataItem;
    }

    //Function get event when click icon
    public folderFileIconClick(event: MouseEvent, dataItem: FolderFileViewModel): void {
        // Prevent the event from propagating to the parent elements
        event.stopPropagation();
        this.clickSelectFolderFile = dataItem;
    }

    //Fuction make path
    public toCollectPath(): string {
        let result: string = '';
        if (this.collectorBreadCrumbValue.length === 1) {
            return result;
        } else {
            for (let i = 1; i < this.collectorBreadCrumbValue.length; i++) {
                result += `${this.collectorBreadCrumbValue[i].text}/`;
            }
            return result;
        }
    }

    //Function Download file
    public downloadFile(filePath: string, fileName: string): void {
        this.isLoading = true;
        this._serviceFile.downloadFile(filePath).subscribe(
            (response: Blob) => {
                // Create a Blob from the file data
                const blob = new Blob([response], { type: `application/octet-stream` });

                // Create a link element
                const link = document.createElement('a');

                // Set the download attribute and create a URL for the blob
                link.download = `${fileName}`;
                link.href = window.URL.createObjectURL(blob);

                // Append the link to the body and trigger the click event
                document.body.appendChild(link);
                link.click();

                // Clean up: remove the link and revoke the URL
                document.body.removeChild(link);
                window.URL.revokeObjectURL(link.href);

                this.toastr.success('File download successful!');
                this.isLoading = false;
            },
            (error) => {
                this.toastr.warning('File download error!');
                this.isLoading = false;
            }
        );
    }

    //Download Folder Zip
    public downloadFolderZip(folderName: string): void {
        const folderPath: string = this.toCollectPath();
        this.isLoading = true;

        this._serviceFolder.downloadFolderZip(folderName, folderPath).subscribe(
            (response: Blob) => {
                // Create a Blob from the file data
                const blob = new Blob([response], { type: `application/zip` });

                // Create a link element
                const link = document.createElement('a');

                // Set the download attribute and create a URL for the blob
                link.download = `${folderName}.zip`;
                link.href = window.URL.createObjectURL(blob);

                // Append the link to the body and trigger the click event
                document.body.appendChild(link);
                link.click();

                // Clean up: remove the link and revoke the URL
                document.body.removeChild(link);
                window.URL.revokeObjectURL(link.href);

                this.toastr.success('Folder download successful!');
                this.isLoading = false;
            },
            (error) => {
                this.toastr.warning('Folder download error!');
                this.isLoading = false;
            }
        );
    }

    //Function Back (button)
    public backFolder(): void {
        if (this.backHistory.length > 0) {
            this.forwardHistory.push(this.collectorBreadCrumbValue.slice());
            this.isForwardDisabled = false;
            this.collectorBreadCrumbValue = this.backHistory.pop()!.slice();
            this.skip = 0;
            //Change the url to the new path
            this.changeUrl();
            //Add Url pageNumber and pageSize
            this.addPageSizeUrl(this.skip, this.pageSize);
            this.isClickedBackButton = true;

            this.getAll(this.skip, this.pageSize)
        }
    }

    //Function Forward (button)
    public forwardFolder(): void {
        if (this.forwardHistory.length > 0) {
            this.backHistory.push(this.collectorBreadCrumbValue.slice());
            this.isBackDisabled = false;
            this.collectorBreadCrumbValue = this.forwardHistory.pop()!.slice();
            this.skip = 0;
            //Change the url to the new path
            this.changeUrl();
            //Add Url pageNumber and pageSize
            this.addPageSizeUrl(this.skip, this.pageSize);

            this.isClickedForwardButton = true;
            this.getAll(this.skip, this.pageSize)
        }
    }

    //Up (button)
    public upFolder(): void {
        if (this.collectorBreadCrumbValue.length > 1) {
            this.backHistory.push(this.collectorBreadCrumbValue.slice());
            this.collectorBreadCrumbValue.pop();
            this.skip = this.findSkipPreviousPath();
            //Clear sort and filter
            this.clearSort();
            this.clearFilter();
            //Change the url to the new path
            this.changeUrl();
            //Add Url pageNumber and pageSize
            this.addPageSizeUrl(this.skip, this.pageSize);

            this.getAll(this.skip, this.pageSize);
            this.refreshBreadCrumb();
        }
    }

    //Function for If the path is not found when returning to Back History. Restoring BreadCrumb to its previous state
    public reloadPathWhenBackFolderNotFound(): void {
        this.forwardHistory.pop();
        let saveCollectorOldValue: BreadCrumbItem[] = this.collectorBreadCrumbValue.slice();
        this.collectorBreadCrumbValue = this.backHistory[this.backHistory.length - 1].slice();
        this.backHistory.push(saveCollectorOldValue.slice());
        //Change the url to the new path
        this.changeUrl();
        //Add Url pageNumber and pageSize
        this.addPageSizeUrl(this.skip, this.pageSize);

        this.refreshBreadCrumb()
    }

    //Function for If the path is not found when returning to Forward History. Restoring BreadCrumb to its previous state
    public reloadPathWhenForwardFolderNotFound(): void {
        let saveCollectorOldValue: BreadCrumbItem[] = this.collectorBreadCrumbValue.slice();
        this.collectorBreadCrumbValue = this.backHistory.pop()!.slice();
        this.forwardHistory.push(saveCollectorOldValue.slice());
        //Change the url to the new path
        this.changeUrl();
        //Add Url pageNumber and pageSize
        this.addPageSizeUrl(this.skip, this.pageSize);

        this.refreshBreadCrumb()
    }
}
