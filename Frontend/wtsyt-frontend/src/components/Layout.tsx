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
          className="absolute top-2 left-0 mt-1 ml-4 px-4 py-2 bg-gray-800/80 text-gray-200 rounded-lg hover:bg-gray-700/90 
            backdrop-blur-md border border-gray-600/50 transition-all duration-200 z-10 flex items-center gap-1 group shadow-md 
            hover:shadow-lg"
        >
          <svg 
            className="w-4 h-4 group-hover:-translate-x-1 transition-transform duration-200" 
            fill="none" 
            stroke="currentColor" 
            viewBox="0 0 24 24"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
          </svg>
          <span className="font-medium">Back</span>
        </button>
      )}

      <div className="max-w-3xl mx-auto">
        <Outlet />
      </div>
    </div>
  );
}