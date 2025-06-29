import { Link, NavLink } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const navItems = [
  { label: 'Home', path: '/' },
  { label: 'Items', path: '/items' },
  { label: 'Categories', path: '/categories' },
  { label: 'Stats', path: '/stats' },
];

export default function Header() {
  const { user, logout } = useAuth();
  
  return (
    <header className="bg-white shadow mb-6">
      <div className="container mx-auto px-4 py-4 flex justify-between items-center">
        <Link to="/" className="text-2xl font-bold text-indigo-600">
          WTSYT
        </Link>

        <nav className="space-x-4 flex items-center">
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
            <div className="ml-4 flex items-center space-x-3">
              <span className="text-sm text-gray-600">Hello, {user.displayName}</span>
              <button onClick={logout} className="text-red-500 text-sm hover:underline">
                Logout
              </button>
            </div>
          ) : (
            <NavLink to="/login" className="text-blue-600 hover:underline ml-4">
              Login
            </NavLink>
          )}
        </nav>
      </div>
    </header>
  );
}