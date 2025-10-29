import * as React from "react";
// @ts-ignore
import { getAllDistilleries } from "@/api/distilleries/distilleriesApi";
// @ts-ignore
import { Distillery } from "@/lib/domain/types";

type State = {
    items: Distillery[];
    loading: boolean;
    isPageLoading: boolean;
    error: unknown;
    hasMore: boolean;
    cursor: string | null;
    amount: number;
};

export function useInfiniteDistilleries(options?: {
    initialAmount?: number;
    root?: React.RefObject<Element>;
    rootMargin?: string;
    threshold?: number;
    countryId?: string | null | undefined;
    regionId?: string | null | undefined;
    pattern?: string | null | undefined;
}) {
    const {
        initialAmount = 12,
        root,
        rootMargin = "0px 0px 400px 0px",
        threshold = 0.1,
        countryId,
        regionId,
        pattern,
    } = options ?? {};

    const [state, setState] = React.useState<State>({
        items: [],
        loading: false,
        isPageLoading: false,
        error: null,
        hasMore: true,
        cursor: null,
        amount: initialAmount,
    });

    const fetchingRef = React.useRef(false);
    const hasMoreRef = React.useRef(true);
    React.useEffect(() => {
        hasMoreRef.current = state.hasMore;
    }, [state.hasMore]);

    const filtersRef = React.useRef<{ countryId?: string | null; regionId?: string | null; pattern?: string | null }>({
        countryId: countryId ?? null,
        regionId: regionId ?? null,
        pattern: pattern ?? null,
    });
    React.useEffect(() => {
        filtersRef.current = { countryId: countryId ?? null, regionId: regionId ?? null, pattern: pattern ?? null };
    }, [countryId, regionId, pattern]);

    const refresh = React.useCallback(async () => {
        if (fetchingRef.current) return;
        fetchingRef.current = true;
        setState((s) => ({
            ...s,
            loading: true,
            isPageLoading: false,
            error: null,
            items: [],
            cursor: null,
            hasMore: true,
        }));
        try {
            const res: any = await getAllDistilleries({
                amount: state.amount,
                countryId: filtersRef.current.countryId ?? undefined,
                regionId: filtersRef.current.regionId ?? undefined,
                pattern: filtersRef.current.pattern ?? undefined,
                cursor: null
            });
            setState((s) => ({
                ...s,
                items: res.items ?? [],
                cursor: res.nextCursor ?? null,
                loading: false,
                isPageLoading: false,
                hasMore: Boolean(res.nextCursor),
            }));
        } catch (e) {
            setState((s) => ({ ...s, loading: false, isPageLoading: false, error: e }));
        } finally {
            fetchingRef.current = false;
        }
    }, [state.amount]);

    const loadNext = React.useCallback(async () => {
        if (fetchingRef.current || !hasMoreRef.current) return;
        fetchingRef.current = true;
        setState((s) => ({
            ...s,
            loading: s.items.length === 0,
            isPageLoading: s.items.length > 0,
            error: null,
        }));
        try {
            const res: any = await getAllDistilleries({
                cursor: state.cursor ?? undefined,
                amount: state.amount,
                countryId: filtersRef.current.countryId ?? undefined,
                regionId: filtersRef.current.regionId ?? undefined,
                pattern: filtersRef.current.pattern ?? undefined,
            });
            setState((s) => {
                const seen = new Set(s.items.map(d => d.id));
                const merged = s.items.concat((res.items ?? []).filter((it: Distillery) => !seen.has(it.id)));
                return {
                    ...s,
                    items: merged,
                    cursor: res.nextCursor ?? null,
                    loading: false,
                    isPageLoading: false,
                    hasMore: Boolean(res.nextCursor),
                };
            });
        } catch (e) {
            setState((s) => ({ ...s, loading: false, isPageLoading: false, error: e }));
        } finally {
            fetchingRef.current = false;
        }
    }, [state.cursor, state.amount]);

    const [sentinelEl, setSentinel] = React.useState<Element | null>(null);
    const setSentinelCb = React.useCallback((el: Element | null) => setSentinel(el), []);

    React.useEffect(() => {
        if (!sentinelEl) return;
        const rootEl = (root?.current as Element | null) ?? null;
        const io = new IntersectionObserver(
            (entries) => {
                if (entries[0]?.isIntersecting) loadNext();
            },
            { root: rootEl, rootMargin, threshold }
        );
        io.observe(sentinelEl);
        return () => io.disconnect();
    }, [sentinelEl, root, rootMargin, threshold, loadNext]);

    React.useEffect(() => {
        const target = (root?.current as HTMLElement | null) ?? (globalThis as unknown as Window);
        const onScroll = () => {
            if (fetchingRef.current || !hasMoreRef.current) return;
            const nearBottom = (() => {
                if (root?.current) {
                    const el = root.current as HTMLElement;
                    return el.scrollTop + el.clientHeight >= el.scrollHeight - 200;
                } else {
                    const win = globalThis as unknown as Window;
                    const doc = win.document.documentElement;
                    return win.scrollY + win.innerHeight >= doc.scrollHeight - 200;
                }
            })();
            if (nearBottom) loadNext();
        };
        (target as any).addEventListener("scroll", onScroll, { passive: true } as AddEventListenerOptions);
        return () => (target as any).removeEventListener("scroll", onScroll);
    }, [root, loadNext]);

    React.useEffect(() => {
        void refresh();
    }, []);

    React.useEffect(() => {
        void refresh();
    }, [countryId, regionId, pattern]);

    return {
        items: state.items,
        loading: state.loading,
        isPageLoading: state.isPageLoading,
        error: state.error,
        hasMore: state.hasMore,
        refresh,
        setSentinel: setSentinelCb,
        loadNext,
    };
}
