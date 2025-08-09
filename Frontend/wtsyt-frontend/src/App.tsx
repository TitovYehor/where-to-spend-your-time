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
import Tags from './pages/Tags.tsx';

export default function App() {
  return (
    <div className="min-h-screen bg-gray-50 text-gray-800">
      <Header />
      <Routes>
        <Route path="/login" element={ <Login/> } />
        <Route path="/register" element={ <Register/> } />
        
        <Route element={ <Layout/> }>
          <Route path="/" element={ <Home/> } />
          
          <Route path="/admin" element={ <RequireAdmin><AdminDashboard/></RequireAdmin> } />
          <Route path="/admin/categories" element={ <RequireAdmin><AdminCategories/></RequireAdmin> } />
          <Route path="/admin/items" element={ <RequireAdmin><AdminItems/></RequireAdmin> } />
          <Route path="/admin/tags" element={ <RequireAdmin><AdminTags/></RequireAdmin> } />

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
  );
}