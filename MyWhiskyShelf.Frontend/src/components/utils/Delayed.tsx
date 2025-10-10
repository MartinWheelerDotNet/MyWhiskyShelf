import * as React from "react";

export default function Delayed({
                                    show,
                                    delayMs = 300,
                                    children,
                                }: {
    show: boolean;
    delayMs?: number;
    children: React.ReactNode;
}) {
    const [visible, setVisible] = React.useState(false);
    React.useEffect(() => {
        if (!show) { setVisible(false); return; }
        const t = setTimeout(() => setVisible(true), delayMs);
        return () => clearTimeout(t);
    }, [show, delayMs]);
    return visible ? <>{children}</> : null;
}