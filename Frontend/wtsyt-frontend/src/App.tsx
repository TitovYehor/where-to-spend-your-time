import { Routes, Route } from 'react-router-dom';
import Layout from "./components/Layout.js";
import Login from "./pages/Login";
import Register from './pages/Register.tsx';
import Home from './pages/Home.tsx';
import Profile from './pages/Profile.tsx';
import PublicProfile from './pages/PublicProfilePage.tsx';
import ItemDetails from './pages/ItemDetails.tsx';
import ReviewDetails from './pages/ReviewDetails.tsx';
import Categories from './pages/Categories.tsx';
import Stats from './pages/Stats.tsx';
import Header from './components/Header.tsx';
import RequireAdmin from './components/RequireAdmin.tsx';
import AdminDashboard from './pages/admin/AdminDashboard.tsx';
import AdminCategories from './pages/admin/AdminCategories.tsx';
import AdminItems from './pages/admin/AdminItems.tsx';
import AdminTags from './pages/admin/AdminTags.tsx';
import AdminUsers from './pages/admin/AdminUsers.tsx';
import Tags from './pages/Tags.tsx';
import GuestOnlyRoute from './components/GuestOnlyRoute.tsx';
import Background from './components/Background.tsx';
import { useState } from 'react';

export default function App() {
  const [solidBackground, setSolidBackground] = useState(false);
  
  return (
    <div className="min-h-screen text-gray-800 relative">
      <Background solid={solidBackground} />
      
      <div className="relative z-10">
        <Header toggleBackground={() => setSolidBackground(v => !v)} solid={solidBackground} />
          
        <Routes>
          <Route element={ <GuestOnlyRoute/> }>
            <Route path="/login" element={ <Login/> } />
            <Route path="/register" element={ <Register/> } />
          </Route>

          <Route element={ <Layout/> }>
            <Route path="/" element={ <Home/> } />
            
            <Route path="/admin" element={ <RequireAdmin><AdminDashboard/></RequireAdmin> } />
            <Route path="/admin/categories" element={ <RequireAdmin><AdminCategories/></RequireAdmin> } />
            <Route path="/admin/items" element={ <RequireAdmin><AdminItems/></RequireAdmin> } />
            <Route path="/admin/tags" element={ <RequireAdmin><AdminTags/></RequireAdmin> } />
            <Route path="/admin/users" element={ <RequireAdmin><AdminUsers/></RequireAdmin> } />

            <Route path="/profile" element={ <Profile/> } />
            <Route path="/users/:userId" element={ <PublicProfile/> } />
            <Route path="/items/:id" element={ <ItemDetails/> } />
            <Route path="/reviews/:reviewId" element={ <ReviewDetails/> } />
            <Route path="/categories" element={ <Categories/>} />
            <Route path="/tags" element={ <Tags/> } />
            <Route path="/stats" element={ <Stats/> } />
          </Route>
        </Routes>
      </div>
    </div>
  );
}