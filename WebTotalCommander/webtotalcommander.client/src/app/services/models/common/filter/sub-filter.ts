import { FilterDefinition } from "./filter-definition";

export interface SubFilter {
    filters?: null | Array<FilterDefinition>;
    logic: string;
  }