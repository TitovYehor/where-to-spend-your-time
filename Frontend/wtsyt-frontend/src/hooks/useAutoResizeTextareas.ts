import { useEffect, type RefObject } from "react";

export function useAutoResizeTextareas(
  refs: RefObject<HTMLTextAreaElement | null>[],
  values: (string | undefined)[]
) {
  useEffect(() => {
    const autoResize = (el: HTMLTextAreaElement | null) => {
      if (!el) return;
      el.style.height = "auto";
      el.style.height = `${el.scrollHeight}px`;
    };

    requestAnimationFrame(() => {
      refs.forEach((ref) => autoResize(ref.current));
    });
  }, values);
}