import { Outlet, useLocation, useNavigate } from "react-router-dom";

export default function Layout() {
  const location = useLocation();
  const navigate = useNavigate();

  const hideBackButton = ["/", "/login", "/register"].includes(location.pathname);

  return (
    <div className="relative max-w-xl mx-auto px-4 py-1">
      {!hideBackButton && (
        <button
          onClick={() => navigate(-1)}
          className="absolute top-1 left-1 px-3 py-1 bg-blue-500 text-white text-bg rounded hover:bg-blue-600 transition z-10"
        >
          ← Back
        </button>
      )}

      <Outlet />
    </div>
  );
}