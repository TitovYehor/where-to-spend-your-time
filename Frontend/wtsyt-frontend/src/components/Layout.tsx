import { Outlet, useLocation, useNavigate } from "react-router-dom";

export default function Layout() {
  const location = useLocation();
  const navigate = useNavigate();

  const hideBackButton = ["/", "/login", "/register"].includes(location.pathname);

  return (
    <div className="relative px-4 py-6">
      {!hideBackButton && (
        <button
          onClick={() => navigate(-1)}
          className="absolute top-0 left-0 mt-1 ml-4 px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 transition z-10"
        >
          ← Back
        </button>
      )}

      <div className="max-w-3xl mx-auto">
        <Outlet />
      </div>
    </div>
  );
}