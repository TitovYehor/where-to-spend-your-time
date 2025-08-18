import { Link, NavLink } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export default function Header() {
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
    <header className="bg-white shadow-sm border-b border-gray-200 sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-6 py-3 flex justify-between items-center">
        <button
          onClick={() => handleReplaceNavigate("/")}
          className="text-2xl font-bold text-indigo-600 hover:text-indigo-700 transition-colors"
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
                `text-sm text-gray-600 hover:text-indigo-600 transition-colors ${
                  isActive ? 'font-semibold underline' : ''
                }`
              }
            >
              {label}
            </NavLink>
          ))}

          {user?.role === "Admin" && (
            <Link
              to="/admin"
              className="bg-indigo-600 text-white px-3 py-1.5 rounded-md shadow-sm hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 transition text-sm font-medium"
            >
              Admin Panel
            </Link>
          )}

          {user ? (
            <div className="flex items-center gap-x-4">
              <span className="text-sm text-gray-600">Hello, {user.displayName}</span>
              <button
                onClick={handleLogout}
                className="text-red-500 text-sm hover:underline"
              >
                Logout
              </button>
            </div>
          ) : (
            <div className="flex items-center gap-x-4">
              <NavLink to="/login" className="text-blue-600 hover:underline">
                Login
              </NavLink>
              <NavLink to="/register" className="text-blue-600 hover:underline">
                Register
              </NavLink>
            </div>
        )}
        </nav>
      </div>
    </header>
  );
}