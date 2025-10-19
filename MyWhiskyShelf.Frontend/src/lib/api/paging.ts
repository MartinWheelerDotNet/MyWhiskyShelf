export interface CursorPagedResponse<T> {
    items: T[];
    nextCursor: string | null;
    amount: number;
}