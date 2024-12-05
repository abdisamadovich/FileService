import { Injectable } from "@angular/core";
import { CompositeFilterDescriptor, FilterDescriptor } from "@progress/kendo-data-query";
import { SubFilter } from "@@viewmodels/common/filter/sub-filter";
import { FilterState } from "@@viewmodels/common/filter/filter-state";

@Injectable({ providedIn: "root" })
export class FilterHelperServices {
    public convertFilters(filter: CompositeFilterDescriptor): SubFilter[] {
        const result: SubFilter[] = [];
        for (let i = filter.filters.length - 1; i >= 0; i--) {
            const currentFilter: CompositeFilterDescriptor = <any>filter.filters[i];
            if (!currentFilter || !currentFilter.logic) {
                filter.filters.splice(i, 1);
            }
        }
        for (let i = 0; i < filter.filters.length; i++) {
            const currentFilter: CompositeFilterDescriptor = <any>filter.filters[i];
            if (currentFilter)
                result.push({
                    logic: currentFilter.logic,
                    filters:
                        currentFilter.filters?.map((x) => {
                            const descriptor: FilterDescriptor = <any>x;
                            let strVal;
                            if (
                                typeof descriptor.value == 'object' &&
                                descriptor.value.constructor == Date
                            ) {
                                if (
                                    descriptor.operator === 'lte' ||
                                    descriptor.operator === 'gt'
                                ) {
                                    const oneDayInMs = 24 * 60 * 60 * 1000;
                                    strVal = new Date(
                                        descriptor.value.getTime() + oneDayInMs
                                    ).toISOString();
                                } else {
                                    strVal = descriptor.value.toISOString();
                                }
                            } else {
                                strVal = descriptor.value;
                            }
                            return {
                                field: <string>(<any>descriptor.field),
                                operator: <string>(<any>descriptor.operator),
                                value: strVal,
                            };
                        }) || [],
                });
        }
        return result;
    }

    public creteInitialFilterState(): FilterState {
        return {
            filter: {
                filters: [],
                logic: 'and',
            }
        };
    }

}
