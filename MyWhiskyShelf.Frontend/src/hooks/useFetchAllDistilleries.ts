import * as React from "react";
// @ts-ignore
import { getAllDistilleries } from "@/api/distilleriesApi";

export interface UseDistilleriesResult<T> {
  data: T[];
  loading: boolean;
  error: unknown;
  refresh: () => Promise<void>;
}

export function useFetchAllDistilleries<T = any>(): UseDistilleriesResult<T> {
  const [data, setData] = React.useState<T[]>([]);
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState<unknown>(null);

  const load = React.useCallback(async () => {
    setLoading(true);
    setError(null);

    const ctrl = new AbortController();
    try {
      const rows = await getAllDistilleries();
      setData(rows as unknown as T[]);
    } catch (err) {
      
      setError(err);
    } finally {
      setLoading(false);
    }
    return () => ctrl.abort();
  }, []);

  
  React.useEffect(() => {
    
    (async () => {
      try {
        await load();
      } catch {}
    })();
    return () => {};
  }, [load]);

  const refresh = React.useCallback(async () => {
    await load();
  }, [load]);

  return { data, loading, error, refresh };
}
