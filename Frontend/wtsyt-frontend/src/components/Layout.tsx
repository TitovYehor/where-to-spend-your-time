import { Outlet, useLocation, useNavigate } from "react-router-dom";

export default function Layout() {
  const location = useLocation();
  const navigate = useNavigate();

  const hideBackButton = ["/", "/login", "/register"].includes(location.pathname);

  const handleBack = () => {
    if (window.history.length > 1) {
      navigate(-1);
    } else {
      navigate("/");
    }
  };

  return (
    <div className="relative px-4 py-6">
      {!hideBackButton && (
        <button
          onClick={handleBack}
          className="absolute top-2 left-0 mt-1 ml-4 px-3 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 transition z-10"
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