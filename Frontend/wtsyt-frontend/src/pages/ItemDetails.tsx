import { useEffect, useRef, useState } from "react";
import { useAutoResizeTextareas } from "../hooks/useAutoResizeTextareas";
import { useParams } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { Link } from 'react-router-dom';
import type { Item } from "../types/item";
import type { Review } from "../types/review";
import { getItemById } from "../services/itemService";
import { getMyReviewForItem, addReview, updateReview, deleteReview, getPagedReviewsForItem } from "../services/reviewService";
import { handleApiError } from "../utils/handleApi";
import { getMediaUrl } from "../services/mediaService";
import ReviewCard from "../components/reviews/ReviewCard";
import { Image as ImageIcon, Video, Tag as TagIcon, FileText, Star, FolderOpen, Pencil, Trash2, ChevronLeft, ChevronRight } from "lucide-react";
import { formatRating } from "../utils/formatters";
import Alert from "../components/common/Alerts";

interface ItemDetailsProps {
  setDisableBackground?: (disabled: boolean) => void;
}

export default function ItemDetails({ setDisableBackground }: ItemDetailsProps) {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();

  const [item, setItem] = useState<Item | null>(null);
  const [reviews, setReviews] = useState<Review[]>([]);
  const [myReview, setMyReview] = useState<Review | null>(null);

  const [totalReviews, setTotalReviews] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(3);
  
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [rating, setRating] = useState(0);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  const itemId = Number(id);

  const reviewContentRef = useRef<HTMLTextAreaElement | null>(null);

  const fetchItem = async (signal?: AbortSignal) => {
    if (isNaN(itemId)) return;
    try {
      const itemData = await getItemById(itemId, signal);
      setItem(itemData);
    } catch (e) {
      if (!signal?.aborted) {
        setError(handleApiError(e));
      }
    }
  };

  const fetchReviews = async (signal?: AbortSignal) => {
    if (isNaN(itemId)) return;
    try {
      const reviewsData = await getPagedReviewsForItem(itemId, { page, pageSize }, signal);
      setReviews(reviewsData.items);
      setTotalReviews(reviewsData.totalCount);
    } catch (e) {
      if (!signal?.aborted) {
        setError(handleApiError(e));
      }
    }
  };

  const fetchMyReview = async (signal?: AbortSignal) => {
    if (!user || isNaN(itemId)) return;
    try {
      const myReviewData = await getMyReviewForItem(itemId, signal);
      setMyReview(myReviewData);
      setTitle(myReviewData.title);
      setContent(myReviewData.content);
      setRating(myReviewData.rating);
    } catch (e) {
      if (!signal?.aborted) {
        setMyReview(null);
        setTitle("");
        setContent("");
        setRating(0);
        setError(handleApiError(e));
      }
    }
  };

  useAutoResizeTextareas([reviewContentRef], [content]);

  useEffect(() => {
    const controller = new AbortController();

    fetchItem(controller.signal);
    fetchReviews(controller.signal);
    fetchMyReview(controller.signal);
    
    return () => controller.abort();
  }, [id, user, page]);

  useEffect(() => {
    if (setDisableBackground) {
      setDisableBackground(!!item?.media?.length);
    }
    return () => setDisableBackground?.(false);
  }, [item, setDisableBackground]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setMessage("");

    if (isNaN(itemId)) {
      setError("Invalid item ID");
      return;
    }

    try {
      if (myReview) {
        await updateReview(myReview.id, { title, content, rating });
        setMessage("Review updated");
      } else {
        await addReview({ itemId, title, content, rating });
        setMessage("Review created");
      }

      await fetchReviews();
      await fetchMyReview();
    } catch (e: any) {
      setMessage("");
      const message = handleApiError(e);
      setError(message);
    }
  };

  const handleDelete = async () => {
    if (!myReview) return;

    try {
      await deleteReview(myReview.id);
      setMyReview(null);
      setTitle("");
      setContent("");
      setRating(0);
      setMessage("Review deleted");

      await fetchReviews();
      await fetchMyReview();
    } catch (e) {
      const message = handleApiError(e);
      setError(message);
    }
  };

  if (!item) return <div className="p-6">Loading item...</div>;

  const firstMedia = item.media[0];

  return (
    <div className={firstMedia ? "relative min-h-screen" : ""}>
      {firstMedia && (
        <div className="fixed inset-0 z-0 overflow-hidden">
          {firstMedia.type === "Image" ? (
            <img
              src={getMediaUrl(firstMedia.url)}
              alt="Background"
              className="w-full h-full object-cover"
            />
          ) : (
            <video
              src={getMediaUrl(firstMedia.url)}
              autoPlay
              muted
              loop
              className="w-full h-full object-cover"
            />
          )}
          <div className="absolute inset-0 bg-black/40" />
        </div>
      )}

      <div className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl">
        <article className="mb-7">
          <h1 className="text-4xl font-bold mb-4 text-gray-900">{item.title}</h1>
          <p className="text-lg text-gray-700 mb-3">{item.description}</p>

          <div className="flex items-center gap-4 mb-3">
            <Link 
              to={`/?categoryId=${item.categoryId}`} 
              className="flex items-center gap-1 text-sm bg-gray-100 text-blue-600 px-3 py-1 rounded-md"
            >
              <FolderOpen className="w-4 h-4" />
              {item.categoryName}
            </Link>
            <span className="flex items-center gap-1 text-yellow-600 font-semibold">
              <Star className="w-4 h-4" />
              {formatRating(item.averageRating)}/5
            </span>
          </div>

          <div>
            <h3 className="text-lg font-semibold text-gray-800 mb-3 flex items-center gap-2">
              <ImageIcon className="w-5 h-5 text-blue-500" />
              Media
            </h3>

            {item.media.length === 0 ? (
              <p className="text-sm text-gray-500 italic">No media uploaded yet</p>
            ) : (
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                {item.media.map((m) => (
                  <div
                    key={m.id}
                    className="relative group rounded-xl overflow-hidden shadow-md hover:shadow-xl transition cursor-pointer"
                  >
                    {m.type === "Image" ? (
                      <img
                        src={getMediaUrl(m.url)}
                        alt="Media"
                        className="h-40 w-full object-cover group-hover:scale-105 transition-transform"
                        onClick={() => window.open(getMediaUrl(m.url), "_blank")}
                      />
                    ) : (
                      <video
                        src={getMediaUrl(m.url)}
                        controls
                        className="h-40 w-full object-cover"
                      />
                    )}
                    <span className="absolute bottom-2 right-2 bg-black/60 text-white text-xs px-2 py-1 rounded flex items-center gap-1">
                      {m.type === "Image" ? <ImageIcon className="w-3 h-3" /> : <Video className="w-3 h-3" />}
                      {m.type}
                    </span>
                  </div>
                ))}
              </div>
            )}
          </div>

          <div className="mt-5 flex flex-wrap gap-2">
            {item.tags.map((tag) => (
              <Link
                key={tag.id}
                to={`/?tagsids=${tag.id}`}
                className="flex items-center gap-1 bg-blue-100 text-green-700 text-xs font-medium px-3 py-1 rounded-full hover:scale-105 transition"
              >
                <TagIcon className="w-4 h-4" />
                {tag.name}
              </Link>
            ))}
          </div>
        </article>

        <section className="mb-7">
          <h2 className="text-xl font-semibold mb-4 flex items-center gap-2">
            <FileText className="w-5 h-5 text-blue-500" />
            Reviews
          </h2>

          {reviews.length === 0 ? (
            <p className="text-gray-500">No reviews yet</p>
          ) : (
            <>
              <ul className="space-y-4">
                {reviews.map((review) => (
                  <ReviewCard key={review.id} review={review} />
                ))}
              </ul>

              <div className="flex justify-center items-center gap-4 mt-6">
                <button
                  disabled={page === 1}
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  className="p-2 rounded bg-gray-200 disabled:opacity-50"
                  aria-label="Previous page"
                >
                  <ChevronLeft className="w-5 h-5" />
                </button>
                <span className="text-sm text-gray-700">
                  Page {page} of {Math.ceil(totalReviews / pageSize)}
                </span>
                <button
                  disabled={page * pageSize >= totalReviews}
                  onClick={() => setPage((p) => p + 1)}
                  className="p-2 rounded bg-gray-200 disabled:opacity-50"
                  aria-label="Next page"
                >
                  <ChevronRight className="w-5 h-5" />
                </button>
              </div>
            </>
          )}
        </section>

        {user && (
          <section>
            <h3 className="text-lg font-bold mb-4 flex items-center gap-2">
              <Pencil className="w-5 h-5 text-indigo-600" />
              {myReview ? "Edit your review" : "Write a review"}
            </h3>

            <Alert type="success" message={message} onClose={() => setMessage("")} />
            <Alert type="error" message={error} onClose={() => setError("")} />
            
            <form onSubmit={handleSubmit} className="space-y-4">
              <input type="hidden" value={item.id} readOnly />

              <div>
                <label htmlFor="review-title" className="block text-sm font-medium text-black mb-1">
                  Title
                </label>
                <input
                  id="review-title"
                  type="text"
                  placeholder="Title"
                  className="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  maxLength={80}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  {title?.length || 0}/80 characters
                </p>
              </div>

              <div>
                <label htmlFor="review-content" className="block text-sm font-medium text-black mb-1">
                  Content
                </label>
                <textarea
                  ref={reviewContentRef}
                  id="review-content"
                  placeholder="Write your review..."
                  className="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  value={content}
                  onChange={(e) => {
                    setContent(e.target.value);
                  }}
                  maxLength={1000}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  {content?.length || 0}/1000 characters
                </p>
              </div>

              <div>
                <label htmlFor="review-rating" className="block text-sm font-medium text-black mb-1">
                  Rating
                </label>
                <input
                  id="review-rating"
                  type="number"
                  min="1"
                  max="5"
                  className="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  placeholder="Rating (1-5)"
                  value={rating}
                  onChange={(e) => setRating(Number(e.target.value))}
                  required
                />
              </div>

              <div className="flex flex-wrap gap-4 mt-2">
                <button
                  type="submit"
                  className="flex items-center gap-1 bg-indigo-600 text-white py-2 px-4 rounded hover:bg-indigo-700 transition"
                >
                  <Pencil className="w-4 h-4" />
                  {myReview ? "Update Review" : "Submit Review"}
                </button>
                
                {myReview && (
                  <button
                    type="button"
                    className="flex items-center gap-1 text-red-600 text-sm hover:underline"
                    onClick={handleDelete}
                  >
                    <Trash2 className="w-4 h-4" />
                    Delete Review
                  </button>
                )}
              </div>
            </form>
          </section>
        )}
      </div>
    </div>
  );
}