import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './pages/Home.tsx';
import Items from './pages/Items.tsx';
import Header from './components/Header.tsx';

export default function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gray-50 text-gray-800">
        <Header />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/items" element={<Items />} />
        </Routes>
      </div>
    </Router>
  );
}