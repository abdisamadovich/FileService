import { Injectable } from '@angular/core';
//Kenod Icons
import {
    SVGIcon,

    arrowRotateCcwIcon,
    homeIcon,
    downloadIcon,
    trashIcon,
    pencilIcon,
    //file icons
    fileAddIcon,
    filePdfIcon,
    fileExcelIcon,
    fileWordIcon,
    fileImageIcon,
    fileTxtIcon,
    fileAudioIcon,
    fileTypescriptIcon,
    fileVideoIcon,
    filePptIcon,
    exeIcon,
    fileProgrammingIcon,
    fileZipIcon,
    jsIcon,
    csIcon,
    csprojIcon,
    vbIcon,
    vbprojIcon,
    css3Icon,
    html5Icon,
    fileIcon,
    slnIcon,
    //folder icons
    folderUpIcon,
    folderAddIcon,
    folderIcon,
    //left up and right icons
    arrowLeftIcon,
    arrowRightIcon,
    arrowUpIcon,
} from '@progress/kendo-svg-icons';

@Injectable({ providedIn: 'root' })
export class KendoIcons {
    //Variables SVGIcon
    public homeIcon: SVGIcon = homeIcon;
    public downloadIcon: SVGIcon = downloadIcon;
    public rotateIcon: SVGIcon = arrowRotateCcwIcon;
    public deleteIcon: SVGIcon = trashIcon;
    public editIcon: SVGIcon = pencilIcon;
    public arrowLeft: SVGIcon = arrowLeftIcon;
    public arrowRight: SVGIcon = arrowRightIcon;
    public arrowUp: SVGIcon = arrowUpIcon;
    public folderAddIcon: SVGIcon = folderAddIcon;
    public fileAddIcon: SVGIcon = fileAddIcon;

    //Dictionary FileIcons
    private fileIcons: { [key: string]: SVGIcon } = {
        default: fileIcon,
        folder: folderIcon,
        //video
        '.mp4': fileVideoIcon,
        '.avi': fileVideoIcon,
        '.mkv': fileVideoIcon,
        '.wmv': fileVideoIcon,
        '.mov': fileVideoIcon,
        '.flv': fileVideoIcon,
        '.webm': fileVideoIcon,
        '.3gp': fileVideoIcon,
        '.3g2': fileVideoIcon,
        '.mpg': fileVideoIcon,
        '.mpeg': fileVideoIcon,

        //image
        '.jpg': fileImageIcon,
        '.jpeg': fileImageIcon,
        '.png': fileImageIcon,
        '.gif': fileImageIcon,
        '.tiff': fileImageIcon,
        '.tif': fileImageIcon,
        '.bmp': fileImageIcon,
        '.raw': fileImageIcon,
        '.svg': fileImageIcon,
        '.psd': fileImageIcon,
        '.eps': fileImageIcon,
        '.webp': fileImageIcon,

        //music
        '.mp3': fileAudioIcon,
        '.aac': fileAudioIcon,
        '.wav': fileAudioIcon,
        '.flac': fileAudioIcon,
        '.ogg': fileAudioIcon,

        //exel
        '.xlsx': fileExcelIcon,
        '.xls': fileExcelIcon,

        //word
        '.docx': fileWordIcon,
        '.doc': fileWordIcon,

        //power point
        '.pptx': filePptIcon,
        '.ppt': filePptIcon,

        //program languages
        '.py': fileProgrammingIcon,
        '.js': jsIcon,
        '.ts': fileTypescriptIcon,
        '.cs': csIcon,
        '.csproj': csprojIcon,
        '.vb': vbIcon,
        '.vbproj': vbprojIcon,
        '.sln': slnIcon,
        '.html': html5Icon,
        '.css': css3Icon,

        '.txt': fileTxtIcon,
        '.exe': exeIcon,
        '.msi': exeIcon,
        '.zip': fileZipIcon,
        '.rar': fileZipIcon,
        '.pdf': filePdfIcon,
        '': folderUpIcon,
    };

    //Function Get File and Folder extension
    public getIconForExtension(extension: string): SVGIcon {
        return this.fileIcons[extension.toLowerCase()] || fileIcon;
    }
}
