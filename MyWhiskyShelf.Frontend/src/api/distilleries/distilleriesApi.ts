// @ts-ignore
import { axiosClient } from "@/api/axiosClient";
// @ts-ignore
import { DistilleryResponse } from "@/api/distilleries/contracts/types";
// @ts-ignore
import {PagedResponse} from "@/lib/api/paging";

export async function getAllDistilleries(
    page = 1,
    amount = 10,
    signal?: AbortSignal
): Promise<PagedResponse<DistilleryResponse>> {
    const res = await axiosClient.get<PagedResponse<DistilleryResponse>>("/distilleries", {
        params: { page, amount },
        signal,
    });
    return res.data;
}