export interface PaginationMetaData {
    hasPrevious: boolean;
    hasNext: boolean;
    currentPage: number;
    currentPageSize: number
    totalPages: number;
    pageSize: number;
    totalItems: number;
}