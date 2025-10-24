import * as React from "react";
// @ts-ignore
import { getAllDistilleries } from "@/api/distilleries/distilleriesApi";
// @ts-ignore
import { Distillery } from "@/lib/domain/types";

type State = {
    items: Distillery[];
    cursor: string | null;
    page: number;
    amount: number;
    loading: boolean;
    isPageLoading: boolean;
    error: unknown;
    hasMore: boolean;
};

const getId = (d: Distillery) => d.id;

export function useInfiniteDistilleries(options?: {
    initialAmount?: number;
    root?: React.RefObject<Element>;
    rootMargin?: string;
    threshold?: number;
}) {
    const {
        initialAmount = 24,
        root,
        rootMargin = "0px 0px 400px 0px",
        threshold = 0.1,
    } = options ?? {};

    const [state, setState] = React.useState<State>({
        items: [],
        cursor: null,
        page: 0,
        amount: initialAmount,
        loading: false,
        isPageLoading: false,
        error: null,
        hasMore: true,
    });

    const amountRef = React.useRef(state.amount);
    const fetchingRef = React.useRef(false);
    const hasMoreRef = React.useRef(true);
    React.useEffect(() => { hasMoreRef.current = state.hasMore; }, [state.hasMore]);

    const refresh = React.useCallback(async () => {
        if (fetchingRef.current) return;
        fetchingRef.current = true;

        setState(s => ({ ...s, loading: true, isPageLoading: false, error: null, items: [], cursor: null, page: 0 }));
        try {
            const res = await getAllDistilleries({ cursor: null, amount: amountRef.current });
            setState(s => ({
                ...s,
                items: res.items,
                cursor: res.nextCursor ?? null,
                page: 1,
                loading: false,
                isPageLoading: false,
                hasMore: Boolean(res.nextCursor),
            }));
        } catch (e) {
            setState(s => ({ ...s, loading: false, isPageLoading: false, error: e }));
        } finally {
            fetchingRef.current = false;
        }
    }, []);

    const loadNext = React.useCallback(async () => {
        if (fetchingRef.current || !hasMoreRef.current) return;
        fetchingRef.current = true;

        setState(s => ({
            ...s,
            loading: s.items.length === 0,
            isPageLoading: s.items.length > 0,
            error: null,
        }));

        try {
            const res = await getAllDistilleries({ cursor: state.cursor, amount: amountRef.current });
            setState(s => {
                const seen = new Set(s.items.map(getId));
                const merged = s.items.concat(res.items.filter((it: Distillery) => !seen.has(getId(it))));
                const hasMore = Boolean(res.nextCursor);

                return {
                    ...s,
                    items: merged,
                    cursor: res.nextCursor ?? null,
                    page: s.page + 1,
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
    }, [state.cursor]);

    // sentinel management
    const [sentinelEl, setSentinel] = React.useState<Element | null>(null);
    const setSentinelCb = React.useCallback((el: Element | null) => setSentinel(el), []);

    React.useEffect(() => {
        if (!sentinelEl) return;
        const rootEl = (root?.current as Element | null) ?? null;
        const io = new IntersectionObserver(
            entries => { if (entries[0]?.isIntersecting) loadNext(); },
            { root: rootEl, rootMargin, threshold }
        );
        io.observe(sentinelEl);
        return () => io.disconnect();
    }, [sentinelEl, root, rootMargin, threshold, loadNext]);

    // fallback scroll listener (if no sentinel)
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

    // kick off initial load
    React.useEffect(() => {
        if (state.items.length === 0 && !fetchingRef.current) {
            void refresh();
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

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
        setSentinel: setSentinelCb,
        loadNext,
    };
}