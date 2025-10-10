export interface PagedResponse<T> {
    items: T[];
    page: number;
    amount: number;
}