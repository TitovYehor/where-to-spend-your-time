import { Link, NavLink } from 'react-router-dom';

const navItems = [
  { label: 'Home', path: '/' },
  { label: 'Items', path: '/items' },
  { label: 'Categories', path: '/categories' },
  { label: 'Stats', path: '/stats' },
];

export default function Header() {
  return (
    <header className="bg-white shadow mb-6">
      <div className="container mx-auto px-4 py-4 flex justify-between items-center">
        <Link to="/" className="text-2xl font-bold text-indigo-600">
          WTSYT
        </Link>
        <nav className="space-x-4">
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
        </nav>
      </div>
    </header>
  );
}