import { Outlet, useLocation, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

export default function Layout() {
  const location = useLocation();
  const navigate = useNavigate();

  const [canGoBack, setCanGoBack] = useState(false);

  useEffect(() => {
    const historyIndex = window.history.state?.idx ?? 0;
    setCanGoBack(historyIndex > 0);
  }, [location]);

  const hideBackButton = !canGoBack || ["/login", "/register"].includes(location.pathname);

  const handleBack = () => {
    if (canGoBack) {
      window.history.back();
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