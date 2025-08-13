import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

export default function GuestOnlyRoute() {
    const { user } = useAuth();
    return user ? <Navigate to="/" replace /> : <Outlet />;
};