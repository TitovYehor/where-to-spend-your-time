import { Link, NavLink } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

interface HeaderProps {
  toggleBackground: () => void;
  solid: boolean;
}

export default function Header({ toggleBackground, solid }: HeaderProps) {
  const { user, userLogout } = useAuth();
  
  const navItems = [
    { label: 'Home', path: '/' },
    { label: 'Categories', path: '/categories' },
    { label: 'Tags', path: '/tags' },
    { label: 'Stats', path: '/stats' },
    ...(user ? [{ label : 'Profile', path: '/profile' }] : []),
  ];

  const handleReplaceNavigate = (path: string) => {
    window.location.replace(path);
  };

  const handleLogout = async () => {
    await userLogout();
    window.location.replace("/");
  };

  return (
    <header className="fixed top-0 left-0 w-full bg-gray-900/90 shadow-lg border-b border-gray-700 z-50 backdrop-blur-xl">

      <div className="max-w-7xl mx-auto px-6 py-3 flex justify-between items-center">
        <button
          onClick={() => handleReplaceNavigate("/")}
          className="text-2xl font-bold text-white hover:text-indigo-300 transition-colors"
          aria-label="Go to home"
        >
          WTSYT
        </button>

        <nav className="flex items-center gap-x-4">
          {navItems.map(({ label, path }) => (
            <NavLink
              key={path}
              to={path}
              className={({ isActive }) =>
                `text-sm font-medium transition-colors ${
                  isActive 
                  ? 'text-white underline font-semibold' 
                  : 'text-gray-300 hover:text-white'
                }`
              }
            >
              {label}
            </NavLink>
          ))}

          {user?.role === "Admin" && (
            <Link
              to="/admin"
              className="bg-indigo-600 text-white px-4 py-2 rounded-md shadow-md hover:bg-indigo-500 
                focus:outline-none focus:ring-2 focus:ring-indigo-400 focus:ring-offset-2 focus:ring-offset-gray-900 transition-all 
                text-sm font-medium"
            >
              Admin Panel
            </Link>
          )}

          {user ? (
            <div className="flex items-center gap-x-4">
              <span className="text-sm text-gray-200">Hello, {user.displayName}</span>
              <button
                onClick={handleLogout}
                className="text-red-300 hover:text-white text-sm font-medium transition-colors"
              >
                Logout
              </button>
            </div>
          ) : (
            <div className="flex items-center gap-x-4">
              <NavLink to="/login" className="text-blue-300 hover:text-white font-medium transition-colors">
                Login
              </NavLink>
              <NavLink to="/register" className="bg-blue-600 text-white px-3 py-1.5 rounded-md 
                shadow-sm hover:bg-blue-500 transition-colors font-medium">
                Register
              </NavLink>
            </div>
        )}

        <button
            onClick={toggleBackground}
            className="px-3 py-1.5 rounded-md text-sm font-medium
                       bg-gray-700/70 hover:bg-gray-600 text-white shadow-sm transition"
          >
            {solid ? "Aurora Mode" : "Solid Mode"}
          </button>
        </nav>
      </div>
    </header>
  );
}