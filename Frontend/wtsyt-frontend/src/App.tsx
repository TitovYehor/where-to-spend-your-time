import { Routes, Route } from 'react-router-dom';
import Login from "./pages/Login";
import Register from './pages/Register.tsx';
import Home from './pages/Home.tsx';
import Profile from './pages/Profile.tsx';
import Items from './pages/Items.tsx';
import ItemDetails from './pages/ItemDetails.tsx';
import Header from './components/Header.tsx';

export default function App() {
  return (
    <div className="min-h-screen bg-gray-50 text-gray-800">
      <Header />
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/" element={<Home />} />
        <Route path="/profile" element={<Profile />} />
        <Route path="/items" element={<Items />} />
        <Route path="/items/:id" element={ <ItemDetails />} />
      </Routes>
    </div>
  );
}