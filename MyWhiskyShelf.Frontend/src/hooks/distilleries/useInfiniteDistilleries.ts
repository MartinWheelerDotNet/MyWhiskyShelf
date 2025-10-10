import * as React from "react";
// @ts-ignore
import { getAllDistilleries } from "@/api/distilleries/distilleriesApi";
// @ts-ignore
import { Distillery } from "@/lib/domain/types";

type State = {
    items: Distillery[];
    page: number;           
    amount: number;
    loading: boolean; 
    isPageLoading: boolean;
    error: unknown;
    hasMore: boolean;
};

type Options = {
    root?: React.RefObject<Element | null>;
    rootMargin?: string;
    threshold?: number;
};

export function useInfiniteDistilleries(initialAmount = 10, opts: Options = {}) {
    const { root, rootMargin = "800px", threshold = 0 } = opts;

    const [state, setState] = React.useState<State>({
        items: [],
        page: 0,
        amount: initialAmount,
        loading: true,
        isPageLoading: false,
        error: null,
        hasMore: true,
    });

    const fetchingRef = React.useRef(false);
    const hasMoreRef = React.useRef(true);
    const amountRef = React.useRef(initialAmount);
    React.useEffect(() => { hasMoreRef.current = state.hasMore; }, [state.hasMore]);

    const [sentinelEl, setSentinelEl] = React.useState<HTMLDivElement | null>(null);
    const setSentinel = React.useCallback((el: HTMLDivElement | null) => {
        setSentinelEl(el);
    }, []);

    const getId = (x: any) => String(x.id ?? x.Id ?? x.distilleryId ?? "");

    const load = React.useCallback(async (pageToLoad: number) => {
        if (fetchingRef.current || !hasMoreRef.current) return;
        fetchingRef.current = true;

        setState(s => ({
            ...s,
            loading: s.items.length === 0,
            isPageLoading: s.items.length > 0,
            error: null,
        }));

        try {
            const res = await getAllDistilleries(pageToLoad, amountRef.current);

            setState(s => {
                const seen = new Set(s.items.map(getId));
                const merged = s.items.concat(res.items.filter((it: Distillery) => !seen.has(getId(it))));
                const hasMore = res.items.length === s.amount;
                return {
                    ...s,
                    items: merged,
                    page: res.page,
                    loading: false,
                    isPageLoading: false,
                    hasMore,
                };
            });
        } catch (e) {
            setState(s => ({ ...s, loading: false, isPageLoading: false, error: e }));
        } finally {
            fetchingRef.current = false;
        }
    }, []);
    
    React.useEffect(() => { void load(1); }, [load]);

    const loadNext = React.useCallback(() => {
        setState(s => {
            if (fetchingRef.current || !s.hasMore) return s;
            const next = s.page + 1;
            void load(next);
            return s;
        });
    }, [load]);

    const refresh = React.useCallback(() => {
        fetchingRef.current = false;
        hasMoreRef.current = true;
        amountRef.current = initialAmount;
        setState({
            items: [],
            page: 0,
            amount: initialAmount,
            loading: true,
            isPageLoading: false,
            error: null,
            hasMore: true,
        });
        void load(1);
    }, [initialAmount, load]);

    React.useEffect(() => {
        if (!sentinelEl) return;
        const rootEl = root?.current ?? null;
        const io = new IntersectionObserver(
            entries => { if (entries[0]?.isIntersecting) loadNext(); },
            { root: rootEl, rootMargin, threshold }
        );
        io.observe(sentinelEl);
        return () => io.disconnect();
    }, [sentinelEl, root, rootMargin, threshold, loadNext]);

    React.useEffect(() => {
        const target = (root?.current as HTMLElement | null) ?? globalThis;
        const onScroll = () => {
            if (fetchingRef.current || !hasMoreRef.current) return;
            const nearBottom = (() => {
                if (root?.current) {
                    const el = root.current as HTMLElement;
                    return el.scrollTop + el.clientHeight >= el.scrollHeight - 200;
                } else {
                    return window.scrollY + window.innerHeight >= document.documentElement.scrollHeight - 200;
                }
            })();
            if (nearBottom) loadNext();
        };
        target.addEventListener("scroll", onScroll, { passive: true });
        return () => target.removeEventListener("scroll", onScroll);
    }, [root, loadNext]);

    React.useEffect(() => {
        if (fetchingRef.current || !hasMoreRef.current) return;
        const rootEl = (root?.current as HTMLElement | null);
        const viewport = rootEl ? rootEl.clientHeight : window.innerHeight;
        const scrollHeight = rootEl ? rootEl.scrollHeight : document.documentElement.scrollHeight;
        if (scrollHeight <= viewport + 120) loadNext();
    }, [state.items.length, root, loadNext]);

    return {
        items: state.items,
        loading: state.loading,
        isPageLoading: state.isPageLoading,
        error: state.error,
        hasMore: state.hasMore,
        endReached: !state.hasMore && state.items.length > 0,
        page: state.page,
        amount: state.amount,
        refresh,
        setSentinel,
        loadNext,
    };
}