export function formatRating(rating?: number | null): string {
  if (rating == null) return "N/A";
  return Number.isInteger(rating) ? rating.toString() : rating.toFixed(1);
}