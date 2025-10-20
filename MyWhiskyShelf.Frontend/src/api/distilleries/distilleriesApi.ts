// @ts-ignore
import { axiosClient } from "@/api/axiosClient";
// @ts-ignore
import { DistilleryResponse } from "@/api/distilleries/contracts/types";
// @ts-ignore
import type { CursorPagedResponse } from "@/lib/api/paging";
// @ts-ignore
import { mapToDistillery } from "@/lib/mappers/distillery";
// @ts-ignore
import type { Distillery } from "@/lib/domain/types";

type GetAllParams = {
    cursor?: string | null;
    amount?: number;
    signal?: AbortSignal;
};

export async function getAllDistilleries(
    { cursor = null, amount = 10, signal }: GetAllParams = {}
): Promise<CursorPagedResponse<Distillery>> {
    const res = await axiosClient.get<CursorPagedResponse<DistilleryResponse>>("/distilleries", {
        params: { amount, ...(cursor ? { cursor } : {}) },
        signal,
    });

    const data = res.data;
    return {
        items: data.items.map(mapToDistillery),
        nextCursor: data.nextCursor,
        amount: data.amount,
    };
}