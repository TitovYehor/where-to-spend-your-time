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

  return (
    <header className="bg-white shadow mb-6">
      <div className="container mx-auto px-4 py-4 flex justify-between items-center">
        <Link to="/" className="text-2xl font-bold text-indigo-600">
          WTSYT
        </Link>

        {user?.role === "Admin" && (
          <div className="flex items-center gap-x-4">
            <Link to="/admin" className="text-bg font-bold text-indigo-600 hover:underline">
              Admin Panel
            </Link>
          </div>
        )}

        <nav className="flex items-center gap-x-4">
          {navItems.map(({ label, path }) => (
            <NavLink
              key={path}
              to={path}
              className={({ isActive }) =>
                `text-gray-700 hover:text-indigo-600 ${
                  isActive ? 'font-semibold underline' : ''
                }`
              }
            >
              {label}
            </NavLink>
          ))}

          {user ? (
            <div className="flex items-center gap-x-4">
              <span className="text-sm text-gray-600">Hello, {user.displayName}</span>
              <button onClick={userLogout} className="text-red-500 text-sm hover:underline">
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