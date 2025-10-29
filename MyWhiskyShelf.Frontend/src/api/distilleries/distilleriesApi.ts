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

type DistilleryFilterParams = {
    cursor?: string | null;
    amount?: number;
    pattern?: string | undefined;
    countryId?: string | undefined;
    regionId?: string | undefined;
    signal?: AbortSignal;
};

export async function getAllDistilleries(
    { pattern, cursor = null, amount = 10, countryId, regionId, signal }: DistilleryFilterParams = {}
): Promise<CursorPagedResponse<Distillery>> {
    const res = await axiosClient.get<CursorPagedResponse<DistilleryResponse>>("/distilleries", {
        params: { pattern, amount, countryId, regionId, ...(cursor ? { cursor } : {}) },
        signal,
    });

    const data = res.data;
    return {
        items: data.items.map(mapToDistillery),
        nextCursor: data.nextCursor,
        amount: data.amount,
    };
}