import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { Link } from 'react-router-dom';
import type { Item } from "../types/item";
import type { Review } from "../types/review";
import { getItemById } from "../services/itemService";
import { getMyReviewForItem, addReview, updateReview, deleteReview, getPagedReviewsForItem } from "../services/reviewService";
import { handleApiError } from "../utils/handleApi";
import { getMediaUrl } from "../services/mediaService";

export default function ItemDetails() {
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

  const itemId = Number(id);

  const fetchItem = async () => {
    if (isNaN(itemId)) return;
    try {
      const itemData = await getItemById(itemId);
      setItem(itemData);
    } catch (e) {
      handleApiError(e);
    }
  };

  const fetchReviews = async () => {
    if (isNaN(itemId)) return;
    try {
      const reviewsData = await getPagedReviewsForItem(itemId, { page, pageSize });
      setReviews(reviewsData.items);
      setTotalReviews(reviewsData.totalCount);
    } catch (e) {
      handleApiError(e);
    }
  };

  const fetchMyReview = async () => {
    if (!user || isNaN(itemId)) return;
    try {
      const myReviewData = await getMyReviewForItem(itemId);
      setMyReview(myReviewData);
      setTitle(myReviewData.title);
      setContent(myReviewData.content);
      setRating(myReviewData.rating);
    } catch {
      setMyReview(null);
      setTitle("");
      setContent("");
      setRating(0);
    }
  };

  useEffect(() => {
    fetchItem();
    fetchReviews();
    fetchMyReview();
  }, [id, user, page]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (isNaN(itemId)) {
      setError("Invalid item ID");
      return;
    }

    try {
      if (myReview) {
        await updateReview(myReview.id, { title, content, rating });
      } else {
        await addReview({ itemId, title, content, rating });
      }

      await fetchReviews();
      await fetchMyReview();
    } catch (e: any) {
      setError(e?.response?.data?.message || "Failed to submit review");
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

      await fetchReviews();
      await fetchMyReview();
    } catch (e) {
      handleApiError(e);
    }
  };

  if (!item) return <div className="p-6">Loading item...</div>;

  const firstMedia = item.media[0];

  return (
    <div className="relative min-h-screen">
      {firstMedia && (
        <div className="fixed inset-0 z-0 overflow-hidden">
          {firstMedia.type === "Image" ? (
            <img
              src={getMediaUrl(firstMedia.url)}
              alt="Background"
              className="w-full h-full object-cover opacity-50"
            />
          ) : (
            <video
              src={getMediaUrl(firstMedia.url)}
              autoPlay
              muted
              loop
              className="w-full h-full object-cover opacity-50"
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
            <Link to={`/?categoryId=${item.categoryId}`} className="text-sm bg-gray-100 px-3 py-1 rounded-md">
              {item.categoryName}
            </Link>
            <span className="text-yellow-600 font-semibold">
              ⭐ {item.averageRating.toFixed(1)}
            </span>
          </div>

          <div>
            <h3 className="text-lg font-semibold text-gray-800 mb-3">Media</h3>
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
                    <span className="absolute bottom-2 right-2 bg-black/60 text-white text-xs px-2 py-1 rounded">
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
                className="bg-gradient-to-r from-blue-100 to-blue-200 text-blue-800 text-xs font-medium px-3 py-1 
                          rounded-full hover:scale-105 transition"
              >
                #{tag.name}
              </Link>
            ))}
          </div>
        </article>

        <section className="mb-7">
          <h2 className="text-xl font-semibold mb-4">Reviews</h2>
          {reviews.length === 0 ? (
            <p className="text-gray-500">No reviews yet</p>
          ) : (
            <>
              <ul className="space-y-4">
                {reviews.map((review) => (
                  <li key={review.id}>
                    <Link 
                      to={`/reviews/${review.id}`}
                      className="block p-4 bg-white rounded shadow hover:shadow-md transition"
                    >
                      <h3 className="text-lg font-semibold mb-1">{review.title}</h3>
                      <p className="text-sm text-gray-500 mb-1">By {review.author}</p>
                      <p className="text-sm text-gray-700 mb-1">Content: {review.content}</p>
                      <p className="text-yellow-500 font-medium mb-1">Rating: {review.rating}/5</p>
                      <p className="text-xs text-gray-500">
                        {new Date(review.createdAt).toLocaleDateString()}
                      </p>
                    </Link>
                  </li>
                ))}
              </ul>

              <div className="flex justify-center items-center gap-4 mt-6">
                <button
                  disabled={page === 1}
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  className="px-3 py-1 bg-gray-200 rounded disabled:opacity-50"
                >
                  Previous
                </button>
                <span className="text-sm text-gray-700">
                  Page {page} of {Math.ceil(totalReviews / pageSize)}
                </span>
                <button
                  disabled={page * pageSize >= totalReviews}
                  onClick={() => setPage((p) => p + 1)}
                  className="px-3 py-1 bg-gray-200 rounded disabled:opacity-50"
                >
                  Next
                </button>
              </div>
            </>
          )}
        </section>

        {user && (
          <section>
            <h3 className="text-lg font-bold mb-4">
              {myReview ? "Edit your review" : "Write a review"}
            </h3>

            {error && <p className="text-red-500 mb-2">{error}</p>}
            
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
                  required
                />
              </div>

              <div>
                <label htmlFor="review-content" className="block text-sm font-medium text-black mb-1">
                  Content
                </label>
                <textarea
                  id="review-content"
                  placeholder="Write your review..."
                  className="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  value={content}
                  onChange={(e) => setContent(e.target.value)}
                  required
                />
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
                  className="bg-indigo-600 text-white py-2 px-4 rounded hover:bg-indigo-700 transition"
                >
                  {myReview ? "Update Review" : "Submit Review"}
                </button>
                
                {myReview && (
                  <button
                    type="button"
                    className="text-red-600 text-sm hover:underline"
                    onClick={handleDelete}
                  >
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